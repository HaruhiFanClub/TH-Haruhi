
using System;
using UnityEngine;
using DG.Tweening;

public class UIChooseDifficult : UiFullView
{
    private UIChooseDifficultComponent _bind;
    
    protected override Animator Animator => _bind.Animator;

    private const int ItemHeight = 240;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIChooseDifficultComponent>();
        _bind.Menu.Enable = false;

        //默认选中普通
        UiMenuBase.SetDefultIndex(GetType(), 1);
        _bind.Menu.OnInitOver += OnMenuInitOver;
        _bind.Menu.OnSelectChange += OnSelectChange;

        _bind.Easy.onClick.AddListener(()=> { OnClick(ELevelDifficult.Easy); });
        _bind.Normal.onClick.AddListener(() => { OnClick(ELevelDifficult.Normal); });
        _bind.Hard.onClick.AddListener(() => { OnClick(ELevelDifficult.Hard); });
        _bind.Lunatic.onClick.AddListener(() => { OnClick(ELevelDifficult.Lunatic); });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _bind.Menu.OnInitOver -= OnMenuInitOver;
        _bind.Menu.OnSelectChange -= OnSelectChange;
    }

    private void OnClick(ELevelDifficult diff)
    {
        LevelMgr.Data.Difficult = diff;


        //复制出来闪烁
        var source = _bind.Menu.ItemList[_bind.Menu.CurrSelectIdx] as UIChooseDifficultItem;
        var obj = ResourceMgr.Instantiate(source.gameObject);
        var item = obj.GetComponent<UIChooseDifficultItem>();
        source.SetActiveSafe(false);
        UIChoosePlayer.Show(item);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UiManager.BackGround.MaskFadeIn(0.7f, 0.4f);
    }

    protected override void OnOpenOver()
    {
        _bind.Menu.Enable = true;
    }

    private void OnSelectChange(ISelectAble select, ISelectAble unSelect, bool isNext)
    {
        for (int i = 0; i < _bind.Menu.ItemList.Count; i++)
        {
            var item = _bind.Menu.ItemList[i];
            var diffY = (i - _bind.Menu.CurrSelectIdx) * -ItemHeight;
            item.RectTransform.DOAnchorPosY(diffY, 0.4f);
        }
        TweenSelectBg();
    }


    private Tweener _bgTween;
    private void TweenSelectBg()
    {
        if (_bgTween != null)
        {
            _bgTween.Kill();
        }
        _bgTween = _bind.Bg.rectTransform.DORotate(new Vector3(0, 0, GetSelectBgRotaZ()), 0.5f);
    }

    private float GetSelectBgRotaZ()
    {
        var z = (_bind.Menu.CurrSelectIdx - 1) * -10;
        if ((_bind.Menu.CurrSelectIdx - 1) > 0)
            z /= 2;
        return z;
    }

    private void OnMenuInitOver()
    {
        PlayOpenAni();
    }

    private void PlayOpenAni()
    {
        var aniSec = 0.3f;

        for (int i = 0; i < _bind.Menu.ItemList.Count; i++)
        {
            var item = _bind.Menu.ItemList[i];
            item.RectTransform.anchoredPosition = Vector2.zero;
            var diffY = (i - _bind.Menu.CurrSelectIdx) * -ItemHeight;
            item.RectTransform.DOAnchorPosY(diffY, aniSec);
        }
        
        _bind.Bg.Alpha = 0;
        _bind.Bg.DOFade(0.8f, aniSec);

        _bind.Bg.rectTransform.eulerAngles = new Vector3(0, 90, 90);
        _bind.Bg.rectTransform.DORotate(new Vector3(0, 0, GetSelectBgRotaZ()), aniSec);
    }

    private void PlayCloseAni()
    {
        var aniSec = 0.7f;
        for (int i = 0; i < _bind.Menu.ItemList.Count; i++)
        {
            var item = _bind.Menu.ItemList[i];
            item.RectTransform.DOAnchorPosY(0, aniSec);
        }

      
        _bind.Bg.rectTransform.DORotate(new Vector3(0, 90, 90), aniSec);
        _bind.Bg.DOFade(0F, aniSec);
    }


    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        PlayCloseAni();
        UiManager.BackGround.MaskFadeOut(0.6f);
    }
}
