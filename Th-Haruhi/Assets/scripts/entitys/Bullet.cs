using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float LifeTime;
    public float Speed;
    
    private float _startTime;
    private Vector3 _forward;
    private bool _inited;


    public void Shoot(Vector3 startPos, Vector3 forward)
    {
        transform.position = startPos;
        _startTime = Time.time;
        _forward = forward;
        _inited = true;
    }

    private void Update()
    {
        if (!_inited) return;

        if(Time.time - _startTime > LifeTime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += _forward * Time.deltaTime * Speed;
    }
}