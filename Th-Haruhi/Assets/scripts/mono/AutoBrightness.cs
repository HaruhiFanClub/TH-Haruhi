
using UnityEngine;
using DG.Tweening;

public class AutoBrightness : MonoBehaviour
{
    public float Max = 1.5f;
    public float Min = 1f;
    public float Speed = 1f;
    private int addFlag;
    private float _current;
    private float _default;

    private Material _material;
    private void Awake()
    {
        _material = GetComponent<Renderer>().sharedMaterial;
        _current = _material.GetFloat("_Brightness");
        _default = _current;
    }

    private void Update()
    {
        if (_current <= Min)
        {
            addFlag = 1;
        }
        if (_current >= Max)
        {
            addFlag = -1;
        }
        _current += Time.deltaTime * Speed * addFlag;
        _material.SetFloat("_Brightness", _current);
    }

    private void OnDestroy()
    {
        _material.SetFloat("_Brightness", _default);
    }
}