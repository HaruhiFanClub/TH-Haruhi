

using System;

public abstract class UiFullView : UiInstance
{
    public static UiFullView CurrentView { private set; get; }
    public static Type PrevViewType { private set; get; }


    static UiFullView()
    {
        GameEventCenter.AddListener(GameEvent.UI_Back, OnBack);
    }

    private static void OnBack(object o)
    {
        if (PrevViewType == null || CurrentView.GetType() == typeof(UIMainView))
            return;

        CurrentView.Close();

        if(PrevViewType == typeof(UIMainView))
        {
            UIMainView.Show(false);
        }
        else
        {
            UiManager.Show(PrevViewType, null);
        }
    }

    //记录上一个选择的界面
    protected override void OnShow()
    {
        base.OnShow();
        if(CurrentView != null)
        {
            PrevViewType = CurrentView.GetType();
            CurrentView.Close();
        }
        CurrentView = this;
    }
}
