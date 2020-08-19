
public class UIGameView : UiInstance
{
    private UIGameViewCompoent _compoent;
    public static void Show()
    {
        UiManager.Show<UIGameView>();
    }
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _compoent = GetComponent<UIGameViewCompoent>();
        _compoent.BtnBack.onClick.AddListener(Back);

        _compoent.Haruhi.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Haruhi));
        _compoent.Mikuru.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Mikuru));
        _compoent.Nagato.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Nagato));
        _compoent.Kyon.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Kyon));
        _compoent.Koizumi.onClick.AddListener(() => CharacterMgr.SwitchCharacter(CharacterMgr.Koizumi));
    }

    private void Back()
    {
        GameWorld.ShowTitle();
        this.Close();
    }
}
