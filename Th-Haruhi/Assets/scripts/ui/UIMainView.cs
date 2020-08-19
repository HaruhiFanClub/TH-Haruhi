
using LitJson;
using UnityEngine;

public class UIMainView : UiInstance
{
    private UIMainViewCompent _compent;
    public static void Show()
    {
        UiManager.ImmediatelyShow<UIMainView>();
    }
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
       
        _compent = GetComponent<UIMainViewCompent>();
        _compent.BtnEnterGame.onClick.AddListener(ClickEnterGame);
    }

    private void ClickEnterGame()
    {
        GameWorld.FirstStartGame();
        this.Close();
    }
}
