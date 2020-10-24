
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UiTips : UiInstance
{
    private UiTipsCompoent _compoent;
    public static void Show(string str)
    {
        UiManager.Show<UiTips>(view => 
        {
            view.ShowTips(str);
        });
    }

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _compoent = GetComponent<UiTipsCompoent>();
    }

    private void ShowTips(string str)
    {
        transform.localScale = Vector3.zero;
        _compoent.Text.text = str;

        transform.DOScale(Vector3.one, 0.1f);
    }

    protected override void OnShow()
    {
        base.OnShow();
        StartCoroutine(DoClose(2f));
    }

    private IEnumerator DoClose(float waitsec)
    {
        yield return new WaitForSeconds(waitsec);
        transform.DOScale(Vector3.zero, 0.1f).onComplete = () =>
        {
            this.Close();
        };
    }
}
