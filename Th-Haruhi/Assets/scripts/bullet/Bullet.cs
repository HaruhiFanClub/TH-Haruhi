using UnityEngine;

public class Bullet : EntityBase
{
    public override EEntityType EntityType => EEntityType.Bullet;
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    protected GameObject Model { private set; get; }
    public bool AutoDestroy { set; get; }

    private float _startTime;
    private Vector3 _forward;
    private bool _bShooted;

    public static int TotalBulletCount;
    private void OnEnable()
    {
        TotalBulletCount++;
    }

    private void OnDisable()
    {
        TotalBulletCount--;
    }

    public virtual void Init(BulletDeploy deploy, Transform master, GameObject model)
    {
        Deploy = deploy;
        Master = master;
        Model = model;
        AutoDestroy = true;
        ReInit(master);
    }

    public virtual void ReInit(Transform t)
    {
        Master = t;
    }

    public void Shoot(Vector3 startPos, Vector3 up)
    {
        transform.position = startPos;
        transform.up = up;    
        _startTime = Time.time;
        _forward = up;
        _bShooted = true;
    }

    private float _nextStartTime;
    private bool _inStop;

    protected override void Update()
    {
        if (InCache) return;
        if (!_bShooted) return;

        if (Deploy.eventTime > 0 && Time.time - _startTime > Deploy.eventTime)
        {
            if(_inStop)
            {
                if(Time.time > _nextStartTime)
                {
                    _inStop = false;
                    _startTime = Time.time;
                }
                return;
            }
            if(!_inStop && Deploy.eventWait > 0)
            {
                _inStop = true;
                _nextStartTime = Time.time + Deploy.eventWait;
                return;
            }
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
    public float speed;
    public float rota;
    public float scale;
    public float alpha;
    public float radius;
    public int spriteIdx;
    public bool bottomCenter;

    public float eventTime;
    public float eventWait;
}