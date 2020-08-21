

using System;
using System.IO;
using UnityEngine;

public static class LocalStorage
{
    public enum EStorageType
    {
        Common,
        SaveData,
        BulletCfg
    }

    private static string GetStoragePath(EStorageType type)
    {
        var path = PathUtility.CommonStoragePath;
        switch (type)
        {
            case EStorageType.SaveData:
                path = PathUtility.SaveDataPath;
                break;
            case EStorageType.BulletCfg:
                path = PathUtility.BulletConfigPath;
                break;
        }
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (System.Exception)
            {
                // ignored
            }
        }
        return path; 
    }

    public static T Read<T>(string key, EStorageType ePath)
    {
        System.Object value = Read(key, typeof(T), ePath);
        if (value != null)
            return (T)value;
        return default(T);
    }

    public static object Read(string key, Type type, EStorageType ePath)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(GetStoragePath(ePath) + "/" + key);

            //尝试解密
            // bytes = FileUtility.CopyTo(bytes);     
            
            BufferReader buf = new BufferReader(bytes);
            return buf.Read(type);
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public static void Write(string key, object value, EStorageType ePath)
    {
        if (value != null)
        {
            BufferWriter buf = new BufferWriter();
            buf.WriteObject(value);
            Fragment[] fragment;
            byte[] bytes = buf.GetBuffer(out fragment);

            //整个这段是加密
            {
                MemoryStream ms = new MemoryStream();
                for (int i = 0; i < fragment.Length; ++i)
                    ms.Write(bytes, fragment[i].begin, fragment[i].length);
                ms.Flush();
                bytes = ms.ToArray();
                var newb = new byte[bytes.Length];
                Array.Copy(bytes, newb, ms.Length);

                //加密保存
                //bytes = FileUtility.CopyFrom(newb, 5);

                fragment = new Fragment[1];
                fragment[0].begin = 0;
                fragment[0].length = bytes.Length;
            }

            try
            {
                if (bytes != null && fragment.Length > 0)
                    using (FileStream stm = new FileStream(GetStoragePath(ePath) + "/" + key, FileMode.Create))
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
            string path = GetStoragePath(ePath) + "/" + key;
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    public static bool Contains(string key, EStorageType ePath)
    {
        return File.Exists(GetStoragePath(ePath) + "/" + key);
    }

    public static bool Remove(string key, EStorageType ePath)
    {
        bool removed = false;
        string path = GetStoragePath(ePath) + "/" + key;
        if (File.Exists(path))
        {
            File.Delete(path);
            removed = true;
        }
        return removed;
    }
}