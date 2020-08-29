
using UnityEngine;
using DG.Tweening;

public class ParticleFade : MonoBehaviour
{
    public float FadeTime = 10f;
    private Material _material;
    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        var currColor = _material.GetColor("_TintColor");
        currColor.a = 0;
        _material.SetColor("_TintColor", currColor);
    }

    private void Update()
    {
        var currColor =  _material.GetColor("_TintColor");
        if (currColor.a < 1f)
        {
            var delta = Time.deltaTime / FadeTime;
            currColor.a += delta;
            _material.SetColor("_TintColor", currColor);
        }
    }
}