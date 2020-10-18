
using UnityEngine;

public class StageCamera3D : MonoBehaviour
{
    public static StageCamera3D Instance;

    private void Awake()
    {
        Instance = this;

    }

    private void Update()
    {
        if(_inShake)
        {
            UpdateShake();
            return;
        }
    }

   

    private bool _inShake;
    private float _shakeElapsed;
    private float _shakeDuration;
    private float _shakeMagnitude;
    private Vector3 _originPos;

    public void Shake(float duration, float magnitude)
    {
        _originPos = transform.position;
        _inShake = true;
        _shakeElapsed = 0f;
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
    }

    private void UpdateShake()
    {
        if (!_inShake) return;

        if (_shakeElapsed < _shakeDuration)
        {
            float x = _originPos.x + Random.Range(-1f, 1f) * _shakeMagnitude;
            float y = _originPos.y + Random.Range(-1f, 1f) * _shakeMagnitude;
            transform.position = new Vector3(x, y, _originPos.z);
            _shakeElapsed += Time.deltaTime;
        }
        else
        {
            if (_inShake)
            {
                _inShake = false;
                _shakeElapsed = 0;
                _shakeDuration = 0;
                _shakeMagnitude = 0;
                transform.position = _originPos;
            }
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }
}