

using System.Collections;

public static class GameWorld
{
    public static void ShowTitle()
    {
        UIMainView.Show(true);
    }

    private static IEnumerator ShowTitleImpl()
    {
        yield return Level.UnloadCurrentScene();
    }

    public static void EnterLevel(int levelId)
    {
        //显示loading
        UILoading.Show(()=>
        {
            Level.Load(levelId, finishAction: (scene) =>
            {
                //关闭loading
                UILoading.Close();
            });
        });
    }

    public static void Update()
    {
       
    }

    public static void ClearCache()
    {

        //清理各种池
        //EffectFactory.BackAllToPool();

        //清理UI
        UiManager.Clear();    
        
        //清理音效
        Sound.StopEnvironmentMusic();
        Sound.StopMusic();
        Sound.ClearSoundCache();

        //清理UI图集
        SpriteAtlasMgr.ClearCache();
    }
}
