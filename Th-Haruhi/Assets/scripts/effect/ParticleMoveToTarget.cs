

using UnityEngine;
[ExecuteInEditMode]

public class ParticleMoveToTarget : MonoBehaviour
{
    public Transform Target;  //目标位置.(手动拖拽)
    public float Speed;

    private Transform _transform;
    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;
    private Vector3 _pos;            //粒子移动的目标位置.

    void Start()
    {
        _transform = gameObject.GetComponent<Transform>();
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];  //实例化，个数为粒子系统设置的最大粒子数.

        _pos = _transform.InverseTransformPoint(Target.position); //粒子系统模拟空间设置为Local时，需要把目标位置换成粒子系统的本地坐标.
        //pos = target_Trans.position;  //粒子系统模拟空间为World时，直接把目标位置赋值给pos.
    }

    void Update()
    {
        //获取当前激活的粒子.
        int num = _particleSystem.GetParticles(_particles);
        var delta = Time.deltaTime;

        //设置粒子移动.
        for (int i = 0; i < num; i++)
        {
            if(MathUtility.SqrDistance(_pos , _particles[i].position) > 0.2f)
            {
                 var forward = (_pos - _particles[i].position).normalized;
                _particles[i].position += forward * Speed * delta;
            }
            else
            {
                _particles[i].color = new Color32(0, 0, 0, 0);
            }
        }

        //重新赋值粒子.
        _particleSystem.SetParticles(_particles, num);
    }
}