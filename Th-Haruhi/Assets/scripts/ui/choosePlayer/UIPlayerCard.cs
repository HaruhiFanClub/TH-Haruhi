using DG.Tweening;
using System;
using UnityEngine;

public class UIPlayerCard : MonoBehaviour, ISelectAble
{
    public Animator Animator;

    public bool IsSelected { private set; get; } = true;
    public bool IsEnable => true;
    public bool InClick { private set; get; }

    public RectTransform RectTransform => GetComponent<RectTransform>();

    private const float ClickWaitTime = 0.2f;

    public Action OnClick;

    public void DoClick()
    {
        if (InClick) return;
        InClick = true;
        Sound.PlayTHSound("ok00");


        DOVirtual.DelayedCall(ClickWaitTime, () =>
        {
            OnClick?.Invoke();
            InClick = false;
        });
    }
   
    public void SetSelect(bool b, bool fromAuto = false) { }

    public void OnUpdate() { }
}
