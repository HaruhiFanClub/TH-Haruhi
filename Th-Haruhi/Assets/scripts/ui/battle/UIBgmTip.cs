
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBgmTip : UiInstance
{
    public static void Show(int bgmId)
    {
        var soundDeploy = TableUtility.GetDeploy<SoundDeploy>(bgmId);
        if (soundDeploy == null) 
        {
            return;
        }
        if (string.IsNullOrEmpty(soundDeploy.name)) 
        {
            return;
        }

        UiManager.Show<UIBgmTip>(view =>
        {
            view.Refresh(soundDeploy.name);
        });
    }

    private UIBgmTipComponent _component;
    private Vector2 _textDefaultPos;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _component = GetComponent<UIBgmTipComponent>();
        _textDefaultPos = _component.Text.rectTransform.anchoredPosition;
    }

    private void Refresh(string name)
    {
        StartCoroutine(DoRefrsh(name));
    }

    private IEnumerator DoRefrsh(string name)
    {
        _component.Text.text = string.Format("BGM.  {0}", name);
        var pos = _textDefaultPos;
        pos.x += 450;
        _component.Text.rectTransform.anchoredPosition = pos;
        _component.Text.Alpha = 1f;

        _component.Text.rectTransform.DOAnchorPos(_textDefaultPos, 1f);
        yield return new WaitForSeconds(5f);
        _component.Text.DOFade(0f, 1f);

        yield return new WaitForSeconds(1f);
        this.Close();
    }
}
