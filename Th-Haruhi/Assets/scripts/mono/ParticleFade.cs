
using UnityEngine;
using DG.Tweening;

public class ParticleFade : MonoBehaviour
{
    public float FadeTime = 10f;
    public float TargetAlpha = 0.7f;
    private Material _material;
    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _material.SetFloat("_AlphaScale", 0);
    }

    private void Update()
    {
        var currAlpha =  _material.GetFloat("_AlphaScale");
        if (currAlpha < TargetAlpha)
        {
            var delta = Time.deltaTime / FadeTime;
            currAlpha += delta;
            _material.SetFloat("_AlphaScale", currAlpha);
        }
    }
}