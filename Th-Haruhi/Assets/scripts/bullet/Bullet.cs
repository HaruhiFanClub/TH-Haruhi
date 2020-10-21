using DG.Tweening;
using System;
using System.Collections;
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
    public const float GrizeDistance = 0.25f;

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
    protected bool Shooted { private set; get; }

    public int Atk { protected set; get; }

    private int _totalFrame;

    private bool _isAni;

    public bool BoundDestroy = true;

    private Action<Bullet> _onDestroy;

    private int _updateCallBackCount;

    private int DefaultSortOrder;

    public static int TotalBulletCount;


    private void OnEnable()
    {
        TotalBulletCount++;
    }

    private void OnDisable()
    {
        TotalBulletCount--;
    }

    public virtual void Init(BulletDeploy deploy)
    {
        Deploy = deploy;
        _isAni = deploy.isAni;
        DefaultSortOrder = Renderer.sortingOrder;
        ReInit();
    }

    public virtual void ReInit()
    {
    }
 
    public virtual void Shoot(MoveData moveData, List<EventData> eventList = null, int atk = 1, Action<Bullet> onDestroy = null)
    {
        Atk = atk;

        _onDestroy = onDestroy;

        CacheTransform.position = moveData.StartPos;

        _totalFrame = 0;
        _updateCallBackCount = 0;

        EventList = eventList;
        MoveData = moveData;
        UpdateForward();

        Shooted = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (InCache) return;
        if (!Shooted) return;
        _totalFrame++;

        float delta = Time.deltaTime;
        UpdateAnimation();

        //update movement
        UpdateMoveByForward(delta);
        UpdateEventList();
    }

    protected bool CheckBulletOutSide(Vector3 bulletCenter)
    {
        //超出边界销毁
        if(BoundDestroy)
        {
            if (bulletCenter.x < -15f || bulletCenter.x > 5f || bulletCenter.y < -12f || bulletCenter.y > 12f)
            {
                BulletFactory.DestroyBullet(this);
                return true;
            }
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
                case EventData.EventType.Frame_Update:
                    {
                        if (_totalFrame >= e.FrameCount)
                        {
                            if(e.UpdateInterval == 0)
                            {
                                for(int t = 0; t < e.UpdateTimes; t++)
                                {
                                    _updateCallBackCount++;
                                    e.OnUpdate?.Invoke(this);
                                }
                            }
                            else if(_totalFrame % e.UpdateInterval == 0)
                            {
                                _updateCallBackCount++;
                                e.OnUpdate?.Invoke(this);
                            }

                            
                            if(e.UpdateTimes > 0 && _updateCallBackCount > e.UpdateTimes)
                            {
                                EventList.RemoveAt(i);
                            }
                        }
                    }
                    break;
                case EventData.EventType.Frame_Destroy:

                    if (_totalFrame >= e.FrameCount)
                    {
                        EventList.RemoveAt(i);
                        this.PlayEffectAndDestroy(502);
                    }
                    break;

                //狙击玩家
                case EventData.EventType.Frame_AimToPlayer:
                    if (_totalFrame >= e.FrameCount)
                    {
                        EventList.RemoveAt(i);
                       
                    }
                    break;

                //改变速度信息
                case EventData.EventType.Frame_ChangeSpeed:

                    if( _totalFrame >= e.FrameCount)
                    {
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
                        if(e.ForwardData.Forward != null)
                        {
                            MoveData.Forward = (Vector3)e.ForwardData.Forward;
                            UpdateForward();
                        }
                        EventList.RemoveAt(i);
                    }
                    break;
            }
        }
    }

    private void UpdateMoveByForward(float deltaTime)
    {
        //无移动方向不处理
        if (MoveData.Forward == Vector3.zero) 
        {
            return;
        }

        //Acceleration
        if (!MathUtility.FloatEqual(AccelerationX, 0f) || !MathUtility.FloatEqual(AccelerationY, 0f))
        {
            MoveData.SpeedX += AccelerationX * 5f;
            MoveData.SpeedY += AccelerationY * 5f;
        }

        var distX = deltaTime * MoveData.SpeedX * LuaStg.LuaStgSpeedChange;
        var distY = deltaTime * MoveData.SpeedY * LuaStg.LuaStgSpeedChange;

        var currPos = CacheTransform.transform.position;
        currPos.x += MoveData.Forward.x * distX;
        currPos.y += MoveData.Forward.y * distY;
        CacheTransform.position =  currPos;
    }


    public void SetMaster(Transform master)
    {
        Master = master;
    }

    public void SetAngle(float z)
    {
        MoveData.Forward = z.AngleToForward();
        UpdateForward();
    }

    private void UpdateForward()
    {
        if(MoveData.Forward != Vector3.zero)
        {
            CacheTransform.up = MoveData.Forward;
        }
    }

    public void SetBoundDestroy(bool b)
    {
        BoundDestroy = b;
    }

    public void AimToPlayer(float speed, float angleMin, float angleMax, bool setRot)
    {
        MoveData.Speed = speed;

        var player = StageMgr.MainPlayer;
        if (player != null)
        {
            var rndAngle = UnityEngine.Random.Range(angleMin, angleMax);
            var fwd = (player.transform.position - CacheTransform.position).normalized;
            fwd = Quaternion.Euler(0, 0, rndAngle) * fwd;
            MoveData.Forward = fwd;
            if(setRot)
            {
                UpdateForward();
            }
        }
    }

    public override void OnRecycle()
    {
        base.OnRecycle();

        SetBoundDestroy(true);

        EventList?.Clear();
        EventList = null;


        _onDestroy?.Invoke(this);
        _onDestroy = null;

        _currAniIdx = 0;
        _totalFrame = 0;
        Shooted = false;
        Renderer.sortingOrder = DefaultSortOrder;

        MoveData = null;
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
    public float bombEffectSpeed;
    public int centerPivot;
    public bool isAni;
    public bool isBoxCollider;
    public float sizeX;
    public float sizeY;
    public float brightness;
    public bool delayDestroy;
    public EColor EColor;
}