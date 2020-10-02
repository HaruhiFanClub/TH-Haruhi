
using UnityEngine;

public class StageCamera3D : MonoBehaviour
{
    public float MinSec;
    public float MaxSec;
    public float MinRotaX;
    public float MaxRotaX;
    public float MinRotaZ;
    public float MaxRotaZ;
    public float Speed;
    private float _dufaultY;
    private float _nextTime;
    private bool _bInRota;


    public static StageCamera3D Instance;

    private void Awake()
    {
        _nextTime = Time.time + Random.Range(MinSec, MaxSec);
        _currentEuler = transform.eulerAngles;
        _dufaultY = _currentEuler.y;
        Instance = this;

    }

    private void Update()
    {
        if(_inShake)
        {
            UpdateShake();
            return;
        }

        if(_bInRota)
        {
            _currentEuler = Vector3.Lerp(_currentEuler, _targetEuler, Time.deltaTime * Speed); ;
            transform.eulerAngles = _currentEuler;
            if(Mathf.Abs(transform.eulerAngles.magnitude - _targetEuler.magnitude) < 1f)
            {
                _bInRota = false;
            }
            return;
        }

        if(Time.time > _nextTime)
        {
            GetNext();
        }
    }

    private Vector3 _targetEuler;
    private Vector3 _currentEuler;
    private void GetNext()
    {
        var targetX = Random.Range(MinRotaX, MaxRotaX);
        var targetZ = Random.Range(MinRotaZ, MaxRotaZ);
        _targetEuler = new Vector3(targetX, _dufaultY, targetZ);
        _bInRota = true;
        _nextTime = Time.time + Random.Range(MinSec, MaxSec);
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