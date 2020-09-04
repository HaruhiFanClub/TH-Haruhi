
using System;
using System.Collections;
using UnityEngine;

public class UIPauseView : UiInstance
{
    private UIPauseViewComponent _bind;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIPauseViewComponent>();
        _bind.BtnUnpause.onClick.AddListener(BtnUnpause);
        _bind.BtnBack.onClick.AddListener(BtnBack);
    }

    private void BtnUnpause()
    {
        this.Close();
    }

    private void BtnBack()
    {
        //this.Close();
        GameSystem.ShowTitle();
    }

    protected override void OnShow()
    {
        base.OnShow();
        _bind.Menu.Enable = true;
        Sound.PlayUiAudioOneShot(1004);
        Sound.PauseMusic();
        GamePause.PauseGame(EPauseFrom.Esc);
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        Sound.UnPauseMusic();
        GamePause.DoContionueGame(EPauseFrom.Esc);
    }
}
