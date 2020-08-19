using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
public class LoadedAssetBundle
{
    public AssetBundle AssetBundle;
    public int ReferencedCount;

    public LoadedAssetBundle(AssetBundle assetBundle)
    {
        AssetBundle = assetBundle;
        ReferencedCount = 1;
    }
}

// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
public class AssetBundleManager
{
    public static AssetBundle ShadersBundle;
    private static AssetBundleManifest _assetBundleManifest;

    private static readonly Dictionary<string, LoadedAssetBundle> LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();

    //private static readonly List<AssetBundleLoadOperation> InProgressOperations = new List<AssetBundleLoadOperation>();

    private static readonly Dictionary<string, string[]> Dependencies = new Dictionary<string, string[]>();


    // Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
    public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName)
    {
        LoadedAssetBundle bundle;
        LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
        if (bundle == null)
            return null;

        // No dependencies are recorded, only the bundle itself is required.
        string[] dependencies;
        if (!Dependencies.TryGetValue(assetBundleName, out dependencies))
            return bundle;

        // Make sure all dependencies are loaded
        foreach (var dependency in dependencies)
        {
            // Wait all the dependent assetBundles being loaded.
            LoadedAssetBundle dependentBundle;
            LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            if (dependentBundle == null)
                return null;
        }

        return bundle;
    }


    public static void Initialize()
    {
        string abName = "StreamingAssets.haruhi";
        if (_assetBundleManifest == null)
        {
            LoadedAssetBundles.Clear();
            var asset = LoadAsset(abName);
            _assetBundleManifest = asset.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            ShadersBundle = LoadAsset(PathUtility.ShaderBundleName);
            LoadedAssetBundles[PathUtility.ShaderBundleName] = new LoadedAssetBundle(ShadersBundle);
            Debug.Log("shader bundle load finished!" + ShadersBundle.name);
        }
    }


    // Load AssetBundle and its dependencies.
    protected static void LoadAssetBundle(string assetBundleName)
    {
        if (_assetBundleManifest == null)
        {
            Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            return;
        }


        // Check if the assetBundle has already been processed.
        if (!LoadAssetBundleInternal(assetBundleName))
            LoadDependencies(assetBundleName);
    }


    public static AssetBundle LoadAsset(string assetBundleName)
    {
        return AssetBundle.LoadFromFile(PathUtility.GetAbPath(assetBundleName));
    }

    protected static bool LoadAssetBundleInternal(string assetBundleName)
    {
        // Already loaded.
        LoadedAssetBundle bundle;
        LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
        if (bundle != null)
        {
            bundle.ReferencedCount++;
            return true;
        }


        string url = PathUtility.GetAbPath(assetBundleName); //PathUtility.GetDivPlatformNativeUrlPath(PathUtility.NativePath) + assetBundleName;
        var b = AssetBundle.LoadFromFile(url);
        LoadedAssetBundles[assetBundleName] = new LoadedAssetBundle(b);
        return false;
    }


    // Where we get all the dependencies and load them all.
    protected static void LoadDependencies(string assetBundleName)
    {
        if (_assetBundleManifest == null)
        {
            Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            return;
        }

        // Get dependecies from the AssetBundleManifest object..
        string[] dependencies = _assetBundleManifest.GetAllDependencies(assetBundleName);
        if (dependencies.Length == 0)
            return;

        // Record and load all dependencies.
        Dependencies.Add(assetBundleName, dependencies);
        for (int i = 0; i < dependencies.Length; i++)
            LoadAssetBundleInternal(dependencies[i]);
    }

    // Unload assetbundle and its dependencies.
    public static void UnloadAssetBundle(string assetBundleName, bool unloadDependencies = false)
    {
        UnloadDependencies(assetBundleName, unloadDependencies);
        UnloadAssetBundleInternal(assetBundleName);
    }

    protected static void UnloadDependencies(string assetBundleName, bool unloadDependencies)
    {
        string[] dependencies;
        if (!Dependencies.TryGetValue(assetBundleName, out dependencies))
            return;

        if (unloadDependencies)
        {
            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }
        }
        Dependencies.Remove(assetBundleName);
    }

    protected static void UnloadAssetBundleInternal(string assetBundleName)
    {
        LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName);
        if (bundle == null)
            return;
        // Debug.LogMemoryLog("try unload assetBundle, " + assetBundleName + " referencedCount:" + bundle.ReferencedCount);
        if (--bundle.ReferencedCount == 0)
        {
            //Debug.LogError("real unload bundle:"+ assetBundleName);
            bundle.AssetBundle.Unload(false);
            LoadedAssetBundles.Remove(assetBundleName);

            // Debug.LogMemoryLog(assetBundleName + " has been unloaded successfully");
        }
    }

    // Load asset from the given assetBundle.
    public static UnityEngine.Object LoadAsset(string assetBundleName, string assetName)
    {
        //Debug.LogError("Loading " + assetName + " from " + assetBundleName + " bundle");
        LoadAssetBundle(assetBundleName);
        LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName);
        var obj = bundle.AssetBundle.LoadAsset(PathUtility.GetAssetNamePath(assetName));
        return obj;
    }

    // Load level from the given assetBundle.
    public static IEnumerator LoadLevelAsync(string levelName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        string assetBundleName = PathUtility.GetAbName(levelName);
        LoadAssetBundle(assetBundleName);

        var sceneName = PathUtility.GetAssetNamePath(levelName);
        yield return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
    }
}
