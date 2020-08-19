
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UIMainView : UiInstance
{
    
    public static void Show(bool showAni)
    {
        var view = UiManager.ImmediatelyShow<UIMainView>();
        view.DoOpen(showAni);
    }

    private UIMainViewCompoent _compent;
    private bool _inited;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();

        _compent = GetComponent<UIMainViewCompoent>();
       
        //test
        _compent.Btn_ExtraStart.IsEnable = false;
    }

    private void DoOpen(bool playAni)
    {
        StartCoroutine(PlayAnimation(playAni));
    }

    private IEnumerator PlayAnimation(bool playAni)
    {
        if(!playAni)
        {
            AddBtnEvent();
            yield break;
        }

        _compent.Bg.Alpha = 0;
        _compent.MenuCanvasGroup.alpha = 0;

        yield return new WaitForSeconds(2f);
        _compent.Bg.DOFade(1f, 3f);
        Sound.PlayMusic(1);

        yield return new WaitForSeconds(1.5f);
        _compent.MenuCanvasGroup.DOFade(1f, 0.3f).onComplete = AddBtnEvent;
    }

    private void AddBtnEvent()
    {
        if (_inited) return;
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
        _inited = true;
    }

    protected override void OnShow()
    {
        base.OnShow();

        
    }

    private void Btn_GameStart()
    {
        UiTips.Show("Btn_GameStart");
    }
    private void Btn_ExtraStart()
    {
        UiTips.Show("Btn_ExtraStart");
    }
    private void Btn_ParcticeStart()
    {
        UiTips.Show("Btn_ParcticeStart");
    }
    private void Btn_SpellParctice()
    {
        UiTips.Show("Btn_SpellParctice");
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
}
