

using System;

public class UiMenuHoriz : UiMenuBase
{
    protected override void Start()
    {
        base.Start();
        GameEventCenter.AddListener(GameEvent.UI_Left, OnClickPrev);
        GameEventCenter.AddListener(GameEvent.UI_Right, OnClickRight);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEventCenter.RemoveListener(GameEvent.UI_Left, OnClickPrev);
        GameEventCenter.RemoveListener(GameEvent.UI_Right, OnClickRight);
    }

    private void OnClickRight(object argument)
    {
        if(Enable)
            SelectNext();
    }

    private void OnClickPrev(object argument)
    {
        if (Enable)
            SelectPrev();
    }
}
