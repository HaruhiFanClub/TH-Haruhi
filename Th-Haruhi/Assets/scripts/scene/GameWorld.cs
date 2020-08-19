

using System.Collections;
using System.Collections.Generic;

public static class GameWorld
{
    public static bool InGameing = false;

    public static void ShowTitle()
    {
        ClearCache();
        UIMainView.Show(true);

    }

    private static IEnumerator ShowTitleImpl()
    {
        yield return GameScene.UnloadCurrentScene();
    }

    public static void EnterScene(string sceneUrl, string bornId = "1")
    {
        GameScene.Load(sceneUrl, finishAction: (scene) =>
        {
            UIGameView.Show();

            //创建角色
            CharacterMgr.MainPlayer.transform.position = RegionRoot.FindBornRegionById(bornId).transform.position;
        });
    }

    public static void Update()
    {
       
    }

    private static void ClearCache()
    {
        //删除角色
        CharacterMgr.Clear();

        //清理各种池
        EffectFactory.BackAllToPool();

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
