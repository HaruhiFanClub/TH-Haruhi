

public class UiList
{
    public static void RegisterUI()
    {
        _<UILogo>           ("UILogoAnimation.prefab",      UiLayer.Tips,   UiLoadType.Multi); 
        _<UIMainView>       ("UIMainView.prefab",           UiLayer.Main,   UiLoadType.Multi); 
        _<UIGameView>       ("UIGameView.prefab",           UiLayer.Main,   UiLoadType.Multi);
        _<UIChooseDifficult>("UIChooseDifficult.prefab",    UiLayer.Main,   UiLoadType.Multi);
        _<UIChoosePlayer>   ("UIChoosePlayer.prefab",       UiLayer.Main,   UiLoadType.Multi);

       // _<UILoading>        ("UILoading.prefab",            UiLayer.Loding, UiLoadType.Single);

        _<UiTips>           ("UiTips.prefab",               UiLayer.Tips,   UiLoadType.Multi);
        _<UIFps>            ("UIFps.prefab",                UiLayer.DontDestroy, UiLoadType.Multi); 

        //battle
        _<UIBattle>         ("UIBattle.prefab",             UiLayer.Battle, UiLoadType.Multi);
        _<UIPauseView>      ("UIPauseView.prefab",          UiLayer.Tips,   UiLoadType.Multi);
        _<UIDeadView>       ("UIDeadView.prefab",           UiLayer.Tips,   UiLoadType.Multi);
        _<UIBossBg>         ("UIBossBg.prefab",             UiLayer.Bg,     UiLoadType.Multi);

        _<UIDrawingChat>    ("UIDrawingChat.prefab",        UiLayer.Main,   UiLoadType.Multi);
        
        //debug
        _<UIDebugBulletLib> ("debug/DebugBulletLib.prefab", UiLayer.Tips,   UiLoadType.Multi);
    }

    private static void _<T>(string viewPath, UiLayer layer, UiLoadType loadType) where T : UiInstance
    {
        UiManager.R<T>("ui/prefabs/" + viewPath, layer, loadType);
    }
}
