using System;
using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static float FrameTime = 0.016666f; //基准帧率时间

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


        FirstStartGame();
    }

    private void FirstStartGame()
    {
        UILogo.Show(() =>
        {
            ShowTitle();
            UiManager.Show<UIFps>();
        });
    }

    public static void ShowTitle()
    {
        GamePause.DoContionueGame(EPauseFrom.Esc);
        CoroutineStart(ShowTitleImpl());
    }

    private static IEnumerator ShowTitleImpl()
    {  
        //加载Loading
        yield return UILoading.YieldShow();
        yield return Yielders.Frame;

        //卸载场景
        yield return Level.UnloadCurrentScene();
        yield return Yielders.Frame;

        //显示主界面
        UIMainView.Show(true);
        yield return new WaitForSeconds(0.3f);

        //关闭loading
        UILoading.Close();

    }

    public static void ClearCache()
    {

        //清理各种池
        //EffectFactory.BackAllToPool();

        UiMenuBase.ClearSelectStatus();

        //清理UI
        UiManager.Clear();

        //清理音效
        Sound.StopEnvironmentMusic();
        Sound.StopMusic();
        Sound.ClearSoundCache();

        //清理UI图集
        SpriteAtlasMgr.ClearCache();
    }

  
    void Update()
    {
        if (!_inited) return;
        TimeScaleManager.Update();
        SaveDataMgr.Update();
        UiManager.Update();
        
    }
    private void LateUpdate()
    {
        Sound.LateUpdate();
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

    void OnDestroy()
    {
        UiManager.Destory();
    }

}
