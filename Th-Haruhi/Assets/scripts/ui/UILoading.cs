using DG.Tweening;
using System;

public class UILoading : UiInstance
{
    private UILoadingCompoent _compoent;
    private Action _showOver;
    public static bool InLoading;

    public static void Show(Action showOver)
    {
        UiManager.Show<UILoading>(view => { view._showOver = showOver; });
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

        InLoading = true;
        _compoent.Image.Alpha = 0f;
        _compoent.Image.DOFade(1f, 0.5f).onComplete = ()=> 
        {
            if (_showOver != null)
            {
                _showOver();
                _showOver = null;
            }
        };
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        InLoading = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InLoading = false;
    }
}
