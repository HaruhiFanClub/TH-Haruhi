using UnityEngine;
using DG.Tweening;

public class UIChoosePlayer : UiFullView
{
    public static void Show(UIChooseDifficultItem difficultItem)
    {
        UiManager.Show<UIChoosePlayer>(view=>
        {
            view.Refresh(difficultItem);
        });
    }


    private UIChoosePlayerComponet _bind;

    protected override Animator Animator => _bind.Animator;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIChoosePlayerComponet>();
    }

    private void Refresh(UIChooseDifficultItem difficultItem)
    {
        difficultItem.transform.SetParent(transform, false);
        difficultItem.StartFlash();
        DOVirtual.DelayedCall(0.4f, () => 
        { 
            difficultItem.EndFlash();
            difficultItem.transform.DOMove(_bind.DiffEndPos.position, 0.3f);
            difficultItem.transform.DOScale(0.8f, 0.3f);
        });
    }
    protected override void OnOpenOver()
    {
        _bind.Menu.Enable = true;
    }
}
