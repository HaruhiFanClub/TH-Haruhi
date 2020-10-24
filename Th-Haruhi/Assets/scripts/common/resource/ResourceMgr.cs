
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class AsyncResource
{
    public Object Object;
    public AssetBundle AssetBundle;
}

public class ResourceMgr : MonoBehaviour
{
    private static ResourceMgr _instance;

    public static void InitInstance()   
    {
        if (_instance != null) return;
        var gameObj = new GameObject("ResourceMgr");
        DontDestroyOnLoad(gameObj);
        _instance = gameObj.AddComponent<ResourceMgr>();
    }

    private class LoadInfo
    {
        public string Resource;
        public Action<Object> Notify;
        public bool IsDone;
    }

    private class InstantiateInfo
    {
        public Object Source;
        public Action<GameObject> CallBack;
     
    }

    private static List<InstantiateInfo> instantiateList = new List<InstantiateInfo>();
    public static void InstantiateX(Object _object, Action<GameObject> callBack)
    {
        if (_object == null)
        {
            Debug.LogError("Want InstantiateX SourceObject Is Null!");
            return;
        }
        var info = new InstantiateInfo();
        info.Source = _object;
        info.CallBack = callBack;
        instantiateList.Add(info);
    }


    public new static GameObject Instantiate(Object _object)
    {
        GameObject result = Object.Instantiate(_object, null) as GameObject;
        return result;
    }

    private void UpdateInstantiate()
    {
        if (instantiateList.Count <= 0) return;

        var info = instantiateList[0];
        instantiateList.RemoveAt(0);
        if (info.Source == null)
        {
            return;
        }

        GameObject result = Object.Instantiate(info.Source, null) as GameObject;
        if (result == null)
        {
            Debug.LogError("InstantiateGameObjectError, Source:" + info.Source);
        }
        else
        {
            info.CallBack(result);
        }
    }

    private static readonly List<LoadInfo> LoadingList = new List<LoadInfo>();
    private static readonly List<LoadInfo> ToLoadList = new List<LoadInfo>();
    void Update()
    {
        UpdateInstantiate();
        UpdateLoad();
    }

    private void UpdateLoad()
    {
        if (LoadingList.Count > 0)
        {
            if (LoadingList[0].IsDone)
            {
                LoadingList.RemoveAt(0);
            }
            return;
        }

        if (ToLoadList.Count <= 0)
        {
            return;
        }

        var info = ToLoadList[0];
        ToLoadList.RemoveAt(0);
        LoadingList.Add(info);

        LoadObject(info.Resource, obj =>
        {
            info.IsDone = true;
            info.Notify(obj);
        });
    }

    public static void LoadObject(string resource, Action<Object> callBack)
    {
        _instance.StartCoroutine(LoadObjectAsync(resource, callBack));
    }

    private static IEnumerator LoadObjectAsync(string resource, Action<Object> callBack)
    {
        var async = new AsyncResource();
        yield return LoadObjectWait(resource, async);
        callBack(async.Object);
    }

    public static IEnumerator LoadObjectWait(string resource, AsyncResource async)
    {

#if UNITY_EDITOR && !STANDALONE_BUNDLE
        async.Object = LoadImpl(resource);
        yield break;
#else
        yield return LoadAssetBundleObject(resource, async);
#endif
    }

    public static void Load(string resource, Action<Object> notify = null)
    {
	    if (string.IsNullOrEmpty(resource))
	    {
            Debug.LogError("resource path is null");
		    return;
	    }
        var loadInfo = new LoadInfo();
        loadInfo.Resource = resource;
        loadInfo.Notify = notify;
        loadInfo.IsDone = false;
        ToLoadList.Add(loadInfo);
    }

    private static IEnumerator LoadAssetBundleObject(string assetName, AsyncResource async)
    {
        string assetBundleName = PathUtility.GetAbName(assetName);

        //shader特殊处理
        if (assetBundleName == PathUtility.ShaderBundleName)
        {
            var shaderObj = AssetBundleManager.ShadersBundle.LoadAsset(assetName);
            async.Object = shaderObj;
            yield break;
        }

        yield return AssetBundleManager.LoadAsset(assetBundleName, assetName, async);

        if (async.Object == null)
        {
            Debug.LogError("LoadAsset request.asset Object == null, assetBundleName:" + assetBundleName + " assetName:" + assetName);
        }
    }

    public static IEnumerator LoadScene(string levelName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        yield return AssetBundleManager.LoadLevelAsync(levelName, mode);
    }


#if UNITY_EDITOR && !STANDALONE_BUNDLE
    private static Object LoadImpl(string resource)
    {
        Object mainObject = null;
        if (!string.IsNullOrEmpty(resource))
        {
            mainObject = UnityEditor.AssetDatabase.LoadAssetAtPath(PathUtility.GetResourcesPathToProjectPath(resource), ResourcesUtility.TypeOfResource(resource));
            if (!mainObject)
                Debug.LogError(string.Format("load : {0} faild", resource));
        }
        return mainObject;
    }
#endif


    public static byte[] GetTableData(string resource)
    {
        byte[] data = null;
#if UNITY_EDITOR
        string path = PathUtility.TablePath + resource;
#else
        string path = PathUtility.TablePath + PathUtility.GetUniquePath(resource) + ".haruhi_table";
#endif
        if (File.Exists(path))
        {
            try
            {
                data = FileUtility.ReadAllBytes(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError("GetResourceData the resource is " + resource);
                Debug.LogError(e);
            }
            if (data != null)
            {
                return data;
            }
        }
        return data;
    }


    public static string GetResourceText(string resource)
    {
#pragma warning disable
        Encoding encoding = null;
#pragma warning restore

        byte[] buffer = GetTableData(resource);
        if (buffer != null)
        {
            //buffer = FileUtility.CopyTo(buffer);	//尝试解密
            Encoding encode = Encoding.GetEncoding("GBK");
            if (buffer.Length >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                encode = Encoding.UTF8;
            else if (buffer.Length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
                encode = Encoding.Unicode;
            byte[] result = Encoding.Convert(encode, Encoding.UTF8, buffer);
            string text;
            if (result.Length >= 3 && result[0] == 0xEF && result[1] == 0xBB && result[2] == 0xBF)
                text = Encoding.UTF8.GetString(result, 3, result.Length - 3);
            else
                text = Encoding.UTF8.GetString(result);
            return text;
        }
        return "";
    }

    public static void ManualGC()
    {
        Debug.Log("## start ManualGC!");
        if(_instance) _instance.StartCoroutine(DoGc());
    }

    private static IEnumerator DoGc()
    {
        var request = Resources.UnloadUnusedAssets();
        while (!request.isDone)
        {
            yield return Yielders.Frame;
        }
        GC.Collect();

        Debug.Log("## ManualGC Over!");
    }

}