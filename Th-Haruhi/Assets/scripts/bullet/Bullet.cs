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

    private int _lastHelixFrame;

    private float _movedDistance;

    private bool _isAni;

    public bool BoundDestroy = true;

    private Action<Bullet> _onDestroy;

    public List<Bullet> SonBullets = new List<Bullet>();

    private int _updateCallBackCount;

    private int DefaultSortOrder;

    public LuaStgTask MainTask;



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

        _movedDistance = 0;
        _totalFrame = 0;
        _lastHelixFrame = 0;
        _updateCallBackCount = 0;

        EventList = eventList;
        MoveData = moveData;
        UpdateForward();

        Shooted = true;


        MainTask = new LuaStgTask(this, 0, 1, -1, null, LuaStgTask.TaskExecuseType.Sequence);
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

        MainTask?.OnUpdate();
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
                        MoveData.Speed = e.AimToPlayerData.Speed;

                        var player = StageMgr.MainPlayer;
                        if (player != null) 
                        {
                            var fwd = (player.transform.position - CacheTransform.position).normalized;
                            fwd = Quaternion.Euler(0, 0, e.AimToPlayerData.Angle) * fwd;
                            MoveData.Forward = fwd;
                            UpdateForward();
                        }
                    }
                    break;

                //改变速度信息
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
                        if(e.ForwardData.Forward != null)
                        {
                            MoveData.Forward = (Vector3)e.ForwardData.Forward;
                            UpdateForward();
                        }
                       
                        MoveData.HelixRefretFrame = e.ForwardData.HelixRefretFrame;
                        MoveData.HelixToward = e.ForwardData.HelixToward;
                        MoveData.EulurPerFrame = e.ForwardData.EulurPerFrame;
                        EventList.RemoveAt(i);
                    }
                    break;
                case EventData.EventType.Distance_ChangeFoward:
                    if(_movedDistance >= e.Distance)
                    {
                        if (e.ForwardData.Forward != null)
                        {
                            MoveData.Forward = (Vector3)e.ForwardData.Forward;
                            UpdateForward();
                        }
                        MoveData.HelixRefretFrame = e.ForwardData.HelixRefretFrame;
                        MoveData.HelixToward = e.ForwardData.HelixToward;
                        MoveData.EulurPerFrame = e.ForwardData.EulurPerFrame;
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

        //螺旋移动
        if (MoveData.HelixToward != MoveData.EHelixToward.None) 
        {
            var eulurZ = (int)MoveData.HelixToward * MoveData.EulurPerFrame * deltaTime * 60f;
            MoveData.Forward = Quaternion.Euler(0, 0, eulurZ) * MoveData.Forward;
            UpdateForward();

            if (MoveData.HelixRefretFrame > 0 && _totalFrame - _lastHelixFrame >= MoveData.HelixRefretFrame)
            {
                _lastHelixFrame = _totalFrame;
                MoveData.HelixToward = MoveData.HelixToward == MoveData.EHelixToward.Right ? 
                                                MoveData.EHelixToward.Left : 
                                                MoveData.EHelixToward.Right;
            }
        }

        //角度加速度
        if(MoveData.AngleData != null)
        {
            MoveData.Forward = Vector3.Lerp(MoveData.Forward, MoveData.AngleData.TargetAngle.AngleToForward(), deltaTime);
            UpdateForward();
        }


        //速度加速度
        if (!MathUtility.FloatEqual(MoveData.Acceleration, 0f))
        {
            MoveData.Speed += MoveData.Acceleration * LuaStg.LuaStgSpeedChange * deltaTime;
            if (!MathUtility.FloatEqual(MoveData.EndSpeed, 0) && Mathf.Abs(MoveData.EndSpeed - MoveData.Speed) < 0.1f)
            {
                MoveData.Speed = MoveData.EndSpeed;
            }
        }

        var dist = deltaTime * MoveData.Speed * LuaStg.LuaStgSpeedChange;
        _movedDistance += dist;
        CacheTransform.position += MoveData.Forward * dist;
    }


    public void SetMaster(Transform master)
    {
        Master = master;
    }

    public void SetForward(float z)
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


    public override void OnRecycle()
    {
        base.OnRecycle();

        MainTask = null;

        SetBoundDestroy(true);

        EventList?.Clear();
        EventList = null;

        for (int i = 0; i < SonBullets.Count; i++)
        {
            if(!SonBullets[i].InCache)
                BulletFactory.DestroyBullet(SonBullets[i]);
        }
        SonBullets.Clear();

        _onDestroy?.Invoke(this);
        _onDestroy = null;

        _currAniIdx = 0;
        _movedDistance = 0;
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