using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(UiImage))]
public class UiButton : Button
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

    private const string ImageName = "ImageDisplay";

    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate OnDown;
    public VoidDelegate OnUp;
    public VoidDelegate OnEnter;
    public VoidDelegate OnExit;
    public bool isDebug;
    private bool m_isEnable = true;
    public bool isEnable
    {
        set
        {
            if (m_isEnable != value)
            {
                SetButtonEnable(value);
            }
            m_isEnable = value;
        }
        get { return m_isEnable; }
    }

    private UiImage _displayImage;
    public bool UseTween;
    public void SetUseTween(bool b)
    {
        if (UseTween != b)
        {
            if (b)
            {
                SetButtonUseDisplayImage(true);
            }
        }
        UseTween = b;
    }


    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        if (transform.GetComponent<UiImage>() == null)
        {
            transform.gameObject.AddComponent<UiImage>();
        }
#endif

        var img = transform.Find(ImageName);
        _displayImage = img != null ? img.GetComponent<UiImage>() : transform.GetComponent<UiImage>();
        if (Application.isPlaying && isDebug && !Debug.isDebugBuild)
        {
            SetActiveByCanvasGroup(false);
        }
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
        if (b && isDebug && !Debug.isDebugBuild)
        {
            return;
        }
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
        if (UseTween) OnDownTween();
    }

    private Tween _tween;
    private Vector3 _originalScale;
    private void OnDownTween()
    {
        if (!isEnable) return;
        if (_tween != null && _tween.IsPlaying())
        {
            _tween.Kill();
            RestScale();
        }

        _originalScale = _displayImage.RectTransform.localScale;
        _tween = _displayImage.transform.DOScale(_originalScale * 0.85f, 0.1f);
        _tween.SetUpdate(true);
    }

    private void OnUpTween()
    {
        if (_tween != null && _tween.IsPlaying())
        {
            _tween.OnComplete(RestScale);
        }
        else
        {
            RestScale();
        }
    }

    private void RestScale()
    {
        if (_originalScale == Vector3.zero) _originalScale = Vector3.one;
        _displayImage.transform.localScale = _originalScale * 1;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnUp != null) OnUp.Invoke(gameObject);
        if (UseTween) OnUpTween();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (OnEnter != null) OnEnter.Invoke(gameObject);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (OnExit != null) OnExit.Invoke(gameObject);
    }


    private void SetButtonUseDisplayImage(bool b)
    {
#if UNITY_EDITOR

        //如果是use tween, 需要区分点击区域与事件区域
        if (!enabled) return;
        var sourceImage = GetComponent<UiImage>();
        if (sourceImage == null) return;
        if (!sourceImage.enabled) return;

        Transform img = transform.Find(ImageName);
        if (b)
        {
            UiImage displayImage = null;
            if (img == null)
            {
                GameObject imageObj = new GameObject();
                imageObj.layer = Layers.Ui;
                imageObj.name = ImageName;
                imageObj.transform.SetParent(transform);
                displayImage = imageObj.AddComponent<UiImage>();
            }
            else
            {
                img.gameObject.layer = Layers.Ui;
                if (img.gameObject.activeSelf == false)
                {
                    img.gameObject.SetActiveSafe(true);
                    displayImage = img.GetComponent<UiImage>();
                }
            }

            if (displayImage != null)
            {
                displayImage.rectTransform.sizeDelta = sourceImage.rectTransform.sizeDelta;
                displayImage.RectTransform.anchoredPosition = Vector2.zero;
                displayImage.color = sourceImage.color;
                displayImage.material = sourceImage.material;
                displayImage.type = sourceImage.type;
                displayImage.fillAmount = sourceImage.fillAmount;
                displayImage.fillCenter = sourceImage.fillCenter;
                displayImage.sprite = sourceImage.sprite;
                displayImage.raycastTarget = false;
                displayImage.transform.SetAsFirstSibling();
                sourceImage.Alpha = 0;

                var objs = transform.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i].parent != transform) continue;
                    if (objs[i] == transform) continue;
                    if (objs[i] == displayImage.transform) continue;
                    objs[i].SetParent(displayImage.transform);
                }
            }
        }
        else
        {
            if (img != null)
            {
                var objs = img.transform.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i].parent != img.transform) continue;
                    if (objs[i] == transform) continue;
                    objs[i].SetParent(transform);
                }
                img.gameObject.SetActiveSafe(false);
            }
            sourceImage.Alpha = 1;
        }

#endif
    }

    private void SetButtonEnable(bool enable)
    {
        interactable = enable;

        if (_displayImage != null)
        {
            _displayImage.material = enable ? GameSystem.DefaultRes.UiDefault : GameSystem.DefaultRes.UiGray;
        }
    }
}