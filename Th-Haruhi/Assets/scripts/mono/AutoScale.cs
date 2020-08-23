
using UnityEngine;
using DG.Tweening;

public class AutoScale : MonoBehaviour
{
    private float _curScale;
    public float MaxScale = 1f;
    public float MinScale = 0.8f;
    public float Speed = 1f;

    private int addFlag;

    private Vector3 _defaultScale;
    private void Awake()
    {
        _defaultScale = transform.localScale;
    }

    private void Update()
    {
        if(_curScale <= MinScale)
        {
            addFlag = 1;
        }
        if(_curScale >= MaxScale)
        {
            addFlag = -1;
        }
        _curScale += Time.deltaTime * Speed * addFlag;
        transform.localScale = _defaultScale * _curScale;
    }
}