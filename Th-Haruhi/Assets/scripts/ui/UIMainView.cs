
using System.Collections;
using UnityEngine;

public class UIMainView : UiFullView
{
    public static int PrevSelected = 0;

    public static void Show(bool isFirst)
    {
        var view = UiManager.ImmediatelyShow<UIMainView>(!isFirst);
        view.DoOpen(isFirst);
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
    }

    private void DoOpen(bool bFisstOpen)
    {
        _compent.Menu.Enable = false;
        UiManager.BackGround.BgMask.Alpha = 0f;

        if (bFisstOpen)
        {
            _compent.Animator.Play("FirstOpen");
            StartCoroutine(FirstOpen());
        }
        else
        {
            _compent.Animator.Play("ReOpen");
            //Sound.PlayMusic(1);
        }
    }

    private IEnumerator FirstOpen()
    {
        yield return Sound.CacheSound(1);
        Sound.PlayMusic(1);
        UiManager.BackGround.FadeBg(3f);
        yield return new WaitForSeconds(3f);
        UiManager.BackGround.EnableEffect(true);
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
        LevelMgr.Data = new LevelData();
        LevelMgr.Data.CurLevelId = 1;

        UiManager.Show<UIChooseDifficult>();
    }

    private void Btn_ExtraStart()
    {
        

    }
    private void Btn_ParcticeStart()
    {
        LevelMgr.Data = new LevelData();
        //LevelMgr.StartNewGame(1);
    }

    private void Btn_SpellParctice()
    {
        UiTips.Show(Application.streamingAssetsPath);
    }
    private void Btn_Replay()
    {
        UiTips.Show(Application.dataPath);
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
