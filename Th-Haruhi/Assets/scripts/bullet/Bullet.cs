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
    public List<EventData> EventList { protected set; get; }
    public int ShootFrame { protected set; get; }

    private bool _bShooted;

    private float _movedDistance;

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

 
    public virtual void Shoot(MoveData moveData, List<EventData> eventList = null, int atk = 1)
    {
        Atk = atk;
        _movedDistance = 0;
        transform.position = moveData.StartPos;
        ShootFrame = 0;
        EventList = eventList;
        InitBulletMoveData(moveData);
        _bShooted = true;
    }

    private int _currAniIdx;
    private float _lastAniTime;
    private void UpdateAnimation()
    {
        if (Deploy.isAni && AniList != null)
        {
            if (Time.time - _lastAniTime > 0.1f)
            {
                _currAniIdx++;
                if (_currAniIdx >= AniList.Count)
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
        ShootFrame++;
        UpdateAnimation();
        UpdateBulletMove();
        UpdateEventList();
    }

 
    private void UpdateEventList()
    {
        if (EventList == null || EventList.Count == 0) return;

        for(int i = EventList.Count - 1; i >= 0; i--)
        {
            var e = EventList[i];
            switch (e.Type)
            {
                case EventData.EventType.Frame_Destroy:

                    if (ShootFrame >= e.FrameCount)
                    {
                        EventList.RemoveAt(i);
                        BulletFactory.DestroyBullet(this);
                    }
                    break;
                case EventData.EventType.Frame_ChangeSpeed:

                    if( ShootFrame >= e.FrameCount)
                    {
                        MoveData.EndSpeed = e.SpeedData.EndSpeed;
                        MoveData.Acceleration = e.SpeedData.Acceleration;

                        if (e.SpeedData.Speed > 0f)
                        {
                            MoveData.Speed = e.SpeedData.Speed;
                        }
                        EventList.RemoveAt(i);
                    }
                    break;
                case EventData.EventType.Frame_ChangeForward:

                    if (ShootFrame >= e.FrameCount)
                    {
                        MoveData.Forward = e.ForwardData.Forward;
                        MoveData.HelixRefretFrame = e.ForwardData.HelixRefretFrame;
                        MoveData.HelixToward = e.ForwardData.HelixToward;
                        MoveData.EulurPerFrame = e.ForwardData.EulurPerFrame;
                        EventList.RemoveAt(i);
                    }
                    break;
                case EventData.EventType.Distance_ChangeFoward:
                    if(_movedDistance >= e.Distance)
                    {
                        MoveData.Forward = e.ForwardData.Forward;
                        MoveData.HelixRefretFrame = e.ForwardData.HelixRefretFrame;
                        MoveData.HelixToward = e.ForwardData.HelixToward;
                        MoveData.EulurPerFrame = e.ForwardData.EulurPerFrame;
                        EventList.RemoveAt(i);
                    }
                    break;
            }
        }
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

            if (MoveData.HelixRefretFrame > 0 && _totalFrame - _lastHelixFrame >= MoveData.HelixRefretFrame)
            {
                _lastHelixFrame = _totalFrame;
                MoveData.HelixToward = MoveData.HelixToward == MoveData.EHelixToward.Right ? 
                                                MoveData.EHelixToward.Left : 
                                                MoveData.EHelixToward.Right;
            }
        }


        if (!MathUtility.FloatEqual(MoveData.Acceleration, 0f))
        {
            MoveData.Speed += MoveData.Acceleration * Time.deltaTime;
            if (!MathUtility.FloatEqual(MoveData.EndSpeed, 0) && Mathf.Abs(MoveData.EndSpeed - MoveData.Speed) < 0.1f)
            {
                MoveData.Speed = MoveData.EndSpeed;
            }
        }

        var dist = Time.deltaTime * MoveData.Speed;
        _movedDistance += dist;
        transform.position += MoveData.Forward * dist;
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
        _currAniIdx = 0;
        _movedDistance = 0;
        _bShooted = false;
        ShootFrame = 0;
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
    public bool isBoxCollider;
    public float sizeX;
    public float sizeY;
    public float brightness;
}