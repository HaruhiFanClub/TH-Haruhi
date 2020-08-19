
using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    public static GameScene ActiveScene { get; protected set; }
    public static string CurrentSceneName;

    public static Transform Root {
        get
        {
            if (ActiveScene == null)
                return null;
            else
                return ActiveScene.transform;
        }
    }

    #region staticMethon

    void Awake()
    {
        SceneEnter(this);
        OnEnter();
    }

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
            typeScene = typeof(GameScene);
        }

        return typeScene;
    }

    public static IEnumerator UnloadCurrentScene()
    {
        if (!string.IsNullOrEmpty(CurrentSceneName))
        {
            var async = SceneManager.UnloadSceneAsync(CurrentSceneName);
            CurrentSceneName = null;
            if(!async.isDone)
            {
                yield return 0;
            }
        }
    }

    public static void Load(string sceneUrl,  Action<GameScene> finishAction = null, string sceneClass = "GameScene")
    {
        if (string.IsNullOrEmpty(sceneUrl))
        {
            if (finishAction != null) finishAction.Invoke(null);
            Debug.LogError(string.Format("sceneId url : {0} not exist", sceneUrl));
            return;
        }
        
        GameSystem.CoroutineStart(LoadImpl(sceneUrl, finishAction, sceneClass));
    }

    private static IEnumerator LoadImpl(string sceneUrl, Action<GameScene> finishAction, string sceneClass)
    {
        GameScene gameScene = null;

        yield return UnloadCurrentScene();

        var sceneName = Path.GetFileNameWithoutExtension(sceneUrl);

#if UNITY_EDITOR
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
#else
        yield return ResourceMgr.LoadScene(sceneUrl, LoadSceneMode.Additive);
#endif
        CurrentSceneName = sceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        GameEventCenter.Send(GameEvent.OnSceneChange);

        //挂脚本
        var typeScene = GetSceneType(sceneClass);
        var sceneObject = new GameObject("GameScene");
        var scene = sceneObject.AddComponent(typeScene) as GameScene;
        finishAction?.Invoke(gameScene);
    }

    private static void SceneEnter(GameScene gameScene)
    {
        if (ActiveScene != null)
            ActiveScene.OnDestroy();

        ActiveScene = gameScene;
    }

    #endregion

    public void OnDestroy()
    {
        OnLeave();
        ActiveScene = null;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnLeave()
    {
    }
}