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
    public bool InMove { private set; get; }

    //当前移动目标点
    public Vector3 MoveTarget { private set; get; }

    //当前移动朝向
    private Vector3 MoveFoward;

    private float _angle;
    protected float Angle
    {
        get { return _angle; }
        set
        {
            _angle = value;
            MoveFoward = value.AngleToForward();
        }
    }

    //移动速度
    private float SpeedX;
    private float SpeedY;

    protected float Velocity
    {
        set { SpeedX = SpeedY = value; }
    }

    //加速度
    public float AccelerationX;       
    public float AccelerationY;

    //父子绑定关系
    public List<EntityBase> SonEntitys = new List<EntityBase>();
    public EntityBase Father;


    //当前位置(战斗坐标)
    public Vector2 Pos
    {
        get
        {
            return Vector2Fight.WorldPosToFightPos(CacheTransform.position);
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

    //根据朝向移动
    private void UpdateMove()
    {
        if(SpeedX <= 0 || SpeedY <= 0)
        {
            return;
        }
        if (MoveFoward == Vector3.zero) 
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        //加速度处理
        if (!MathUtility.FloatEqual(AccelerationX, 0f) || !MathUtility.FloatEqual(AccelerationY, 0f))
        {
            SpeedX += SpeedX * AccelerationX;
            SpeedY += SpeedY * AccelerationY;
        }
        
        var distX = deltaTime * SpeedX * LuaStg.LuaStgSpeedChange;
        var distY = deltaTime * SpeedY * LuaStg.LuaStgSpeedChange;

        var currPos = CacheTransform.position;
        currPos.x += MoveFoward.x * distX;
        currPos.y += MoveFoward.y * distY;
        CacheTransform.position = currPos;
    }


    //移動到指定坐標
    public void MoveToPos(Vector3 target, int t, MovementMode mode, bool setRotation = false)
    {
        StartCoroutine(MoveTo(target, t, mode, setRotation));
    }

    private IEnumerator MoveTo(Vector3 target, int t, MovementMode mode, bool setRotation)
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
            yield return new WaitForFixedUpdate();
            prevDist = dist;
        }
        InMove = false;
    }

    //向Player移動, 怪物巡邏用
    public void MoveToPlayer(int frame, Vector2 xRange, Vector2 yRange, Vector2 xAmp, Vector2 yAmp, MovementMode mMode, DirectionMode dMode)
    {
        var dirX = LuaStg.RandomSign();
        var dirY = LuaStg.RandomSign();
        var player = StageMgr.MainPlayer;
        if (player == null) return;

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

        MoveToPos(new Vector3(selfX + dx * dirX, selfY + dy * dirY), frame, mMode);
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
    private float _defaultBrightness = 1;
    private float _defaultGamma = 1;
    private float _defaultAlpha = 1;
    private bool _bChangedBrightness;
    public void SetHighLight()
    {
        var m = Renderer.material;
        if (!_bChangedBrightness)
        {
            _bChangedBrightness = true;
            _defaultBrightness = m.GetFloat("_Brightness");
            _defaultGamma = m.GetFloat("_Gamma");
            _defaultAlpha = m.GetFloat("_AlphaScale");
        }
        m.SetFloat("_Brightness", 4f);
        m.SetFloat("_Gamma", 0.8f);
        m.SetFloat("_AlphaScale", 0.3f);
    }

    public void RevertHighLight()
    {
        if (_bChangedBrightness)
        {
            var m = Renderer.material;
            m.SetFloat("_Brightness", _defaultBrightness);
            m.SetFloat("_Gamma", _defaultGamma);
            m.SetFloat("_AlphaScale", _defaultAlpha);
            _bChangedBrightness = false;
        }
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
            CacheTransform.position = Father.CacheTransform.position + Vector2Fight.NewLocal(x, y);
        }
        else
        {
            CacheTransform.position = Vector2Fight.NewWorld(x, y);
        }

        Rot = rot;
    }

    //移动行为
    public void SetVelocity(float velocity, float angle, bool aimPlayer, bool setRot)
    {
        if (aimPlayer)
        {
            angle += LuaStg.AnglePlayer(transform);
        }

        Angle = angle;
        SpeedX = velocity;
        SpeedY = velocity;

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
        AccelerationX = accleration * LuaStg.Cos(angle);
        AccelerationY = accleration * LuaStg.Sin(angle);
    }

    public void SetFather(EntityBase father)
    {
        if (father == null) return;
        Father = father;
        father.SonEntitys.Add(this);
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

    protected virtual void OnDestroy()
    {
        
    }

    protected virtual void Update() { }
    protected virtual void FixedUpdate() 
    {
        if (InCache) return;
        UpdateLocalRota();
        UpdateSmear();
        UpdateMove();
    }

    public virtual void OnRecycle()
    {
        MoveFoward = Vector3.zero;
        SpeedX = 0;
        SpeedY = 0;
        AccelerationX = 0;
        AccelerationY = 0;
        Angle = 0;
        Rot = 0;
        InMove = false;
        MoveTarget = Vector3.zero;

        DestoryAllSons();
        DestroyAllTask();
        RevertBanCollision();
        RevertHidden();
        RevertSmear();
        RevertHighLight();
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
}