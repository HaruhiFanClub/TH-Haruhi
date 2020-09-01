﻿
using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public static bool InLevel => ActiveLevel != null;

    public LevelDeploy Deploy { private set; get; }

    protected IEnumerator Init()
    {
        yield return Yielders.Frame;

        Sound.PlayMusic(Deploy.bgmId);

        yield return new WaitForSeconds(2f);

        StartCoroutine(LoopLevel());
    }

    private IEnumerator LoopLevel()
    {
        yield return new WaitForSeconds(3f);

        yield return Enemy.Create(2);
    }


    public virtual void OnEnter()
    {
        GameSystem.ClearCache();
    }

    public virtual void OnLeave()
    {
        GameSystem.ClearCache();
    }

    #region static
    public static Level ActiveLevel { get; protected set; }
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
            typeScene = typeof(Level);
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

    public static IEnumerator Load(int levelId, Action<Level> finishAction = null)
    {
        var deploy = TableUtility.GetDeploy<LevelDeploy>(levelId);
        if (string.IsNullOrEmpty(deploy.resource))
        {
            if (finishAction != null) finishAction.Invoke(null);
            Debug.LogError(string.Format("sceneId url : {0} not exist", deploy.resource));
            yield break;
        }

        yield return LoadImpl(deploy, finishAction);
    }

    private static IEnumerator LoadImpl(LevelDeploy deploy, Action<Level> finishAction)
    {
        Level gameScene = null;

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
        var scene = sceneObject.AddComponent(typeScene) as Level;
        scene.Deploy = deploy;
        yield return scene.Init();
        finishAction?.Invoke(gameScene);
    }

    private static void SceneEnter(Level gameScene)
    {
        if (ActiveLevel != null)
            ActiveLevel.OnDestroy();

        ActiveLevel = gameScene;
    }

    public void Awake()
    {
        SceneEnter(this);
        OnEnter();
    }

    public void OnDestroy()
    {
        OnLeave();
        ActiveLevel = null;
    }
    #endregion
}


public class LevelDeploy : Conditionable
{
    public int id;
    public string name;
    public string resource;
    public int bgmId;
    public string levelClass = "Level";
}