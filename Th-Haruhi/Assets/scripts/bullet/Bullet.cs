using System.Collections.Generic;
using UnityEngine;

public class Bullet : EntityBase
{
    public List<Sprite> AniList { set; get; }
    public override EEntityType EntityType => EEntityType.Bullet;
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    public MeshRenderer Renderer { private set; get; }
    public bool AutoDestroy { set; get; }
    public int Atk { protected set; get; }
    public MoveData MoveData { protected set; get; }
    public float ShootTime { protected set; get; }

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
    }

    public virtual void Shoot(MoveData moveData, int atk)
    {
        Atk = atk;
        transform.position = moveData.StartPos;
        ShootTime = Time.time;
        InitBulletMoveData(moveData);
        _bShooted = true;
    }

    private int _currAniIdx;
    private float _lastAniTime;
    protected override void Update()
    {
        base.Update();
        if(Deploy.isAni && AniList != null)
        {
            if(Time.time - _lastAniTime > 0.1f)
            {
                _currAniIdx++;
                if(_currAniIdx >= AniList.Count)
                {
                    _currAniIdx = 0;
                }
                Renderer.material.mainTexture = AniList[_currAniIdx].texture;
                _lastAniTime = Time.time;
            }
        }
    }
    protected override void FixedUpdate()
    {
        if (InCache) return;
        if (!_bShooted) return;

        /*
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
        }*/
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
        transform.position += MoveData.Forward.normalized * Time.deltaTime * MoveData.Speed;
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
        ShootTime = 0;
        Pool.Free(MoveData);
    }
}


public class BulletDeploy : Conditionable
{
    public int id;
    public string classType;
    public int resourceId;
    public float rota = 90f;
    public float scale;
    public float alpha;
    public float radius;
    public int spriteIdx;
    public int bombEffectId;
    public int centerPivot;
    public bool isAni;
}