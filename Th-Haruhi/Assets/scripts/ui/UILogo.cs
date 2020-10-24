
using System;
using DG.Tweening;

public class UILogo : UiInstance
{
    private UILogoCompoent _compoent;
    private Action _onClose;
    public static void Show(Action onClose)
    {
        UiManager.Show<UILogo>( view =>
        {
            view._onClose = onClose;
        });
    }

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();

        _compoent = GetComponent<UILogoCompoent>();
        _compoent.Logo.Alpha = 0;

    }

    private float fadeInTime = 1f;
    private float fadeOutTime = 1f;
    private float waitTime = 1f;
    protected override void OnShow()
    {
        base.OnShow();
        _compoent.Logo.DOFade(1f, fadeInTime).onComplete = () =>
        {
            DOVirtual.DelayedCall(waitTime, () =>
            {
                _compoent.Logo.DOFade(0f, fadeOutTime).onComplete = () =>
                {
                    this.Close();
                };
            });
        };
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        _onClose();
    }

}
