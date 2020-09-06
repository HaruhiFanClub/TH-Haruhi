
using System;
using System.Collections;
using UnityEngine;

public class UIDeadView : UiInstance
{
    private UIDeadViewComponent _bind;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIDeadViewComponent>();
        _bind.BtnRetry.onClick.AddListener(BtnRetry);
        _bind.BtnBack.onClick.AddListener(BtnBack);
        _bind.BtnRestart.onClick.AddListener(BtnRestart);
    }

    protected override void OnShow()
    {
        base.OnShow();
        _bind.Menu.Enable = true;
        //Sound.PlayUiAudioOneShot(1004);
        Sound.PauseMusic();
        GamePause.PauseGame(EPauseFrom.Esc);
    }

    //续关
    private void BtnRetry()
    {
        this.Close();
        StageMgr.Retry();
    }

    private void BtnBack()
    {
        GameSystem.ShowTitle();
    }

    //从头开始
    private void BtnRestart()
    {

    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        Sound.UnPauseMusic();
        GamePause.DoContionueGame(EPauseFrom.Esc);
    }
}
