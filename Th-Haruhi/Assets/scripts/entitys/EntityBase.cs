using DG.Tweening;
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

public enum EShootSound
{
    Laser = 2006,
    Tan00 = 2003,
    Tan01 = 2004,
    Tan02 = 2005,
    Noraml = 2007,
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

    
    private Dictionary<EShootSound, int> ShootSoundCd = new Dictionary<EShootSound, int>();

    private int GetShootSoundCdFrame(EShootSound e)
    {
        switch (e)
        {
            case EShootSound.Laser:
                return 5;
            case EShootSound.Tan00:
            case EShootSound.Tan01:
            case EShootSound.Tan02:
                return 2;
            case EShootSound.Noraml:
                return 1;
        }
        return 0;
    }

    public void PlayShootSound(EShootSound sound, float volume = 1f)
    {
        if(ShootSoundCd.TryGetValue(sound, out int lastframe))
        {
            var cdFrame = GetShootSoundCdFrame(sound);
            if(GameSystem.FixedFrameCount - lastframe < cdFrame)
            {
                return;
            }
        }

        ShootSoundCd[sound] = GameSystem.FixedFrameCount;
        Sound.PlayUiAudioOneShot((int)sound, true, volume);
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


        var dx = Random.Range(xAmp.x, xAmp.y);
        var dy = Random.Range(yAmp.x, yAmp.y);

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

 
    protected virtual void Awake() 
    {
        CacheTransform = transform;
    }

    protected virtual void OnDestroy() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    public virtual void OnRecycle() 
    {
        InMove = false;
        MoveTarget = Vector3.zero;
    }

    public static void DestroyEntity(EntityBase b)
    {
        if(b != null && b.gameObject != null)
        {
            Destroy(b.gameObject);
        }
    }
}