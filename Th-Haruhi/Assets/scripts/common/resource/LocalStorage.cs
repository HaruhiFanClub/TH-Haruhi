

using System;
using System.IO;
using UnityEngine;

public static class LocalStorage
{
    private static readonly string storagePath;

    static LocalStorage()
    {
        storagePath = PathUtility.StoragePath;
        if (!Directory.Exists(storagePath))
        {
            try
            {
                Directory.CreateDirectory(storagePath);
            }
            catch (System.Exception)
            {
                // ignored
            }
        }
    }

    public static T Read<T>(string key)
    {
        System.Object value = Read(key, typeof(T));
        if (value != null)
            return (T)value;
        return default(T);
    }

    public static System.Object Read(string key, System.Type type)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(storagePath + "/" + key);
            bytes = FileUtility.CopyTo(bytes);                 //尝试解密
            BufferReader buf = new BufferReader(bytes);
            return buf.Read(type);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public static void ClearPid(string pid)
    {
        if (Directory.Exists(storagePath))
        {
            foreach (string file in Directory.GetFiles(storagePath))
                if (file.Contains(pid))
                    File.Delete(file);
            foreach (string directory in Directory.GetDirectories(storagePath))
                if (directory.Contains(pid))
                    Directory.Delete(directory, true);
        }
    }

    public static void Write(string key, System.Object value)
    {
        if (value != null)
        {
            BufferWriter buf = new BufferWriter();
            buf.WriteObject(value);
            Fragment[] fragment;
            byte[] bytes = buf.GetBuffer(out fragment);

#if UNITY_EDITOR
            //windows上方便调试,顺便你保存一份未加密的
            {
                if (bytes != null && fragment.Length > 0)
                    using (FileStream stm = new FileStream(storagePath + "/" + key + ".txt", FileMode.Create))
                    {
                        for (int i = 0; i < fragment.Length; ++i)
                            stm.Write(bytes, fragment[i].begin, fragment[i].length);
                        stm.Flush();
                        stm.Close();
                    }
            }
#endif
            
            //整个这段是加密
            {
                MemoryStream ms = new MemoryStream();
                for (int i = 0; i < fragment.Length; ++i)
                    ms.Write(bytes, fragment[i].begin, fragment[i].length);
                ms.Flush();
                bytes = ms.ToArray();
                var newb = new byte[bytes.Length];
                Array.Copy(bytes, newb, ms.Length);
                bytes = FileUtility.CopyFrom(newb, 5); //加密保存
                fragment = new Fragment[1];
                fragment[0].begin = 0;
                fragment[0].length = bytes.Length;
            }

            try
            {
                if (bytes != null && fragment.Length > 0)
                    using (FileStream stm = new FileStream(storagePath + "/" + key, FileMode.Create))
                    {
                        for (int i = 0; i < fragment.Length; ++i)
                            stm.Write(bytes, fragment[i].begin, fragment[i].length);
                        stm.Flush();
                        stm.Close();
                    }
            }
            catch (IOException e)
            {
                if (e.Message.Contains("Disk full"))
                {
                    Debug.LogError("存储空间不足，请清理存储空间！");
                    //UITip.ShowInfo(G.R(""), UITip.TipsType.Error);
                }
                else 
                {
                    throw;
                }
            }
        }
        else
        {
            string path = storagePath + "/" + key;
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    public static bool Contains(string key)
    {
        return File.Exists(storagePath + "/" + key);
    }

    public static void Clear()
    {
        if (Directory.Exists(storagePath))
        {
            foreach (string file in Directory.GetFiles(storagePath))
                File.Delete(file);
            foreach (string directory in Directory.GetDirectories(storagePath))
                Directory.Delete(directory, true);
        }
    }

    public static bool Remove(string key)
    {
        bool removed = false;
        string path = storagePath + "/" + key;
        if (File.Exists(path))
        {
            File.Delete(path);
            removed = true;
        }
        return removed;
    }
}