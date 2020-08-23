
using UnityEngine;
using DG.Tweening;

public class AutoGamma : MonoBehaviour
{
    public float MaxGamma = 1f;
    public float MinGamma = 0f;
    public float Speed = 1f;
    private int addFlag;
    private float _curGamma;

    private Material _material;
    private void Awake()
    {
        _material = GetComponent<Renderer>().sharedMaterial;
        _curGamma = _material.GetFloat("_Gamma");
    }

    private void Update()
    {
        if (_curGamma <= MinGamma)
        {
            addFlag = 1;
        }
        if (_curGamma >= MaxGamma)
        {
            addFlag = -1;
        }
        _curGamma += Time.deltaTime * Speed * addFlag;
        _material.SetFloat("_Gamma", _curGamma);
    }
}