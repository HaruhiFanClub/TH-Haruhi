
using UnityEngine;

public class StageAutoCamera : MonoBehaviour
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

    private void Awake()
    {
        _nextTime = Time.time + Random.Range(MinSec, MaxSec);
        _currentEuler = transform.eulerAngles;
        _dufaultY = _currentEuler.y;

    }

    private void Update()
    {
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
}