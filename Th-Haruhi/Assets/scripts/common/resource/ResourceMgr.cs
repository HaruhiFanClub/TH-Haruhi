
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    public class LoadInfo : IPool
    {
        public string Resource;
        public Action<Object> Notify;
        public bool IsDone;
        public override void Init() { }
        public override void Reset()
        {
            Resource = "";
            Notify = null;
            IsDone = false;
        }

        public override void Recycle() { }

        public override void OnDestroy() { }
    }

    public class InstantiateInfo : IPool
    {
        public Object Source;
        public Action<GameObject> CallBack;
        public override void Init() { }

        public override void Reset() { }

        public override void Recycle()
        {
            Source = null;
            CallBack = null;
        }

        public override void OnDestroy()
        {

        }
    }

    private static List<InstantiateInfo> instantiateList = new List<InstantiateInfo>();
    public static void InstantiateX(Object _object, Action<GameObject> callBack)
    {
        if (_object == null)
        {
            Debug.LogError("Want InstantiateX SourceObject Is Null!");
            return;
        }
        var info = Pool.New<InstantiateInfo>() as InstantiateInfo;
        info.Source = _object;
        info.CallBack = callBack;
        instantiateList.Add(info);
    }


    public new static GameObject Instantiate(Object _object)
    {
        GameObject result = Object.Instantiate(_object, GameScene.Root) as GameObject;
        return result;
    }

    private void UpdateInstantiate()
    {
        if (instantiateList.Count <= 0) return;

        var info = instantiateList[0];
        instantiateList.RemoveAt(0);
        if (info.Source == null)
        {
            Pool.Free(info);
            return;
        }
        GameObject result = Object.Instantiate(info.Source, GameScene.Root) as GameObject;
        if (result == null)
        {
            Debug.LogError("InstantiateGameObjectError, Source:" + info.Source);
        }
        else
        {
            info.CallBack(result);
        }
        Pool.Free(info);
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
                Pool.Free(LoadingList[0]);
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
        var obj = LoadImmediately(info.Resource);
        info.IsDone = true;
        info.Notify(obj);
    }

    public static Object LoadImmediately(string resource)
    {
        Object loadObject = null;
#if UNITY_EDITOR
        loadObject = LoadImpl(resource);
#else
        loadObject = LoadAsset(resource);
#endif
        if (loadObject == null)
        {
            Debug.LogError("加载的资源失败 resource:" + resource);
        }
        return loadObject;
    }

    public static void Load(string resource, Action<Object> notify = null)
    {
	    if (string.IsNullOrEmpty(resource))
	    {
            Debug.LogError("resource path is null");
		    return;
	    }
        var loadInfo = Pool.New<LoadInfo>() as LoadInfo;
        loadInfo.Resource = resource;
        loadInfo.Notify = notify;
        loadInfo.IsDone = false;
        ToLoadList.Add(loadInfo);
    }

    public static Object LoadAsset(string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        string assetBundleName = PathUtility.GetAbName(assetName);

        //shader特殊处理
        if (assetBundleName == PathUtility.ShaderBundleName)
        {
            var shaderObj = AssetBundleManager.ShadersBundle.LoadAsset(assetName);
            return shaderObj;
        }

        var obj = AssetBundleManager.LoadAsset(assetBundleName, assetName);
        if (obj == null)
        {
            Debug.LogError("LoadAsset request.asset Object == null, assetBundleName:" + assetBundleName + " assetName:" + assetName);
        }

        return obj;
    }

    public static IEnumerator LoadScene(string levelName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        yield return AssetBundleManager.LoadLevelAsync(levelName, mode);
    }


#if UNITY_EDITOR
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


    public static byte[] GetResourceData(string resource)
    {
        //Debug.Log("the !!!!!!!!!GetResourceData the resource is " + resource);
        byte[] data = null;

#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            resource = PathUtility.GetUniquePath(resource);
            using (ZipFile zip = new ZipFile(PathUtility.AssetsPath))
                if (zip != null)
                {
                    ZipEntry entry = zip.GetEntry("assets/" + resource + ".haruhi");
                    if (entry != null)
                        using (Stream stm = zip.GetInputStream(entry))
                            if (stm != null)
                            {
                                MemoryStream mem = new MemoryStream();
                                const int bufsz = 4096;
                                byte[] buf = new byte[bufsz];

                                while (true)
                                {
                                    int readsz = stm.Read(buf, 0, bufsz);
                                    if (readsz > 0)
                                        mem.Write(buf, 0, readsz);
                                    if (readsz < bufsz)
                                        break;
                                }
                                data = mem.ToArray();
                            }
                }
        }
        catch (System.Exception)
        {
            Debug.Log("GetResourceData the resource is " + resource);
        }			
#else
#if UNITY_EDITOR
        string path = PathUtility.NativePath + resource;
#else
        string path = PathUtility.NativePath + PathUtility.GetUniquePath(resource) + ".haruhi";
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

#endif
        return data;
    }


    public static string GetResourceText(string resource)
    {
#pragma warning disable
        Encoding encoding = null;
#pragma warning restore

        byte[] buffer = GetResourceData(resource);
        if (buffer != null)
        {
            buffer = FileUtility.CopyTo(buffer);	//尝试解密
            Encoding encode = Encoding.GetEncoding("GBK");
            if (buffer.Length >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                encode = Encoding.UTF8;
            else if (buffer.Length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
                encode = Encoding.Unicode;
            byte[] result = Encoding.Convert(encode, Encoding.UTF8, buffer);
            string text = "";
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