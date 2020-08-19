using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiGameObject : UIBehaviour
{
    private RectTransform _rectTransform;

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = transform as RectTransform;
            return _rectTransform;
        }
    }
    public Transform TransForm;

    protected override void Awake()
    {
        base.Awake();
        TransForm = transform;
    }

    private CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroup
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
        if (b && !gameObject.activeSelf)
            gameObject.SetActive(true);
        CanvasGroup.SetActiveByCanvasGroup(b);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _rectTransform = null;
    }
}
