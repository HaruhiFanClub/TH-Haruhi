
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStageAllClear : UiInstance
{
    private UIStageAllClearComponent _component;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _component = GetComponent<UIStageAllClearComponent>();
        StartCoroutine(Refresh());
    }

    private IEnumerator Refresh()
    {

        var str = "";
        switch (StageMgr.Data.Difficult)
        {
            case ELevelDifficult.Easy:
                str = "恭喜通关 茶要凉了哦";
                break;
            case ELevelDifficult.Normal:
                str = "恭喜通关 弹幕池要满了哦";
                break;
            case ELevelDifficult.Hard:
                str = "恭喜通关 棋盘要被将军了哦";
                break;
            case ELevelDifficult.Lunatic:
                str = "恭喜通关 书要看完了哦";
                break;
            case ELevelDifficult.Extra:
                str = "恭喜通关 歌要唱完了哦";
                break;
        }

        _component.Text.text = str;
        _component.Text.Alpha = 0f;
        _component.Text.DOFade(1f, 1f);

        yield return new WaitForSeconds(2f);

        _component.Text.DOFade(0f, 1f);
        yield return new WaitForSeconds(2f);
        GameSystem.ShowTitle();
    }
}
