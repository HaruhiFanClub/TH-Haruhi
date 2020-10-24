using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEntityType
{
    Player,
    Enemy,
    Effect,
    Bullet
}

public abstract class EntityBase : MonoBehaviour
{
    //entity类型
    public abstract EEntityType EntityType { get; }

    //transform缓存(效率)
    protected Transform CacheTransform { private set; get; }

    //是否在池中(效率)
    public bool InCache { private set; get; }
    public void SetInCache(bool b)
    {
        InCache = b;
    }

    //Renderer
    public Renderer Renderer { private set; get; }
    public virtual void SetRenderer(Renderer r)
    {
        Renderer = r;
    }

    //移动状态
    public bool InMove;

    //当前移动目标点
    public Vector3 MoveTarget;

    //移動角度
    public float Angle;

    //移動速度
    public float VelocityX;
    public float VelocityY;

    //加速度
    public float AccelX;       
    public float AccelY;

    //根据移动方向自动进行rot旋转
    public bool Navi;

    //父子绑定关系
    public List<EntityBase> SonEntitys = new List<EntityBase>();
    public EntityBase Father;


    //当前位置(战斗坐标)
    public Vector2 Pos
    {
        get
        {
            return CacheTransform.position;
        }
    }


    //当前朝向
    public float Rot
    {
        get { return CacheTransform.eulerAngles.z; }
        set
        {
            var localEuler = CacheTransform.eulerAngles;
            localEuler.z = value;
            CacheTransform.eulerAngles = localEuler;
        }
    }



    //移动行为
    public void SetVelocity(float velocity, float angle, bool aimPlayer, bool setRot)
    {
        if (aimPlayer)
        {
            angle += LuaStg.AnglePlayer(transform);
        }

        Angle = angle;
        VelocityX = velocity * LuaStg.Cos(angle + 90);
        VelocityY = velocity * LuaStg.Sin(angle + 90);

        if (setRot)
        {
            Rot = angle;
        }
    }

    //acceleration
    public void SetAcceleration(float accleration, float angle, bool aimPlayer)
    {
        if (aimPlayer)
        {
            angle += LuaStg.AnglePlayer(transform);
        }
        AccelX = accleration * LuaStg.Cos(angle + 90);
        AccelY = accleration * LuaStg.Sin(angle + 90);
    }

    //根据朝向移动
    private void UpdateMove()
    {
        //根据移动角度算出移动的X距离和Y距离(Angle默认要+90, LuaStg默认是右)
        var distX = VelocityX; 
        var distY = VelocityY;

        //加速度
        VelocityX += AccelX;
        VelocityY += AccelY;

        var currPos = CacheTransform.position;
        currPos.x += distX;
        currPos.y += distY;

        var fwd = currPos - CacheTransform.position;
        CacheTransform.position = currPos;

        if (Navi) 
        {
            transform.up = fwd;
        }
    }

    //移動到指定坐標
    public void MoveToPos(Vector3 target, int t, MovementMode mode, bool setRotation = false)
    {
        StartCoroutine(MoveTo(target, t, mode, setRotation));
    }

    public IEnumerator MoveTo(Vector3 target, int t, MovementMode mode, bool setRotation)
    {
        InMove = true;
        MoveTarget = target;

        var diff = target - CacheTransform.position;

        if (setRotation) 
        {
            CacheTransform.up = diff;
        }
        
        float prevDist = 0f;

        for (float s = 1f / t; s < 1f + 0.5f / t; s += 1f / t)
        {
            float dist = s;
            switch (mode)
            {
                case MovementMode.MOVE_ACCEL:
                    dist = s * s;
                    break;
                case MovementMode.MOVE_DECEL:
                    dist = s * 2 - s * s;
                    break;
                case MovementMode.MOVE_ACC_DEC:
                    dist = s < 0.5f ? s * s * 2 : -2 * s * s + 4 * s - 1;
                    break;
            }

            CacheTransform.position += (dist - prevDist) * diff;
            yield return Yielders.FixedFrame;
            prevDist = dist;
        }
        InMove = false;
    }

    //向Player移動, 怪物巡邏用
    public IEnumerator MoveToPlayer(int frame, Vector2 xRange, Vector2 yRange, Vector2 xAmp, Vector2 yAmp, MovementMode mMode, DirectionMode dMode)
    {
        var dirX = LuaStg.RandomSign();
        var dirY = LuaStg.RandomSign();
        var player = StageMgr.MainPlayer;
        if (player == null) yield break;

        var selfX = CacheTransform.position.x;
        var selfY = CacheTransform.position.y;
        var playerX = player.CacheTransform.position.x;
        var playerY = player.CacheTransform.position.y;

        if ((int)dMode < 2)
        {
            dirX = selfX > playerX ? -1 : 1;
        }
        if((int)dMode == 0 || (int)dMode == 2)
        {
            dirY = selfY > playerY ? -1 : 1;
        }


        var dx = UnityEngine.Random.Range(xAmp.x, xAmp.y);
        var dy = UnityEngine.Random.Range(yAmp.x, yAmp.y);

        if (selfX + dx * dirX < xRange.x)
        {
            dirX = 1;
        }
        if (selfX + dx * dirX > xRange.y)
        {
            dirX = -1;
        }
        if(selfY + dy * dirY < yRange.x)
        {
            dirY = 1;
        }
        if(selfY + dy * dirY > yRange.y)
        {
            dirY = -1;
        }

        yield return MoveTo(new Vector3(selfX + dx * dirX, selfY + dy * dirY), frame, mMode, false);
    }

    //自旋转
    private bool _inSelfRota;
    private float _selfRotaAngle;
    public void SetOmiga(float anglePerFrame)
    {
        _inSelfRota = true;
        _selfRotaAngle = anglePerFrame;
    }

    public void RevertOmiga()
    {
        _inSelfRota = false;
        _selfRotaAngle = 0;
        CacheTransform.eulerAngles = Vector3.zero;
    }

    private void UpdateLocalRota()
    {
        if (_inSelfRota)
        {
            var localEuler = CacheTransform.eulerAngles;
            localEuler.z += _selfRotaAngle;
            CacheTransform.eulerAngles = localEuler;
        }
    }

    //高亮
    private bool _bChangedShader;
    public void SetShaderAdditive()
    {
        if(!_bChangedShader)
        {
            _bChangedShader = true;
            var m = Renderer.material;
            m.shader = GameSystem.DefaultRes.AlphaAdditive;
        }
    }

    public void RevertShader()
    {
        if (_bChangedShader)
        {
            Renderer.material.shader = GameSystem.DefaultRes.AlphaBlended;
            _bChangedShader = false;
        }
    }

    
    private Tweener _turnOffTween;
    public void FadeOut(int frame)
    {
        FadeOutAllSons(frame);
        DoFadeOut(frame);
    }

    public virtual void DoFadeOut(int frame)
    {
        _bChangedShader = true;
        _turnOffTween?.Kill();
        _turnOffTween = Renderer.material.DOFloat(0f, "_AlphaScale", frame * 0.01666f);
        _turnOffTween.onComplete = () =>
        {
            RecycleEnetity();
        };
    }

    //拖尾
    private bool _inSmear;
    private float _smearFrame;
    private int _smearStartFrame;
    public void SetSmear(float interval)
    {
        _inSmear = true;
        _smearFrame = interval;
        _smearStartFrame = GameSystem.FixedFrameCount;
    }

    public void RevertSmear()
    {
        _inSmear = false;
        _smearFrame = 0;
    }

    private void UpdateSmear()
    {
        if (!_inSmear) return;

        if((GameSystem.FixedFrameCount - _smearStartFrame) % _smearFrame == 0)
        {
            var gameObj = Instantiate(Renderer.gameObject);
            gameObj.transform.position = CacheTransform.position;
            gameObj.transform.DOScale(0f, 0.5f).onComplete = ()=>
            {
                Destroy(gameObj);
            };
        }
    }

    //coll
    public bool InBanCollision { private set; get; }
    public void SetBanCollision()
    {
        InBanCollision = true;
    }

    public void RevertBanCollision()
    {
        InBanCollision = false;
    }

    //hidden
    public bool InHidden { private set; get; }
    public void SetHidden()
    {
        InHidden = true;
        Renderer.enabled = false;
    }
    public void RevertHidden()
    {
        InHidden = false;
        Renderer.enabled = true;
    }

    public void SetRelativePosition(float x, float y, float rot)
    {
        if (Father != null)
        {
            CacheTransform.position = Father.CacheTransform.position + new Vector3(x, y);
        }
        else
        {
            CacheTransform.position = new Vector3(x, y);
        }

        Rot = rot;
    }


    public void SetFather(EntityBase father)
    {
        if (father == null) return;
        Father = father;
        father.SonEntitys.Add(this);
    }

    public void FadeOutAllSons(int frame)
    {
        for (int i = 0; i < SonEntitys.Count; i++)
        {
            SonEntitys[i].Father = null;
            if (!SonEntitys[i].InCache)
            {
                SonEntitys[i].FadeOut(frame);
            }
        }
        SonEntitys.Clear();
    }

    public void DestoryAllSons()
    {
        for (int i = 0; i < SonEntitys.Count; i++)
        {
            SonEntitys[i].Father = null;
            if (!SonEntitys[i].InCache)
            {
                SonEntitys[i].RecycleEnetity();
            }
        }
        SonEntitys.Clear();
    }


    protected virtual void Awake() 
    {
        CacheTransform = transform;
    }

    private void Update()
    {
        if (!InCache && !GamePause.InPause)
        {
            OnUpdate();
        }
    }

    private void LateUpdate() 
    {
        if (!InCache && !GamePause.InPause)
        {
            OnLateUpdate();
        }
    }

    private void FixedUpdate() 
    {
        if (!InCache && !GamePause.InPause)
        {
            UpdateLocalRota();
            UpdateSmear();
            UpdateMove();
            OnFixedUpdate();
        }
    }

    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnFixedUpdate() { }

    public virtual void OnRecycle()
    {
        this.RemoveAllTask();

        VelocityX = 0;
        VelocityY = 0;
        AccelX = 0;
        AccelY = 0;
        Angle = 0;
        Rot = 0;
        InMove = false;
        Navi = false;
        MoveTarget = Vector3.zero;

        _turnOffTween?.Kill();
        _turnOffTween = null;

        DestoryAllSons();
        DestroyAllTask();
        RevertBanCollision();
        RevertHidden();
        RevertSmear();
        RevertShader();
        RevertOmiga();
    }

    private void DestroyAllTask()
    {
        var tasks = GetComponents<LuaStgTask>();
        for(int i = 0; i < tasks.Length; i++)
        {
            Destroy(tasks[i]);
        }
    }

    public static void DestroyEntity(EntityBase b)
    {
        if(b != null && b.gameObject != null)
        {
            Destroy(b.gameObject);
        }
    }

    public void RecycleEnetity()
    {
        switch (EntityType)
        {
            case EEntityType.Player:
                DestroyEntity(this);
                break;
            case EEntityType.Enemy:
                DestroyEntity(this);
                break;
            case EEntityType.Effect:
                TextureEffectFactroy.DestroyEffect((TextureEffect)this);
                break;
            case EEntityType.Bullet:
                BulletFactory.DestroyBullet((Bullet)this);
                break;
        }
    }

    protected virtual void OnDestroy() { }
}