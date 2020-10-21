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
    public abstract EEntityType EntityType { get; }

    private Rigidbody2D _rigidBody2d;

    protected Transform CacheTransform { private set; get; }

    public bool InMove { private set; get; }

    public Vector3 MoveTarget { private set; get; }

    public bool InCache { private set; get; }
    public void SetInCache(bool b)
    {
        InCache = b;
    }

    public Renderer Renderer { private set; get; }
    public virtual void SetRenderer(Renderer r)
    {
        Renderer = r;
    }

    #region rigidBody
    public Rigidbody2D Rigid2D
    {
        get
        {
            InitRigid();
            return _rigidBody2d;
        }
    }

    public void InitRigid()
    {
        if (_rigidBody2d == null)
        {
            var r = GetComponent<Rigidbody2D>();
            if (r != null)
            {
                _rigidBody2d = r;
            }
            else
            {
                _rigidBody2d = gameObject.AddComponent<Rigidbody2D>();

                if (EntityType == EEntityType.Player)
                {
                    _rigidBody2d.bodyType = RigidbodyType2D.Dynamic;
                    _rigidBody2d.simulated = true;
                    _rigidBody2d.useAutoMass = false;
                    _rigidBody2d.mass = 10;
                    _rigidBody2d.drag = 30;
                    _rigidBody2d.gravityScale = 0f;
                    _rigidBody2d.angularDrag = 0f;
                }
                else
                {
                    _rigidBody2d.bodyType = RigidbodyType2D.Kinematic;
                }
                _rigidBody2d.freezeRotation = true;
            }
        }
    }

    #endregion


    public float Rot
    {
        get { return CacheTransform.eulerAngles.z; }
        set
        {
            CacheTransform.up = value.AngleToForward();
        }
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
    public void SetSelfRota(float anglePerFrame)
    {
        _inSelfRota = true;
        _selfRotaAngle = anglePerFrame;
    }

    public void RevertSelfRota()
    {
        _inSelfRota = false;
        _selfRotaAngle = 0;
        CacheTransform.localEulerAngles = Vector3.zero;
    }

    private void UpdateLocalRota()
    {
        if (_inSelfRota)
        {
            var localEuler = CacheTransform.localEulerAngles;
            localEuler.z += _selfRotaAngle;
            CacheTransform.localEulerAngles = localEuler;
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


    //angelSpeed
    public float AccelerationX;        //加速度X
    public float AccelerationY;        //加速度Y

    public void SetAcceleration(float accleration, float rot, bool aimPlayer = false)
    {
        if (aimPlayer)
        {
            rot += LuaStg.AnglePlayer(transform);
        }
        AccelerationX = accleration * LuaStg.Cos(rot);
        AccelerationY = accleration * LuaStg.Sin(rot);
    }

    public void RevertAcceleration()
    {
        AccelerationX = 0;
        AccelerationY = 0;
    }

    public void SetPos(float stgPosX, float stgPosY)
    {
        if(Father != null)
        {
            CacheTransform.position = Father.CacheTransform.position + Vector2Fight.NewLocal(stgPosX, stgPosY);
        }
        else
        {
            CacheTransform.position = Vector2Fight.NewWorld(stgPosX, stgPosY);
        }
    }


    public List<EntityBase> SonEntitys = new List<EntityBase>();
    public EntityBase Father;

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
        UpdateLocalRota();
        UpdateSmear();
    }

    public virtual void OnRecycle()
    {
        DestoryAllSons();
        DestroyAllTask();
        RevertAcceleration();
        RevertBanCollision();
        RevertHidden();
        RevertSmear();
        RevertHighLight();
        RevertSelfRota();
        InMove = false;
        MoveTarget = Vector3.zero;
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