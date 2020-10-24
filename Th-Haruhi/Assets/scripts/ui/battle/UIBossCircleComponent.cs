
using UnityEngine;
using DG.Tweening;

public class UIBossCircleComponent : MonoBehaviour
{
    public Renderer RaoDong;
    private float _curX;
    private float _curY;
    private float _curZ;

    private int _xForward = -1;
    private int _yForward = 1;
    public float TurnSpeed = 30f;
    public float TurnSpeedX = 10f;

    private Vector3 _originScale;

    private void Awake()
    {
        _originScale = transform.localScale;
    }

    public void TurnOn()
    {
        gameObject.SetActiveSafe(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(_originScale, 1f);
    }

    private void Update()
    {
        var delta = Time.deltaTime;

        _curX += delta * _xForward * TurnSpeedX;
      
        if (_curX < -50)
            _xForward = 1;
        if (_curX > 0)
            _xForward = -1;

        _curY += delta * _yForward * TurnSpeedX;

        if (_curY > 25)
            _yForward = -1;
        if (_curY < -25)
            _yForward = 1;

       
        _curZ += delta * TurnSpeed;
        if (_curZ > 360) _curZ = 0;


        var target = new Vector3(_curX, _curY, _curZ);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(target), delta);
    }
}
