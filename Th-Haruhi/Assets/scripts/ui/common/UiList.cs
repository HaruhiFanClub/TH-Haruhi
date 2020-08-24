

public class UiList
{
    public static void RegisterUI()
    {
        _<UILogo>           ("UILogoAnimation.prefab",      UiLayer.Tips,   UiLoadType.Multi); 
        _<UIMainView>       ("UIMainView.prefab",           UiLayer.Main,   UiLoadType.Multi); 
        _<UIGameView>       ("UIGameView.prefab",           UiLayer.Main,   UiLoadType.Multi);
        _<UIChooseDiffult>  ("UIChooseDifficult.prefab",    UiLayer.Main,   UiLoadType.Single);
        _<UISelectPlayer>   ("UISelectPlayer.prefab",       UiLayer.Main,   UiLoadType.Single);

        _<UILoading>        ("UILoading.prefab",            UiLayer.Loding, UiLoadType.Single);

        _<UiTips>           ("UiTips.prefab",               UiLayer.Tips,   UiLoadType.Multi);
        _<UIFps>            ("UIFps.prefab",                UiLayer.Loding, UiLoadType.Multi); 
    }

    private static void _<T>(string viewPath, UiLayer layer, UiLoadType loadType) where T : UiInstance
    {
        UiManager.R<T>("ui/prefabs/" + viewPath, layer, loadType);
    }
}
