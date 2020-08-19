
public class UIGameView : UiInstance
{
    private UIGameViewCompent _compent;
    public static void Show()
    {
        UiManager.Show<UIGameView>();
    }
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _compent = GetComponent<UIGameViewCompent>();
        _compent.BtnBack.onClick.AddListener(Back);

        _compent.Haruhi.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Haruhi));
        _compent.Mikuru.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Mikuru));
        _compent.Nagato.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Nagato));
        _compent.Kyon.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Kyon));
        _compent.Koizumi.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Koizumi));
    }

    private void Back()
    {
        GameWorld.ShowTitle();
        this.Close();
    }
}
