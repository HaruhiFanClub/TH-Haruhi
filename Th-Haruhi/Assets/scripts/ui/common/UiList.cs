

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
        _<UIPauseView>      ("UIPauseView.prefab",       UiLayer.Main,   UiLoadType.Multi);
        _<UIDeadView>       ("UIDeadView.prefab",        UiLayer.Main,   UiLoadType.Multi);
    }

    private static void _<T>(string viewPath, UiLayer layer, UiLoadType loadType) where T : UiInstance
    {
        UiManager.R<T>("ui/prefabs/" + viewPath, layer, loadType);
    }
}
