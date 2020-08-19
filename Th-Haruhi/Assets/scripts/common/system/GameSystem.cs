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

        //读取存档
        SaveDataMgr.PreloadGameData();

        //初始化游戏设置
        GameSetting.InitSetting();

        //初始化sound模块
        SoundListenter.Init();

        //初始化资源管理器
        ResourceMgr.InitInstance();

        //初始化特效池
        EffectFactory.Init();


        CoroutineStart = StartCoroutine;
        DefaultRes = GetComponent<DefaultRes>();

        //初始化UI
        var uguiRoot = FindObjectOfType<UiRootScript>();
        uguiRoot.Init();
        UiManager.Init(uguiRoot);
        DontDestroyOnLoad(uguiRoot);
        DontDestroyOnLoad(gameObject);

        //清理缓存
        Caching.ClearCache();
        

        UnityEngine.Random.InitState(DateTime.Now.Second);

#if !UNITY_EDITOR && UNITY_ANDROID
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        _inited = true;


        StartGame();
    }

    private void StartGame()
    {
        UiManager.Show<UIFps>();
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
        PlayerDataMgr.SavePlayerData();
    }

    void OnApplicationFocus(bool focusStatus)
    {
    }
}
