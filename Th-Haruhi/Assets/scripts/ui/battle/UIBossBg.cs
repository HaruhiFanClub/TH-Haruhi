
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIBossBg : UiInstance
{
    private static UIBossBg Instance;
    private UIBossBgComponent _bind;
    private bool _bDrawingAni;

    public static void FadeOut()
    {
        if (Instance)
        {
            Instance._bind.BgCanvasGroup.DOFade(0f, 1f).onComplete = () =>
            {
                Instance.Close();
            };
        };
    }

    public static void Show(string drawingPath)
    {
        if(Instance != null)
        {
            Instance.Close();
            Instance = null;
        }

        UiManager.Show<UIBossBg>(view =>
        {
            view.Refresh(drawingPath);
        });
    }

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIBossBgComponent>();
        Instance = this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }

    private void Refresh(string drawingPath)
    {
        _bind.BgCanvasGroup.alpha = 0;
        _bind.BgCanvasGroup.DOFade(1f, 1f);
        _bind.BossDrawing.rectTransform.anchoredPosition = DrawingStartPos;
        _bind.BossDrawing.SetActiveSafe(true);

        _bind.BossDrawing.SetRawImageTexture(drawingPath);
        _bind.BossDrawing.SetNativeSize();

        var h = 900f;
        var size = _bind.BossDrawing.rectTransform.sizeDelta;
        var xRatio = h / size.y;
        _bind.BossDrawing.rectTransform.sizeDelta = new Vector2(size.x * xRatio, h);

        StartCoroutine(DelayHideAtkText());

        _bDrawingAni = true;
    }

    private IEnumerator DelayHideAtkText()
    {
        yield return new WaitForSeconds(2f);
        _bind.AtkTextAlpha.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        _bind.AtkTextRoot.SetActiveSafe(false);
    }

    private Vector2 DrawingStartPos = new Vector2(900, 1000);

    protected override void Update()
    {
        base.Update();

        if (!_bDrawingAni) return;

        var pos = _bind.BossDrawing.rectTransform.anchoredPosition;
        if(pos.x < -1000)
        {
            _bind.BossDrawing.SetActiveSafe(false);
            _bDrawingAni = false;
            return;
        }

        var moveSpeed = 2000f;
        if(pos.x <-100 && pos.x > -200)
        {
            moveSpeed = 60f;
        }

        var move = Time.deltaTime * moveSpeed;
        pos.x -= move;
        pos.y -= move;
        _bind.BossDrawing.rectTransform.anchoredPosition = pos;
    }
}
