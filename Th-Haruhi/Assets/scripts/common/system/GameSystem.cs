using System;
using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance;
    private static bool _inited;

    public delegate Coroutine StartCoroutineFunc(IEnumerator routine);

    public static StartCoroutineFunc CoroutineStart;

    public static DefaultRes DefaultRes;

    private void Awake()
    {
        if (_inited)
        {
            return;
        }

        Instance = this;

#if !UNITY_EDITOR
	    AssetBundleManager.Initialize();
#endif

        var spriteMgr = Resources.FindObjectsOfTypeAll<SpriteAtlasMgr>();
        if (spriteMgr.Length == 0)
        {
            gameObject.AddComponent<SpriteAtlasMgr>();
        }

        SaveDataMgr.PreloadGameData();
        GameSetting.InitSetting();
        SoundListenter.Init();
        ResourceMgr.InitInstance();
        EffectFactory.Init();
        GamePause.Init();

        CoroutineStart = StartCoroutine;
        DefaultRes = GetComponent<DefaultRes>();

        var uguiRoot = FindObjectOfType<UiRootScript>();
        uguiRoot.Init();
        UiManager.Init(uguiRoot);
        DontDestroyOnLoad(uguiRoot);
        DontDestroyOnLoad(gameObject);

        Caching.ClearCache();
        

        UnityEngine.Random.InitState(DateTime.Now.Second);

#if !UNITY_EDITOR && UNITY_ANDROID
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        _inited = true;

        GameWorld.ShowTitle();
    }

    void OnDestroy()
    {
        UiManager.Destory();
    }

    void Update()
    {
        if (!_inited) return;
        TimeScaleManager.Update();
        SaveDataMgr.Update();
        UiManager.Update();
        GameWorld.Update();
    }

    public static bool PauseStatus { private set; get; }

    void OnApplicationPause(bool pauseStatus)
    {
        PauseStatus = pauseStatus;
    }

    void OnApplicationQuit()
    {
        PlayerDataMgr.Data = PlayerDataMgr.Data;
    }

    void OnApplicationFocus(bool focusStatus)
    {
    }
}
