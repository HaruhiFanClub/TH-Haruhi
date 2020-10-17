using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : EntityBase
{
    public enum EnemyMoveStyle
    {
        Idle,
        Move,
        MoveIdle,
    }

    public override EEntityType EntityType => EEntityType.Enemy;
    public Dictionary<EnemyMoveStyle, List<Sprite>> SpriteDic = new Dictionary<EnemyMoveStyle, List<Sprite>>();
    
    //是否无敌
    public bool Invisible { set; get; }

    //血量
    public int HP { private set; get; }
    public int HPMax { private set; get; }
  
    public EnemyDeploy Deploy { private set; get; }
    
    public bool InShoot { private set; get; }
  
    public SpriteRenderer MainRenderer { private set; get; }

    public Material Material { private set; get; }

    public bool InMoveToTarget { private set; get; }
    public bool InMove { private set; get; }
    public bool IsDead { private set; get; }

    //private MoveAI_Base MoveAI;
    private AI_Base AIMoudle;



    private int _nextAnimationFrame;
    private int _currAniIdx;

    private EnemyMoveStyle _aniStyle;
    private EnemyMoveStyle AniStyle
    {
        get { return _aniStyle; }
        set 
        {
            if (_aniStyle != value)
            {
                _currAniIdx = 0;
                _nextAnimationFrame = 0;
            }
            _aniStyle = value;
        }
    }


     public virtual void Init(SpriteRenderer renderer, EnemyDeploy deploy)
    {
        transform.SetLayer(Layers.Enemy);

        transform.localScale = Vector3.one * deploy.scale;

        Deploy = deploy;
        MainRenderer = renderer;

        HP = deploy.maxHp;
        HPMax = HP;

        _aniStyle = EnemyMoveStyle.Idle;
        _currPos = transform.position;

        InitRigid(); 

        if(!string.IsNullOrEmpty(deploy.AIScript))
        {
            AIMoudle = Common.CreateInstance(deploy.AIScript) as AI_Base;
            if (AIMoudle != null)
            {
                AIMoudle.Init(this);
            }
        }
    }
   
    protected override void Update()
    {
        base.Update();
        UpdateHitBrightness();

        if (GamePause.InPause != false)
            return;

        UpdateMoveStyle();
        UpdateAnimation();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (GamePause.InPause != false)
            return;
       
        UpdateMovePos();

        if (AIMoudle != null)
        {
            AIMoudle.OnFixedUpdate();
        }
    }

    /// <summary>
    /// 敌人被击高亮闪白相关
    /// </summary>
    private float _curBrightness = 1f;
    private void UpdateHitBrightness()
    {
        Material.SetFloat("_Brightness", _curBrightness);
        _curBrightness = Mathf.Lerp(_curBrightness, 1f, Time.deltaTime * 30f);
    }

    private void SetBrightness()
    {
        _curBrightness = 1.5f;
        Material.SetFloat("_Brightness", 1.5f);
    }

    /// <summary>
    /// 敌人被子弹击中时
    /// </summary>
    /// <param name="atk"></param>

    public void OnEnemyHit(int atk)
    {
        if (IsDead) return;

        if(!Invisible)
        {
            //闪白
            SetBrightness();

            //扣血
            CalculateHp(atk);
        }
    }

    protected virtual void CalculateHp(int atk)
    {
        //扣血
        HP -= atk;

        //死亡
        if (HP < 0)
        {
            IsDead = true;
            OnDead();
        }
    }

    public void SelfDie()
    {
        if (IsDead) return;
        IsDead = true;
        OnDead();
    }

    protected virtual void OnDead()
    {
        //特效
        EffectFactory.PlayEffectOnce(Deploy.deadEffect, transform.position);

        Sound.PlayUiAudioOneShot(104);

        GameEventCenter.Send(GameEvent.OnEnemyDie);

        Destroy(gameObject);
    }


    private MoveData _moveData;
    private int _totalFrame;
    private float _lastHelixFrame;
    public void Move(MoveData moveData, float moveSpeed)
    {
        InMove = true;
        _moveData = moveData;
        _totalFrame = 0;
        _lastHelixFrame = 0;
        _moveSpeed = moveSpeed;
    }

    private Vector3 _moveTarget;
    private Vector3 _currPos;
    private float _moveSpeed;
    public void MoveToTarget(Vector3 targetPoint, float moveSpeed)
    {
        InMoveToTarget = true;
        _moveTarget = targetPoint;
        _moveSpeed = moveSpeed;
        _currPos = transform.position;
    }

    public void Wander(Vector2 xRange, Vector2 yRange, Vector2 xAmp, Vector2 yAmp, float speed)
    {
        var curPos = Vector2Fight.WorldPosToFightPos(transform.position);
        //Debug.LogError("wander:" + curPos);

        var ampX = UnityEngine.Random.Range(xAmp.x, xAmp.y);
        var ampY = UnityEngine.Random.Range(yAmp.x, yAmp.y);
        var targetX = UnityEngine.Random.Range(0, 2) == 0 ? -ampX : ampX;
        var targetY = UnityEngine.Random.Range(0, 2) == 0 ? -ampY : ampY;
        targetX = Mathf.Clamp(targetX, xRange.x, xRange.y);
        targetY = Mathf.Clamp(targetY, yRange.x, yRange.y);

        MoveToTarget(Vector2Fight.New(targetX, targetY), speed);
    }

    private void UpdateMovePos()
    {
        var delta = Time.deltaTime;
        if (InMoveToTarget)
        {
            _currPos = Vector3.MoveTowards(_currPos, _moveTarget, delta * _moveSpeed);
            Rigid2D.MovePosition(_currPos);

            if (MathUtility.DistanceXY(_currPos, _moveTarget) < 0.5f)
            {
                StopMove();
            }
        }
        else if (InMove)
        {
            _totalFrame += 1;

            //螺旋移动
            if (_moveData.HelixToward != MoveData.EHelixToward.None)
            {
                var eulurZ = (int)_moveData.HelixToward * _moveData.EulurPerFrame * delta * 60f;
                _moveData.Forward = Quaternion.Euler(0, 0, eulurZ) * _moveData.Forward;

                if (_totalFrame - _lastHelixFrame >= _moveData.HelixRefretFrame)
                {
                    _lastHelixFrame = _totalFrame;
                    _moveData.HelixToward = _moveData.HelixToward == MoveData.EHelixToward.Right ?
                                                    MoveData.EHelixToward.Left :
                                                    MoveData.EHelixToward.Right;
                }
            }
            Rigid2D.MovePosition(transform.position + _moveData.Forward.normalized * delta * _moveSpeed);
        }
    }

    private void UpdateMoveStyle()
    {
        if(InMoveToTarget || InMove)
        {
            float moveX;
            if(InMove)
            {
                moveX = _moveData.Forward.normalized.x;
            }
            else
            {
                moveX = (_moveTarget - transform.position).normalized.x;
            }

            if(moveX > 0.01f)
            {
                //right
                if(AniStyle != EnemyMoveStyle.MoveIdle)
                    AniStyle = EnemyMoveStyle.Move;

                 MainRenderer.flipX = false;
            }
            else if(moveX < 0.01f)
            {
                if (AniStyle != EnemyMoveStyle.MoveIdle)
                    AniStyle = EnemyMoveStyle.Move;

             	MainRenderer.flipX = true;
            }
            else
            {
                AniStyle = EnemyMoveStyle.Idle;
            }
        }
        else
        {
            AniStyle = EnemyMoveStyle.Idle;
        }
    }
    
    private void StopMove()
    {
        InMoveToTarget = false;
        InMove = false;
        AniStyle = EnemyMoveStyle.Idle;

        if(_moveData != null)
        {
            Pool.Free(_moveData);
        }
    }


    private void UpdateAnimation()
    {
        if (GameSystem.FixedFrameCount < _nextAnimationFrame) return;

        if (SpriteDic.TryGetValue(AniStyle, out List<Sprite> sprites))
        {
            if (sprites.Count > 0)
            {
                MainRenderer.sprite = sprites[_currAniIdx];
                _currAniIdx++;


                if (_currAniIdx >= sprites.Count)
                {
                    if (AniStyle == EnemyMoveStyle.Move)
                        AniStyle = EnemyMoveStyle.MoveIdle;
                    else
                        _currAniIdx = 0;
                }
            }
        }

        _nextAnimationFrame = GameSystem.FixedFrameCount + Deploy.frameSpeed[(int)AniStyle];
    }

    protected override void OnDestroy()
    {
        if (_moveData != null)
        {
            Pool.Free(_moveData);
        }

        if (AIMoudle != null)
        {
            AIMoudle.OnDestroy();
        }
        base.OnDestroy();
    }


    //创建怪物
    public static IEnumerator Create(int enemyId, float bornX = 0f, float bornY = 150f)
    {
        var deploy = TableUtility.GetDeploy<EnemyDeploy>(enemyId);

        var gameObj = new GameObject(deploy.name);
        gameObj.SetActiveSafe(false);
        gameObj.transform.SetParent(null);
        yield return Yielders.Frame;

        //创建显示模型
        var model = new GameObject(deploy.name + "_model");
        var mainSprite = model.AddComponent<SpriteRenderer>();
        mainSprite.sortingOrder = SortingOrder.Enemy;
        mainSprite.material = new Material(GameSystem.DefaultRes.CommonShader);
        model.transform.SetParent(gameObj.transform, false);
        yield return Yielders.Frame;

        //设置贴图
        var enemy = deploy.isBoss ? gameObj.AddComponent<Boss>() : gameObj.AddComponent<Enemy>();
        for (int i = 0; i < deploy.resoureIds.Length; i++)
        {
            var resourceId = deploy.resoureIds[i];
            yield return TextureUtility.LoadResourceById(resourceId, sprites =>
            {
                enemy.SpriteDic[(EnemyMoveStyle)i] = sprites;
            });
        }

        //Collider
        var collider = gameObj.AddComponent<CircleCollider2D>();
        collider.radius = deploy.radius;
        collider.isTrigger = true;

        //init
        enemy.Material = mainSprite.material;
        enemy.transform.position = Vector2Fight.New(bornX, bornY);
        enemy.Init(mainSprite, deploy);
        gameObj.SetActiveSafe(true);
    }
}


public class EnemyDeploy : Conditionable
{
    public int id;
    public bool isBoss;
    public string name;
    public float scale;
    public float radius;
    public int[] frameSpeed;
    public int[] resoureIds;
    public int maxHp;
    public string AIScript;
    public string[] BossCard;
    public string deadEffect;
    public string BossDraw;
}

