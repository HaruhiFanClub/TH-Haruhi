using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


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
    public static bool Inited;
    private static AssetBundleManifest _assetBundleManifest;

    private static readonly Dictionary<string, LoadedAssetBundle> LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
    private static readonly Dictionary<string, bool> Loadings = new Dictionary<string, bool>();
    private static readonly Dictionary<string, string[]> Dependencies = new Dictionary<string, string[]>();


    // Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
    private static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName)
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


    //初始化assetbundle
    public static IEnumerator Initialize()
    {
#if UNITY_EDITOR && !STANDALONE_BUNDLE
        Inited = true;
        yield break;
#else
        string abName = "StreamingAssets.haruhi";
        if (_assetBundleManifest == null)
        {
            LoadedAssetBundles.Clear();

            var maniAsync = new AsyncResource();
            yield return RequsetAssetBundle(abName, maniAsync);

            _assetBundleManifest = maniAsync.AssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            var shaderAsync = new AsyncResource();
            yield return RequsetAssetBundle(PathUtility.ShaderBundleName, shaderAsync);

            ShadersBundle = shaderAsync.AssetBundle;
            LoadedAssetBundles[PathUtility.ShaderBundleName] = new LoadedAssetBundle(ShadersBundle);
            Debug.Log("shader bundle load finished!" + ShadersBundle.name);

            yield return 0;
            Inited = true;
        }
#endif

    }


    // Load AssetBundle and its dependencies.
    private static IEnumerator LoadAssetBundle(string assetBundleName)
    {
        if (_assetBundleManifest == null)
        {
            Debug.LogError("Assetbundle未初始化，是否未调用 AssetBundleManager.Initialize()");
            yield break;
        }

        // Check if the assetBundle has already been processed.
        var needLoadDepend = false;

        yield return LoadAssetBundleInternal(assetBundleName, b =>
        {
            needLoadDepend = b;
        });

        if (needLoadDepend)
        {
            yield return LoadDependencies(assetBundleName);
        }
    }


    private static IEnumerator RequsetAssetBundle(string assetBundleName, AsyncResource async)
    {
        var url = PathUtility.GetAbPath(assetBundleName);

        //webgl要用request加载
        if(Platform.Plat == EPlatform.UNITY_WEBGL)
        {
            var request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerAssetBundle(url, 0);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError("LoadAsset Error:" + request.error + " url:" + url);
                yield break;
            }

            var asset = DownloadHandlerAssetBundle.GetContent(request);
            async.AssetBundle = asset;
        }
        //从本地加载
        else
        {   
            var asset = AssetBundle.LoadFromFile(url);
            async.AssetBundle = asset;
        }
    }

    private static IEnumerator LoadAssetBundleInternal(string assetBundleName, Action<bool> callBack = null)
    {
        // Already loaded.
        LoadedAssetBundles.TryGetValue(assetBundleName, out var bundle);
        if (bundle != null)
        {
            bundle.ReferencedCount++;
            callBack?.Invoke(false);
            yield break;
        }

        if(Loadings.TryGetValue(assetBundleName, out var inLoading))
        {
            if(inLoading)
            {
                yield break;
            }
        }

        Loadings[assetBundleName] = true;

        var async = new AsyncResource();
        yield return RequsetAssetBundle(assetBundleName, async);

        Loadings[assetBundleName] = false;
        LoadedAssetBundles[assetBundleName] = new LoadedAssetBundle(async.AssetBundle);
        callBack?.Invoke(true);
    }


    // Where we get all the dependencies and load them all.
    protected static IEnumerator LoadDependencies(string assetBundleName)
    {
        if (_assetBundleManifest == null)
        {
            Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
            yield break;
        }

        // Get dependecies from the AssetBundleManifest object..
        string[] dependencies = _assetBundleManifest.GetAllDependencies(assetBundleName);
        if (dependencies.Length == 0)
        {
            yield break;
        }

        // Record and load all dependencies.
        Dependencies.Add(assetBundleName, dependencies);
        for (int i = 0; i < dependencies.Length; i++)
        {
            yield return LoadAssetBundleInternal(dependencies[i]);
        }
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
    public static IEnumerator LoadAsset(string assetBundleName, string assetName, AsyncResource async)
    {
        //Debug.LogError("Loading " + assetName + " from " + assetBundleName + " bundle");

        yield return LoadAssetBundle(assetBundleName);

        LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName);
        var obj = bundle.AssetBundle.LoadAsset(PathUtility.GetAssetNamePath(assetName));
        async.Object = obj;
    }

    // Load level from the given assetBundle.
    public static IEnumerator LoadLevelAsync(string levelName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        string assetBundleName = PathUtility.GetAbName(levelName);
        yield return LoadAssetBundle(assetBundleName);

        var sceneName = PathUtility.GetAssetNamePath(levelName);
        yield return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
    }
}
