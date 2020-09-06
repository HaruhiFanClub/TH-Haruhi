


using System;
using UnityEngine;
using System.IO;

public class PathUtility
{
    public static readonly string AssetsPath;
    public static readonly string ProjectPath;
    public static readonly string ResourcesPath;
    public static readonly string AssetBundlePath;
    private static readonly string ResourcesPathToProjectPath;


    public static readonly string ImagesPath;
    public static readonly string BulletConfigPath;
    public static readonly string SaveDataPath;
    public static readonly string CommonStoragePath;

    private static readonly string ImagesName = "/images/";
    private static readonly string BulletConfigName = "/userdata/bullets/";
    private static readonly string SaveDataName = "/userdata/save/";
    private static readonly string CommonStorageName = "/userdata/storage/";

    public static string TablePath
    {
        get
        {
#if UNITY_EDITOR
            return AssetsPath + "/tables/";
#else
            return AssetBundlePath + "/tables/";
#endif
        }
    }
    static PathUtility()
    {
        AssetsPath = FormatPath(Application.dataPath);
        ResourcesPath = AssetsPath + "/res";
        AssetBundlePath = FormatPath(Application.streamingAssetsPath).Replace("StreamingAssets", "streamingassets"); ;

        var serializablePath = FormatPath(Application.persistentDataPath);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        BulletConfigPath = AssetsPath + BulletConfigName;
        SaveDataPath = AssetsPath + SaveDataName;
        CommonStoragePath = AssetsPath + CommonStorageName;
        ImagesPath = AssetsPath + ImagesName;
#else
        BulletConfigPath = serializablePath + BulletConfigName;
        SaveDataPath = serializablePath + SaveDataName;
        CommonStoragePath = serializablePath + CommonStorageName;
        ImagesPath = serializablePath + ImagesName;
#endif
        if (!Directory.Exists(ImagesPath))
            Directory.CreateDirectory(ImagesPath);

        if (!Directory.Exists(BulletConfigPath))
            Directory.CreateDirectory(BulletConfigPath);

        if (!Directory.Exists(SaveDataPath))
            Directory.CreateDirectory(SaveDataPath);

        if (!Directory.Exists(CommonStoragePath))
            Directory.CreateDirectory(CommonStoragePath);

        int p = AssetsPath.LastIndexOf('/');
        ProjectPath = p != -1 ? AssetsPath.Remove(p) : AssetsPath;
        ResourcesPathToProjectPath = ResourcesPath.Remove(0, ProjectPath.Length + 1);
    }

    public static string FullPathToProjectPath(string path)
    {
        path = FormatPath(path);
        int p = path.IndexOf(ProjectPath, StringComparison.Ordinal);
        int t = ProjectPath.Length;
        if (p != -1 && path.Length > t && (path[t] == '/'))
            return path.Remove(0, ProjectPath.Length + 1);
        return path;
    }

    public static string FullPathToResourcePath(string path)
    {
        path = FormatPath(path);
        int p = path.IndexOf(ResourcesPath, StringComparison.Ordinal);
        int t = ResourcesPath.Length;
        if (p != -1 && path.Length > t && (path[t] == '/'))
            return path.Remove(0, ResourcesPath.Length + 1);
        return path;
    }

    public static string ProjectPathToFullPath(string path)
    {
        return ProjectPath + "/" + FormatPath(path);
    }

    public static string ResourcesPathToFullPath(string path)
    {
        return ResourcesPath + "/" + FormatPath(path);
    }

    public static string ProjectPathToResourcesPath(string path)
    {
        path = FormatPath(path);
        int len = "Assets/res".Length;
        return path.StartsWith("Assets/res") ? path.Remove(0, len + 1) : path;
    }
    public static string ProjectPathToTablePath(string path)
    {
        path = FormatPath(path);
        int len = "Assets/tables".Length;
        return path.StartsWith("Assets/tables") ? path.Remove(0, len + 1) : path;
    }
    public static string GetUniqueTablePathByProjectPath(string path)
    {
        return GetUniquePath(ProjectPathToTablePath(path));
    }

    public static string GetResourcesPathToProjectPath(string path)
    {
        path = FormatPath(path);
        if (!path.StartsWith(ResourcesPathToProjectPath) && path[0] != '/')
            return ResourcesPathToProjectPath + '/' + FormatPath(path);
        return path;
    }

    public static string FormatPath(string path)
    {
        return path.Replace('\\', '/');
    }

    public static string GetUniquePath(string path)
    {
        return FormatPath(path).Replace('/', '_').Replace(" ", "");
    }

    public static string GetUniquePathByProjectPath(string path)
    {
        return GetUniquePath(ProjectPathToResourcesPath(path));
    }

    private static readonly string[] CustomPath =
    {
        
    };

    private static string CheckCustomPath(string path)
    {
        for (int i = 0; path != null && i < CustomPath.Length; i++)
        {
            if (path.Contains(CustomPath[i]))
            {
                return CustomPath[i].Replace("/", "_").Replace(".", "_") + ".haruhi";
            }
        }
        return null;
    }

    public const string ShaderBundleName = "shaders.haruhi";
    public static string GetAbName(string path)
    {
        ResourceType type = ResourcesUtility.GetResourceTypeByPath(path);

        //所有shader和material放到一个assetbundle

        if (type == ResourceType.shader || type == ResourceType.shadervariants)
        {
            return ShaderBundleName;
        }

        //检查是否是按文件夹自定义的assetbundleName
        if (type == ResourceType.scene)
        {
            var strAbName = ProjectPathToResourcesPath(path).Replace("/", "_").Replace(".", "_").Replace(" ", "_") + ".haruhi";
            return strAbName.ToLower();
        }
        //检查是否是按文件夹自定义的assetbundleName
        string abName = CheckCustomPath(path);
        if (abName != null)
        {
            return abName.ToLower();
        }


        //默认的情况，则按照目录名命名assetbundleName
        path = ProjectPathToResourcesPath(path);
        var items = path.Split('/');
        path = path.Replace(items[items.Length - 1], "").Replace("/", "_") + ".haruhi";
        if (CheckAbNameLength(path))
        {
            return path.ToLower();
        }
        throw new Exception("he assetbundle name is too long bigger than 100");
    }

    private static bool CheckAbNameLength(string path)
    {
        if (path.Length >= 100)
        {
            Debug.LogError("the assetbundle name is too long bigger than 100 the path is " + path);

            return true;
        }
        return true;
    }

    public static string GetAssetNamePath(string assetName)
    {
        string path = assetName;
        if (assetName != "AssetBundleManifest")
            path = "Assets/res/" + assetName;
        return path;
    }

    public static string GetAbPath(string resource)
    {
        return Path.Combine(AssetBundlePath, resource);
    }
    public static string GetDivPlatformNativeUrlPath(string _path)
    {
#if UNITY_ANDROID 
        return _path;
#elif UNITY_IOS
        return  "file://" + _path;
#else
        return _path;
#endif
    }
}

