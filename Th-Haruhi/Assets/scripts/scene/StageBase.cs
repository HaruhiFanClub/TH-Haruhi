
using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public abstract class StageBase : MonoBehaviour
{
    public static bool InLevel => ActiveLevel != null;

    public StageDeploy Deploy { private set; get; }

    protected IEnumerator Init()
    {
        yield return Yielders.Frame;

        Sound.PlayMusic(Deploy.bgmId);

        yield return new WaitForSeconds(2f);

        StartCoroutine(LoopLevel());
    }

    protected abstract IEnumerator LoopLevel();

    #region static
    public static StageBase ActiveLevel { get; protected set; }
    public static string CurrentSceneName;

    private static Type GetSceneType(string sceneClass)
    {
        Type typeScene = null;
        if (!string.IsNullOrEmpty(sceneClass))
        {
            typeScene = Type.GetType(sceneClass);
            if (typeScene == null)
            {
                Debug.LogError(string.Format("scene class : {0} not exist", sceneClass));
            }
        }

        if (typeScene == null)
        {
            typeScene = typeof(StageBase);
        }

        return typeScene;
    }

    public static IEnumerator UnloadCurrentScene()
    {
        if (!string.IsNullOrEmpty(CurrentSceneName))
        {
            var async = SceneManager.UnloadSceneAsync(CurrentSceneName);
            CurrentSceneName = null;
            if (!async.isDone)
            {
                yield return 0;
            }
        }
    }

    public static IEnumerator Load(int levelId, Action<StageBase> finishAction = null)
    {
        var deploy = TableUtility.GetDeploy<StageDeploy>(levelId);
        if (string.IsNullOrEmpty(deploy.resource))
        {
            if (finishAction != null) finishAction.Invoke(null);
            Debug.LogError(string.Format("sceneId url : {0} not exist", deploy.resource));
            yield break;
        }

        yield return LoadImpl(deploy, finishAction);
    }

    private static IEnumerator LoadImpl(StageDeploy deploy, Action<StageBase> finishAction)
    {
        StageBase gameScene = null;

        yield return UnloadCurrentScene();

        var sceneName = Path.GetFileNameWithoutExtension(deploy.resource);

#if UNITY_EDITOR
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
#else
        yield return ResourceMgr.LoadScene(deploy.resource, LoadSceneMode.Additive);
#endif
        CurrentSceneName = sceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        GameEventCenter.Send(GameEvent.OnSceneChange);

        //挂脚本
        var typeScene = GetSceneType(deploy.levelClass);
        var sceneObject = new GameObject("GameScene");
        var scene = sceneObject.AddComponent(typeScene) as StageBase;
        scene.Deploy = deploy;
        yield return scene.Init();
        finishAction?.Invoke(gameScene);
    }

    private static void SceneEnter(StageBase gameScene)
    {
        if (ActiveLevel != null)
            ActiveLevel.OnDestroy();

        ActiveLevel = gameScene;
    }

    protected virtual void Awake()
    {
        SceneEnter(this);
        GameSystem.ClearCache();
        UiManager.BackGround.SetActiveSafe(false);
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
        GameSystem.ClearCache();
        UiManager.BackGround.SetActiveSafe(true);
        ActiveLevel = null;
    }
    #endregion
}


public class StageDeploy : Conditionable
{
    public int id;
    public string name;
    public string resource;
    public int bgmId;
    public string levelClass;
}