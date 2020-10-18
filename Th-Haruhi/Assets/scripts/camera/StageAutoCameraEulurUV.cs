
using UnityEngine;

public class StageAutoCameraEulurUV : MonoBehaviour
{
    public float SpeedX;
    public float SpeedY;
    public float SpeedZ;

    private Vector3 _currentEuler;

    private void Awake()
    {
        _currentEuler = transform.eulerAngles;
    }

    private void Update()
    {
        _currentEuler.x += SpeedX;
        _currentEuler.y += SpeedY;
        _currentEuler.z += SpeedZ;
        transform.eulerAngles = _currentEuler;
    }
}