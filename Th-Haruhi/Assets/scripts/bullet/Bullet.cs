using System.Collections.Generic;
using UnityEngine;

public class Bullet : EntityBase
{
    public class ColliderInfo
    {
        public bool IsBox;
        public float Radius;
        public float BoxWidth;
        public float BoxHeight;
        public Vector3 Center;
    }

    //擦弹判定距离
    public const float GrizeDistance = 0.2f;

    //碰撞信息
    public ColliderInfo CollisionInfo;

    //移动数据
    public MoveData MoveData;

    //事件数据
    public List<EventData> EventList;

    public List<Sprite> AniList { set; get; }
    public override EEntityType EntityType => EEntityType.Bullet;
    public BulletDeploy Deploy { private set; get; }
    protected Transform Master { private set; get; }
    public MeshRenderer Renderer { private set; get; }
    protected Transform CacheTransform { private set; get; }
    protected bool Shooted { private set; get; }

    public int Atk { protected set; get; }

    private int _totalFrame;

    private int _lastHelixFrame;


    private float _movedDistance;


    private bool _isAni;

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
        _isAni = deploy.isAni;
        ReInit(master);
    }

    public virtual void ReInit(Transform t)
    {
        Master = t;
    }
 
    public virtual void Shoot(MoveData moveData, List<EventData> eventList = null, int atk = 1)
    {
        Atk = atk;

        CacheTransform = transform;
        CacheTransform.position = moveData.StartPos;

        _movedDistance = 0;
        _totalFrame = 0;
        _lastHelixFrame = 0;

        EventList = eventList;

        if (moveData != null)
        {
            MoveData = moveData;
            transform.up = MoveData.Forward;
        }

        Shooted = true;
    }

    protected override void FixedUpdate()
    {
        if (InCache) return;
        if (!Shooted) return;
        _totalFrame++;

        float delta = Time.deltaTime;
        UpdateAnimation();
        UpdateBulletMove(delta);
        UpdateEventList();
    }

    protected bool CheckBulletOutSide(Vector3 bulletCenter)
    {
        //超出边界销毁
        if (bulletCenter.x < -15f || bulletCenter.x > 5f || bulletCenter.y < -12f || bulletCenter.y > 12f)
        {
            BulletFactory.DestroyBullet(this);
            return true;
        }
        return false;
    }

    private int _currAniIdx;
    private float _lastAniFrame;

    private void UpdateAnimation()
    {
        if (_isAni && AniList != null)
        {
            if (_totalFrame - _lastAniFrame > 6)
            {
                _currAniIdx++;
                if (_currAniIdx >= AniList.Count)
                {
                    _currAniIdx = 0;
                }
                Renderer.material.mainTexture = AniList[_currAniIdx].texture;
                _lastAniFrame = _totalFrame;
            }
        }
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

                    if (_totalFrame >= e.FrameCount)
                    {
                        EventList.RemoveAt(i);
                        BulletFactory.DestroyBullet(this);
                    }
                    break;
                case EventData.EventType.Frame_ChangeSpeed:

                    if( _totalFrame >= e.FrameCount)
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

                    if (_totalFrame >= e.FrameCount)
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

    
    private void UpdateBulletMove(float deltaTime)
    {

        //螺旋移动
        if (MoveData.HelixToward != MoveData.EHelixToward.None) 
        {
            var eulurZ = (int)MoveData.HelixToward * MoveData.EulurPerFrame * deltaTime * 60f;
            MoveData.Forward = Quaternion.Euler(0, 0, eulurZ) * MoveData.Forward;
            CacheTransform.up = MoveData.Forward;

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
            MoveData.Speed += MoveData.Acceleration * deltaTime;
            if (!MathUtility.FloatEqual(MoveData.EndSpeed, 0) && Mathf.Abs(MoveData.EndSpeed - MoveData.Speed) < 0.1f)
            {
                MoveData.Speed = MoveData.EndSpeed;
            }
        }

        var dist = deltaTime * MoveData.Speed;
        _movedDistance += dist;
        CacheTransform.position += MoveData.Forward * dist;
    }


    public override void OnRecycle()
    {
        base.OnRecycle();
        _currAniIdx = 0;
        _movedDistance = 0;
        Shooted = false;
        _totalFrame = 0;
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
    public bool delayDestroy;
    public EColor ExplosionColor;
}