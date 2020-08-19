

using System;

public class UiTextBtnMenuVert : UiTextBtnMenuBase
{
    protected override void Start()
    {
        base.Start();
        GameEventCenter.AddListener(GameEvent.UI_Up, OnClickUp);
        GameEventCenter.AddListener(GameEvent.UI_Down, OnClickDown);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEventCenter.RemoveListener(GameEvent.UI_Up, OnClickUp);
        GameEventCenter.RemoveListener(GameEvent.UI_Down, OnClickDown);
    }

    private void OnClickDown(object argument)
    {
        SelectNext();
    }

    private void OnClickUp(object argument)
    {
        SelectPrev();
    }
}
