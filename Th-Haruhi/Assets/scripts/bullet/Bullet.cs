using UnityEngine;

public class Bullet : EntityBase
{
    public override EEntityType EntityType => EEntityType.Bullet;
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    protected MeshRenderer Renderer { private set; get; }
    public bool AutoDestroy { set; get; }
    public float BulletSpeed { protected set; get; }
    public MoveData MoveData { protected set; get; }


    private float _startTime;
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

    public virtual void Init(BulletDeploy deploy, Transform master, MeshRenderer renderer)
    {
        Deploy = deploy;
        Master = master;
        Renderer = renderer;
       
        AutoDestroy = true;
        ReInit(master);
    }

    public virtual void ReInit(Transform t)
    {
        Master = t;
        BulletSpeed = Deploy.speed;
    }

    public virtual void Shoot(MoveData moveData)
    {
        transform.position = moveData.StartPos;
        _startTime = Time.time;
        InitBulletMoveData(moveData);
        _bShooted = true;
    }

    private float _nextStartTime;
    private bool _inStop;

    protected override void FixedUpdate()
    {
        if (InCache) return;
        if (!_bShooted) return;

        if (Deploy.eventTime > 0 && Time.time - _startTime > Deploy.eventTime)
        {
            if (_inStop)
            {
                if (Time.time > _nextStartTime)
                {
                    _inStop = false;
                    _startTime = Time.time;
                }
                return;
            }
            if (!_inStop && Deploy.eventWait > 0)
            {
                _inStop = true;
                _nextStartTime = Time.time + Deploy.eventWait;
                return;
            }
        }
        UpdateBulletMove();
    }

 

    private void InitBulletMoveData(MoveData data)
    {
        if (data == null) return;
        MoveData = data;
        transform.up = MoveData.Forward;

        _totalFrame = 0;
        _lastHelixFrame = 0;
    }

    private int _totalFrame;
    private float _lastHelixFrame;
    private void UpdateBulletMove()
    {
        _totalFrame += 1;

        //螺旋移动
        if (MoveData.HelixToward != MoveData.EHelixToward.None) 
        {
            var eulurZ = (int)MoveData.HelixToward * MoveData.EulurPerFrame * Time.deltaTime * 60f;
            MoveData.Forward = Quaternion.Euler(0, 0, eulurZ) * MoveData.Forward;
            transform.up = MoveData.Forward;

            if (_totalFrame - _lastHelixFrame >= MoveData.HelixRefretFrame)
            {
                _lastHelixFrame = _totalFrame;
                MoveData.HelixToward = MoveData.HelixToward == MoveData.EHelixToward.Right ? 
                                                MoveData.EHelixToward.Left : 
                                                MoveData.EHelixToward.Right;
            }
        }
        transform.position += MoveData.Forward.normalized * Time.deltaTime * BulletSpeed;
    }

    public virtual void OnBulletHitEnemy()
    {
        if (!InCache)
        {
            PlayBombEffect(transform.position);
            BulletFactory.DestroyBullet(this);
        }
    }

    private void PlayBombEffect(Vector3 pos)
    {
        if (Deploy.bombEffectId > 0)
        {
            TextureEffectFactroy.CreateEffect(Deploy.bombEffectId, SortingOrder.EnemyBullet, effect =>
            {
                effect.transform.position = pos;
                effect.AutoDestroy();
            });
        }
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        _bShooted = false;
        _startTime = 0;
        Pool.Free(MoveData);
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
    public int atk;
    public int bombEffectId;
    public int centerPivot;

    public float eventTime;
    public float eventWait;
}