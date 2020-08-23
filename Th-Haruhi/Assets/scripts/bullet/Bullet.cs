using UnityEngine;

public class Bullet : EntityBase
{
    
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    protected GameObject Model { private set; get; }

    private float _startTime;
    private Vector3 _forward;
    private bool _bShooted;
    private bool _bNoLifeTime;


    public virtual void Init(BulletDeploy deploy, Transform master, GameObject model)
    {
        Deploy = deploy;
        Master = master;
        Model = model;
        ReInit(master);
    }

    public virtual void ReInit(Transform t)
    {
        Master = t;
    }

    public void Shoot(Vector3 startPos, Vector3 up, bool noLifeTime = false)
    {
        transform.position = startPos;
        transform.up = up;    
        _bNoLifeTime = noLifeTime;
        _startTime = Time.time;
        _forward = up;
        _bShooted = true;
    }

    protected override void Update()
    {
        if (InCache) return;
        if (!_bShooted) return;

        if (!_bNoLifeTime && Time.time - _startTime > Deploy.lifeTime)
        {
            BulletFactory.DestroyBullet(this);
            return;
        }

        transform.position += _forward * Time.deltaTime * Deploy.speed;
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        _bShooted = false;
        _forward = Vector3.zero;
        _startTime = 0;
    }
}


public class BulletDeploy : Conditionable
{
    public int id;
    public string classType;
    public int resourceId;
    public float lifeTime;
    public float speed;
    public float rota;
    public float scale;
    public float alpha;
    public bool bottomCenter;
}