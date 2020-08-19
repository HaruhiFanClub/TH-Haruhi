

public class UiList
{
    public static void RegisterUI()
    {
        _<UIMainView>("MainView.prefab",  UiLayer.Main, UiLoadType.Multi); 
        _<UIGameView>("GameView.prefab",  UiLayer.Main, UiLoadType.Multi); 
    }

    private static void _<T>(string viewPath, UiLayer layer, UiLoadType loadType) where T : UiInstance
    {
        UiManager.R<T>("ui/prefabs/" + viewPath, layer, loadType);
    }
}
