
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStageClear : UiInstance
{
    public static UIStageClear Instance;

    private UIStageClearComponent _component;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _component = GetComponent<UIStageClearComponent>();
        Refresh();
    }

    protected override void OnShow()
    {
        base.OnShow();
        Instance = this;
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        Instance = null;
    }

    private void Refresh()
    {
        var testScore = 100000000;
        _component.Bonus.text = testScore.ToString("N0");
        CanvasGroup.alpha = 0;
        CanvasGroup.DOFade(1f, 1f);
    }

    public void FadeOutClose()
    {
        CanvasGroup.DOFade(0f, 1f).onComplete = () =>
        {
            this.Close();
        };
    }
}
