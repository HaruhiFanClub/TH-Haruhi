

using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public static class ResourcesUtility
{
    public static ResourceType GetResourceTypeByPath(string resource)
    {
        var extension = Path.GetExtension(resource);
        if (extension != null)
            switch (extension.ToLower())
            {
                case ".unity":
                    return ResourceType.scene;
                case ".prefab":
                    return ResourceType.prefab;
                case ".mat":
                    return ResourceType.material;
                case ".shader":
                case ".cginc":
                    return ResourceType.shader;
                case ".shadervariants":
                    return ResourceType.shadervariants;
                case ".spm":
                    return ResourceType.speedTree;
                case ".terrainlayer":
                    return ResourceType.terrainLayer;
                case ".fbx":
                case ".obj":
                    return ResourceType.fbx;
                case ".otf":
                case ".ttf":
                case ".ttc":
                case ".fontsettings":
                    return ResourceType.font;
                case ".spriteatlas":
                    return ResourceType.spriteatlas;
                case ".jpg":
                case ".jpeg":
                case ".tga":
                case ".bmp":
                case ".png":
                case ".psd":
                case ".tiff":
                case ".iff":
                case ".gif":
                case ".pict":
                case ".exr":
                case ".cubemap":
                case ".tif":
                case ".guiskin":
                case ".tsv":
                    return ResourceType.texture;
                case ".controller":
                    return ResourceType.controller;
                case ".aif":
                case ".wav":
                case ".mp3":
                case ".and":
                case ".ogg":
                    return ResourceType.audio;
                case ".haruhi":
                    return ResourceType.table;
                case ".cfg":
                    return ResourceType.config;
                case ".cs":
                case ".js":
                case ".boo":
                case ".dll":
                    return ResourceType.script;
                case ".mp4":
                    return ResourceType.mp4;
                case ".bytes":
                    return ResourceType.bytes;
                case ".asset":
                    return ResourceType.asset;
                case ".physicmaterial":
                    return ResourceType.physicMaterial;
                case ".anim":
                    return ResourceType.anim;
                case ".mask":
                    return ResourceType.mask;
                case ".flare":
                    return ResourceType.flare;
                case ".sbsar":
                    return ResourceType.sbsar;
                case "":
                    return ResourceType.folder;
                default:
                    return ResourceType.unknow;
            }
        return ResourceType.unknow;
    }

    public static System.Type TypeOfResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.fbx :
            case ResourceType.prefab :
                return typeof(GameObject);
            case ResourceType.audio :
                return typeof(AudioClip);
            case ResourceType.font :
                return typeof(Font);
            case ResourceType.shader :
                return typeof(Shader);
            case ResourceType.shadervariants:
                return typeof(ShaderVariantCollection);
            case ResourceType.material :
                return typeof(Material);
            case ResourceType.controller :
                return typeof(RuntimeAnimatorController);
            case ResourceType.physicMaterial:
                return typeof(PhysicMaterial);
            case ResourceType.spriteatlas:
                return typeof(SpriteAtlas);
            default :
                return typeof(Object);
        }
    }

    public static System.Type TypeOfResource(string resource)
    {
        return TypeOfResource(GetResourceTypeByPath(resource));
    }

    public static long GetResourceTimeStamp(string resource)
    {
        resource = PathUtility.ResourcesPathToFullPath(resource);
        return File.GetLastWriteTimeUtc(resource).ToFileTime();
    }

    public static string ComputeHash(string filename)
    {
        string hash = string.Empty;
        using (Stream stm = new FileStream(filename, FileMode.Open))
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
                hash = System.BitConverter.ToString(md5.ComputeHash(stm)).Replace("-", "").ToLower();
            stm.Close();
        }
        return hash;
    }

    public static string ComputeStrHash(string str)
    {
        string hash = string.Empty;

        using(MD5 md5 = MD5.Create())
        {
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for(int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            hash = sBuilder.ToString();
        }

        return hash;
    }

	public static long GetResourceSize(string filename)
	{
		using (Stream stm = new FileStream(filename, FileMode.Open))
		{
			return stm.Length;
			//stm.Close();
		}
		//return 0;
	}

    public static string ComputeResourceHash(string resource)
    {
        string hash = string.Empty;
        resource = PathUtility.ResourcesPathToFullPath(resource);
        try
        {
            string metaHash = string.Empty;
            string assetHash = string.Empty;

            using (Stream stm = new FileStream(resource, FileMode.Open))
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                    assetHash = System.BitConverter.ToString(
                        md5.ComputeHash(stm)).Replace("-", "").Substring(8, 16);
                stm.Close();
            }

            using (Stream stm = new FileStream(resource + ".meta", FileMode.Open))
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                    metaHash = System.BitConverter.ToString(
                        md5.ComputeHash(stm)).Replace("-", "").Substring(8, 16);
                stm.Close();
            }

            hash = assetHash + metaHash;
        }
        catch (System.Exception)
        {
        }
        return hash;
    }
    
    public static string HumanReadableFilesize(double size)
    {
        String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
        double mod = 1024.0;
        int i = 0;
        while (size>=mod )
        {
            size /= mod;
            i++;
        }
        return Math.Round(size) + units[i];

    }
    
    public static void WriteFile(string path,byte[] data) {
        FileInfo fi = new FileInfo(path);
        DirectoryInfo dir = fi.Directory;
        if (!dir.Exists) {
            dir.Create();
        }
        FileStream fs = fi.Create();
        fs.Write(data, 0, data.Length);
        fs.Flush();
        fs.Close();
    }
    
    public static void SafeDel(string path)
    {
        try
        {
            Debug.Log("remove " + path);
            File.Delete(path);
        }
        catch (Exception ex)
        {
            Debug.LogError("cannot del " + path + " " + ex);
        }
    }
}

// move class NativeResourcesUtility to NativeResourcesUtility.cs | add by liao
