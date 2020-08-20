
public class UIGameView : UiInstance
{
    private UIGameViewCompoent _compoent;
    public static void Show()
    {
        UiManager.ImmediatelyShow<UIGameView>();
    }
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
    }

}
