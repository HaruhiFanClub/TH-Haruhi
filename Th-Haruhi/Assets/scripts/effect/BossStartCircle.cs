

using UnityEngine;
using DG.Tweening;

public class BossStartCircle : MonoBehaviour
{
    public float Time = 0.4f;
    public float TargetScale = 25f;
    public float TargetAlpha = 0f;
    public float StartScale = 0f;
    public float StartAlpha = 0.3f;

    void OnEnable()
    {
        var renderer = GetComponent<Renderer>();
        renderer.sortingOrder = SortingOrder.Effect;
        renderer.material.SetFloat("_AlphaScale", StartAlpha);
        transform.transform.localScale = Vector3.one * StartScale;
        transform.DOScale(TargetScale, Time);

        renderer.material.DOFloat(TargetAlpha, "_AlphaScale", 1f);
    }
}