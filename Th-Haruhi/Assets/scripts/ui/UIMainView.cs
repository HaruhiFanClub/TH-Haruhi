﻿
using System.Collections;
using UnityEngine;

public class UIMainView : UiFullView
{
    public static int PrevSelected = 0;

    public static void Show(bool showAni, bool playBgm)
    {
        UiManager.Show<UIMainView>(view=> 
        {
            view.StartCoroutine(view.DoOpen(showAni, playBgm));
        });
    }

    private UIMainViewCompoent _compent;

    protected override Animator Animator => _compent.Animator;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();

        _compent = GetComponent<UIMainViewCompoent>();
        _compent.Btn_ExtraStart.IsEnable = false;
        _compent.Btn_GameStart.onClick.AddListener(Btn_GameStart);
        _compent.Btn_ExtraStart.onClick.AddListener(Btn_ExtraStart);
        _compent.Btn_ParcticeStart.onClick.AddListener(Btn_ParcticeStart);
        _compent.Btn_SpellParctice.onClick.AddListener(Btn_SpellParctice);
        _compent.Btn_Replay.onClick.AddListener(Btn_Replay);
        _compent.Btn_PlayerData.onClick.AddListener(Btn_PlayerData);
        _compent.Btn_MusicRoom.onClick.AddListener(Btn_MusicRoom);
        _compent.Btn_Option.onClick.AddListener(Btn_Option);
        _compent.Btn_Manual.onClick.AddListener(Btn_Manual);
        _compent.Btn_Quit.onClick.AddListener(Btn_Quit);

        InitDebug();
    }

    private IEnumerator DoOpen(bool showAni, bool playBgm)
    {
        _compent.Menu.Enable = false;
        UiManager.BackGround.BgMask.Alpha = 0f;
        _compent.Animator.Play(showAni ? "FirstOpen" : "ReOpen");

        if (playBgm)
        {
            yield return Sound.CacheSound(1);
            Sound.PlayMusic(1);
        }

        if (showAni)
        {
            UiManager.BackGround.FadeBg(3f);
            yield return new WaitForSeconds(3f);
            UiManager.BackGround.EnableEffect(true);
        }
    }

    protected override void OnOpenOver()
    {
        if (!_compent.Menu.Enable)
        {
            _compent.Menu.Enable = true;
        }
    }

    private void Btn_GameStart()
    {
        StageMgr.Data = new StageData();
        StageMgr.Data.CurLevelId = 1;

        UiManager.Show<UIChooseDifficult>();
    }

    private void Btn_ExtraStart()
    {
        UiTips.Show("Btn_ExtraStart");
    }

    private void Btn_ParcticeStart()
    {
        UiTips.Show("Btn_ParcticeStart");
        /*
        StageMgr.Data = new StageData();
        StageMgr.Data.CurLevelId = 2;
        StageMgr.Data.PlayerId = 2;
        StageMgr.StartGame();
        */
    }

    private void Btn_SpellParctice()
    {
        UiManager.Show<UISpellParctise>();

        //LuaStg.LoadLuaSTG("nonspell.luastg");
        //UiTips.Show("Btn_SpellParctice");
    }
    private void Btn_Replay()
    {
        UiTips.Show("Btn_Replay");
    }
    private void Btn_PlayerData()
    {
        UiTips.Show("Btn_PlayerData");
    }
    private void Btn_MusicRoom()
    {
        UiTips.Show("Btn_MusicRoom");
    }
    private void Btn_Option()
    {
        UiTips.Show("Btn_Option");
    }
    private void Btn_Manual()
    {
        UiTips.Show("Btn_Manual");
    }
    private void Btn_Quit()
    {
        UILogo.Show(() =>
        {
            Application.Quit();
        });
    }


    //debug
    private void InitDebug()
    {
        if(!GameSetting.ShowDebugBtn)
        {
            _compent.DebugRoot.SetActiveSafe(false);
            return;
        }

        _compent.Debug_BulletLib.onClick.AddListener(() => 
        {
            Debug.Log("Debug Open BulletLib");
            UiManager.Show<UIDebugBulletLib>();
        });

        _compent.R480.onClick.AddListener(() => { GameSetting.SetGameResolutions(GameSetting.EResolution.R480); });
        _compent.R720.onClick.AddListener(() => { GameSetting.SetGameResolutions(GameSetting.EResolution.R720); });
        _compent.R960.onClick.AddListener(() => { GameSetting.SetGameResolutions(GameSetting.EResolution.R960); });
        _compent.R1440.onClick.AddListener(() => { GameSetting.SetGameResolutions(GameSetting.EResolution.R1440); });
        _compent.R1920.onClick.AddListener(() => { GameSetting.SetGameResolutions(GameSetting.EResolution.R1920); });
    }
}
