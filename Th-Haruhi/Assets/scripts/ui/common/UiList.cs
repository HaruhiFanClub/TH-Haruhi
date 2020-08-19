

public class UiList
{
    public static void RegisterUI()
    {
        _<UILogo>    ("UILogoAnimation.prefab",    UiLayer.Tips, UiLoadType.Multi); 
        _<UIMainView>("UIMainView.prefab",  UiLayer.Main, UiLoadType.Multi); 
        _<UIGameView>("UIGameView.prefab",  UiLayer.Main, UiLoadType.Multi);

        
        _<UiTips>("UiTips.prefab", UiLayer.Tips, UiLoadType.Multi);
        _<UIFps>("UIFps.prefab",    UiLayer.Tips, UiLoadType.Multi); 
    }

    private static void _<T>(string viewPath, UiLayer layer, UiLoadType loadType) where T : UiInstance
    {
        UiManager.R<T>("ui/prefabs/" + viewPath, layer, loadType);
    }
}
