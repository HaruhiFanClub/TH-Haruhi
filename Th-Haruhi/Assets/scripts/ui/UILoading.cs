using DG.Tweening;

public class UILoading : UiInstance
{
    private UILoadingCompoent _compoent;
    public static void Show()
    {
        UiManager.ImmediatelyShow<UILoading>();
    }

    public static void Close()
    {
        UiManager.GetInstance<UILoading>().Close();
    }
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _compoent = GetComponent<UILoadingCompoent>();
    }

    protected override void OnShow()
    {
        base.OnShow();
        _compoent.Image.Alpha = 0f;
        _compoent.Image.DOFade(1f, 0.5f);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
