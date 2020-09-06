using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

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
        _bind.Menu.OnInitOver += OnMenuInit;
        _bind.Menu.OnSelectChange += OnSelectChange;

        _bind.Marisa.OnClick = () => { OnSure(1); };
        _bind.Reimu.OnClick = () => { OnSure(2); };
        _bind.Sakuya.OnClick = () => { OnSure(3); };
    }

    private void OnSure(int playerId)
    {
        StageMgr.Data.PlayerId = playerId;
        StageMgr.StartGame();
    }

    protected override void OnDestroy()
    {
        _bind.Menu.OnInitOver -= OnMenuInit;
        _bind.Menu.OnSelectChange -= OnSelectChange;
    }

    private void OnMenuInit()
    {
        _bind.Reimu.SetActiveSafe(false);
        _bind.Marisa.SetActiveSafe(false);
        _bind.Sakuya.SetActiveSafe(false);

        StartCoroutine(DelayShow());
    }

    private IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(0.5f);
        var currSelect = _bind.Menu.ItemList[_bind.Menu.CurrSelectIdx] as UIPlayerCard;
        currSelect.SetActiveSafe(true);
        currSelect.Animator.Play("LeftOpen");

        _bind.Menu.Enable = true;
    }

    private void OnSelectChange(ISelectAble select, ISelectAble unSelect, bool isNext)
    {
        var selectedCard = select as UIPlayerCard;
        var unSelectCard = unSelect as UIPlayerCard;
        selectedCard.SetActiveSafe(true);
        if (isNext)
        {
            selectedCard.Animator.Play("RightOpen");
            unSelectCard.Animator.Play("RightClose");
        }
        else
        {
            selectedCard.Animator.Play("LeftOpen");
            unSelectCard.Animator.Play("LeftClose");
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        UiManager.BackGround.MaskFadeIn(0.7f, 0.4f);
    }


    //難度圖標邏輯
    private UIChooseDifficultItem _difficultItem;
    private Vector3 _difficultItemScale;
    private Vector3 _difficultItemPos;
    private void Refresh(UIChooseDifficultItem difficultItem)
    {
        _difficultItem = difficultItem;
        _difficultItemScale = difficultItem.transform.localScale;
        _difficultItemPos = difficultItem.transform.position;

        difficultItem.transform.SetParent(transform, false);
        difficultItem.StartFlash();
        DOVirtual.DelayedCall(0.4f, () => 
        { 
            difficultItem.EndFlash();
            difficultItem.transform.DOMove(_bind.DiffEndPos.position, 0.3f);
            difficultItem.transform.DOScale(0.85f, 0.3f);
        });
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);

        var currSelect = _bind.Menu.ItemList[_bind.Menu.CurrSelectIdx] as UIPlayerCard;
        if (currSelect != null) 
        {
            currSelect.Animator.Play("LeftClose");
        }
        if (_difficultItem != null)
        {
            var sec = 0.2f;
            _difficultItem.transform.SetParent(UiManager.GetCanvas(UiLayer.Tips));
            _difficultItem.transform.DOMove(_difficultItemPos, sec);
            _difficultItem.CanvasGroup.DOFade(0f, sec);
            _difficultItem.transform.DOScale(_difficultItemScale, sec).onComplete = ()=>
            {
                Destroy(_difficultItem.gameObject);
            };
        }
    }

    protected override void OnOpenOver()
    {
    }
}
