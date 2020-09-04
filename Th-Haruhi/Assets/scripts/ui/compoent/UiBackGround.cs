using UnityEngine;
using DG.Tweening;

public class UiBackGround : MonoBehaviour
{
    public UiImage Bg;
    public UiImage BgMask;
    public GameObject Effect;

    public void FadeBg(float sec)
    {
        Bg.Alpha = 0f;
        Bg.DOFade(1f, sec);
    }

    public void EnableEffect(bool b)
    {
        Effect.SetActiveSafe(b);
    }

    public void MaskFadeIn(float sec, float endAlpha)
    {
        BgMask.DOFade(endAlpha, sec);
    }
    public void MaskFadeOut(float sec)
    {
        BgMask.DOFade(0f, sec);
    }
}
