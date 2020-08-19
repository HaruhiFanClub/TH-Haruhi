using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiSlider : Slider
{
    public bool EnableSound = true;
    public Action OnChangeOver;

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnChangeOver != null)
            OnChangeOver();
    }
}
