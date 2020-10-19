using DG.Tweening;
using UnityEngine;

public class UIChooseDifficultItem : MonoBehaviour, ISelectAble
{
    public UiText Title;
    public UiText TxtDifficult;
    public UiText Desc;
    public UiButton Btn;
    public CanvasGroup CanvasGroup;

    public bool IsSelected { private set; get; } = true;
    public bool IsEnable => true;
    public bool InClick { private set; get; }

    public RectTransform RectTransform => GetComponent<RectTransform>();


    private Color _titleDefaultColor;
    private Color _diffDefaultColor;
    private Color _descDefaultColor;
    void Awake()
    {
        _titleDefaultColor = Title.color;
        _diffDefaultColor = TxtDifficult.color;
        _descDefaultColor = Desc.color;
    }

    private const float ClickWaitTime = 0.7f;
    public void DoClick()
    {
        if (InClick) return;
        Btn.onClick.Invoke();
        InClick = true;
        Sound.PlayTHSound("ok00");

        DOVirtual.DelayedCall(ClickWaitTime, () =>
        {
            InClick = false;
        });
    }
   
    public void SetSelect(bool b, bool fromAuto = false)
    {
        if (!IsEnable) return;
        if (IsSelected == b) return;

        IsSelected = b;

        Title.color = IsSelected ? _titleDefaultColor : new Color(0f, 0f, 0f, 0.6f);
        Desc.color = IsSelected ? _descDefaultColor : new Color(0f, 0f, 0f, 0.6f);
        TxtDifficult.color = IsSelected ? _diffDefaultColor : new Color(0.4f, 0.4f, 0.4f, 0.6f);
    }

    private float _alphaAddFlag;
    private const float ClickAlphaSmooth = 18f;
    private bool _bFlash;

    void Update()
    {
        if (!_bFlash) return;

        if (CanvasGroup.alpha >= 1f)
            _alphaAddFlag = -1;
        if (CanvasGroup.alpha < 0.4f)
            _alphaAddFlag = 1;

        CanvasGroup.alpha += ClickAlphaSmooth * Time.deltaTime * _alphaAddFlag;
    }

    public void StartFlash()
    {
        _bFlash = true;
    }

    public void EndFlash()
    {
        _bFlash = false;
        CanvasGroup.alpha = 1;
    }


    public void OnUpdate() { }
}
