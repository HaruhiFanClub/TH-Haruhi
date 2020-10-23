
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UISpellParctise : UiFullView
{
    protected override Animator Animator => _bind.Animator;

    private UISpellParctiseBind _bind;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UISpellParctiseBind>();

        Refresh();
    }

    protected override void OnOpenOver()
    {
        _bind.Menu.Enable = true;
    }


    private void Refresh()
    {
        AddItem("Kyo_NoSpell", 10101);
        AddItem("Kyo_NoSpell_4", 10102);
        AddItem("「如电话线缠在一起般的羁绊」", 10103);
        AddItem("『诺维科夫所见的宇宙』", 10104);
    }

    private void AddItem(string name, int bossId)
    {
        var menuObj = ResourceMgr.Instantiate(_bind.MenuItem);
        menuObj.SetActiveSafe(true);
        menuObj.GetComponentInChildren<UiText>().text = name;
        var btn = menuObj.GetComponent<UiTextButton>();
        btn.onClick.AddListener(() =>
        {

            StageMgr.Data = new StageData();
            StageMgr.Data.CurLevelId = 2;
            StageMgr.Data.PlayerId = 1;
            StageMgr.Data.LeftLifeCount = 99;
            StageMgr.Data.Difficult = ELevelDifficult.Lunatic;
            StageMgr.Data.ParctiseBossId = bossId;
            StageMgr.StartGame();
        });

        menuObj.transform.SetParent(_bind.Menu.transform, false);
    }
}
