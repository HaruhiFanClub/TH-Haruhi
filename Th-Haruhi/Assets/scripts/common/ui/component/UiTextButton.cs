﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(UiImage))]
public class UiTextButton : Button
{
    private RectTransform _rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate OnDown;
    public VoidDelegate OnUp;
    public VoidDelegate OnEnter;
    public VoidDelegate OnExit;
    private bool _isEnable = true;
    public bool IsEnable
    {
        set
        {
            if (_isEnable != value)
            {
                SetButtonEnable(value);
            }
            _isEnable = value;
        }
        get { return _isEnable; }
    }

    private string TextName = "BtnText";


    private UiText _text;
    private Color _textDefaultColor;
    protected override void Awake()
    {
        base.Awake();

        var text = transform.Find(TextName);

#if UNITY_EDITOR
        var img = transform.GetComponent<UiImage>();
        if (img == null)
        {
            img = gameObject.AddComponent<UiImage>();
        }
        img.Alpha = 0;

        if (text == null)
        {
            var textObj = new GameObject(TextName);
            text = textObj.transform;
            text.SetParent(transform, false);
            _text = textObj.AddComponent<UiText>();
            _text.alignment = TextAnchor.MiddleCenter;
            _text.text = TextName;

            var rt = _text.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }

#endif
        _text = text.GetComponent<UiText>();
        _textDefaultColor = _text.color;
    }

    private CanvasGroup _canvasGroup;
    private CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = this.CreateCanvasGroup();
            }
            return _canvasGroup;
        }
    }

    public void SetActiveByCanvasGroup(bool b)
    {
        CanvasGroup.SetActiveByCanvasGroup(b);
    }

    public bool GetActive()
    {
        return CanvasGroup.alpha >= 1;
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (OnDown != null) OnDown.Invoke(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnUp != null) OnUp.Invoke(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (OnEnter != null) OnEnter.Invoke(gameObject);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(IsEnable && IsSelected)
            base.OnPointerClick(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (OnExit != null) OnExit.Invoke(gameObject);
    }

    private void SetButtonEnable(bool enable)
    {
        interactable = enable;
        _text.color = enable ? _textDefaultColor : new Color(0.3f, 0.3f, 0.3f, _textDefaultColor.a);
    }

    //选择逻辑
    public int MenuIndex { set; get; }
    public bool IsSelected { private set; get; } = true;

    private Tween _shakeTween;
    private float _alphaAddFlag;
    private bool _isClicked;

    private const float SelectAlphaSmooth = 0.8f;
    private const float ClickAlphaSmooth = 7f;
    private const float ClickWaitTime = 0.7f;

    public void SetSelect(bool b)
    {
        if (!IsEnable) return;
        if (IsSelected == b) return;

        if (b && !IsSelected)
            OnSelectEnable();
        if (!b && IsSelected)
            OnSelectDisable();

        interactable = IsSelected;
        IsSelected = b;
        _text.color = IsSelected ? _textDefaultColor : new Color(0.5f, 0.5f, 0.5f, _textDefaultColor.a);
    }


    public void OnUpdate()
    {
        if (!IsEnable) return;

        if(_isClicked)
        {
            if (_text.Alpha >= 1f)
                _alphaAddFlag = -1;
            if (_text.Alpha < 0.5f)
                _alphaAddFlag = 1;

            _text.Alpha += ClickAlphaSmooth * Time.deltaTime * _alphaAddFlag;
        }
        else if(IsSelected)
        {
            if (_text.Alpha >= 1f)
                _alphaAddFlag = -1;
            if(_text.Alpha < 0.7f)
                _alphaAddFlag = 1;

            _text.Alpha += SelectAlphaSmooth * Time.deltaTime * _alphaAddFlag;
        }
    }

    public void DoClick()
    {
        _isClicked = true;
        Sound.PlayUiAudioOneShot(1002);

        DOVirtual.DelayedCall(ClickWaitTime, () =>
        {
            _isClicked = false;
            onClick.Invoke();
        });
    }

    private void OnSelectEnable()
    {
        _shakeTween = transform.DOShakePosition(0.2F, 8, 50, 90, true);

        Sound.PlayUiAudioOneShot(1001);
    }

    private void OnSelectDisable()
    {
        if(_shakeTween != null)
        {
            _shakeTween.Kill(true);
            _shakeTween = null;
        }
    }

    public void OnClickAnimation()
    {

    }
}