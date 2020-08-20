using UnityEngine;


public class RainCollision : MonoBehaviour
{
    private static readonly Color32 Color = new Color32(255, 255, 255, 255);

    public ParticleSystem RainRipple;
    public ParticleSystem RainParticleSystem;
    public ParticleSystem RainSplash;
    public int MinCount = 1;
    public int MaxCount = 3;

    private bool _inited;

    void Start()
    {
        if (RainRipple != null && RainSplash != null && RainParticleSystem != null)
        {
            _inited = true;
        }
        else
        {
        }
    }
    private void Emit(ParticleSystem p, ref Vector3 pos, bool isSplash)
    {
       
        ParticleSystem.EmitParams param = new ParticleSystem.EmitParams();
        if (isSplash)
        {
            float yVelocity = Random.Range(1.0f, 3.0f);
            float zVelocity = Random.Range(-2.0f, 2.0f);
            float xVelocity = Random.Range(-2.0f, 2.0f);
            const float lifetime = 0.8f;// UnityEngine.Random.Range(0.25f, 0.75f);
            float size = Random.Range(0.05f, 0.1f);
            param.velocity = new Vector3(xVelocity, yVelocity, zVelocity);
            param.startLifetime = lifetime;
            param.startSize = size;
        }
        pos.y += 0.1f;
        param.position = pos;
        param.startColor = Color;
        p.Emit(param, 1);
       
    }

    void Update()
    {
        if (_inited && RainParticleSystem.emission.rateOverTimeMultiplier > 0.05f)
        {
            int count = Random.Range(MinCount, MaxCount);
            for (int i = 0; i < count; i++)
            {
                var pos = transform.position;
                pos.x += Random.Range(-20, 20f);
                pos.z += Random.Range(-20f, 20f);
                pos += transform.root.forward * 20f;

                RaycastHit hit;
                if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 100f))
                {
                    var obj = hit.transform.gameObject;
                    var newPos = hit.point;

                    Emit(RainSplash, ref newPos, true);
                }
            }
        }

    }
}