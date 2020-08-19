

using DG.Tweening;
using System;
using UnityEngine;

public abstract class UiOpenView : UiInstance
{

    Vector3 _originScale = Vector3.one;
    protected override void Awake()
    {
        base.Awake();
        _originScale = transform.localScale;
    }

    private Tweener _open1;
    private Tweener _open2;

    private Tweener _close1;
    private Tweener _close2;

    protected new bool InOpen;

    protected override void OnShow()
    {
        InOpen = false;

        base.OnShow();


        KillTween();

        var aniTime = 0.1f;
        CanvasGroup.alpha = 0f;
        _open1 =  CanvasGroup.DOFade(1f, aniTime);
        _open1.SetUpdate(true);

        transform.localScale = _originScale * 0f;
        _open2 = transform.DOScale(_originScale, aniTime);
        _open2.SetUpdate(true);
        _open2.onComplete = () => { InOpen = true; };
        // transform.DoOpenScale();
    }

    protected virtual void OnBeforeClose()
    {

    }
    public override void OnClose(Action<UiInstance> notify)
    {
        InOpen = false;
        OnBeforeClose();
        
        KillTween();

        var aniTime = 0.15f;
        _close1 = CanvasGroup.DOFade(0f, aniTime);
        _close1.SetUpdate(true);

        _close2 = transform.DOScale(0f, aniTime);
        _close2.SetUpdate(true);
        _close2.onComplete = () =>
        {
            base.OnClose(notify);
            transform.localScale = _originScale;
        };
    }

    private void KillTween()
    {
        CanvasGroup.alpha = 1f;
        transform.localScale = _originScale;
        if (_open1 != null) _open1.Kill();
        if (_open2 != null) _open2.Kill();
        if (_close1 != null) _close1.Kill();
        if (_close2 != null) _close2.Kill();
    }
}
