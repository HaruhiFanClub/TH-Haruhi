using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //血量
    public int HP { private set; get; }

    public SpriteRenderer MainRenderer { private set; get; }
    
    public EnemyDeploy Deploy { private set; get; }
    
    public bool InShoot { private set; get; }

    public Material Material { private set; get; }

    public bool InMove { private set; get; }

    public bool IsDead { private set; get; }

    private MoveAI_Base MoveAI;
    private ShootAI_Base ShootAI;


    private EnemyMoveStyle _aniStyle;
    private EnemyMoveStyle AniStyle
    {
        get { return _aniStyle; }
        set 
        {
            if (_aniStyle != value)
            {
                _currAniIdx = 0;
                _nextAnimationTime = 0;
            }
            _aniStyle = value;
        }
    }


    public void Init(SpriteRenderer renderer, EnemyDeploy deploy)
    {
        transform.SetLayer(Layers.Enemy);

        transform.localScale = Vector3.one * deploy.scale;

        Deploy = deploy;
        MainRenderer = renderer;

        HP = deploy.maxHp;

        _aniStyle = EnemyMoveStyle.Idle;
        _currPos = transform.position;

        InitRigid(); 

        //初始化AI模块
        MoveAI =  Common.CreateInstance(deploy.MoveAI) as MoveAI_Base;
        if(MoveAI != null)
        {
            MoveAI.Init(this);
        }
        ShootAI = Common.CreateInstance(deploy.ShootAI) as ShootAI_Base;
        if (ShootAI != null)
        {
            ShootAI.Init(this);
        }
    }
   
    protected override void Update()
    {
        base.Update();
        UpdateHitBrightness();

        if (GamePause.InPause != false)
        {
            return;
        }

        UpdateAnimation();
        UpdateMove();

        

        if (MoveAI != null)
        {
            MoveAI.OnUpdate();
        }

        if(ShootAI != null)
        {
            ShootAI.OnUpdate();
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

    private RelayInterval _hitSoundInterval = new RelayInterval(0.05f);
    public void OnEnemyHit(int atk)
    {
        if (IsDead) return;

        //闪白
        SetBrightness();

        //扣血
        HP -= atk;

        //死亡
        if (HP < 0) 
        {
            IsDead = true;
            OnDead();
        }
    }

    private void OnDead()
    {
        //特效
        EffectFactory.PlayEffectOnce(Deploy.deadEffect, transform.position);

        Sound.PlayUiAudioOneShot(104);

        GameEventCenter.Send(GameEvent.OnEnemyDie);

        Destroy(gameObject);
    }

    private Vector3 _moveTarget;
    private Vector3 _currPos;
    private float _moveSpeed;

    public void Move(Vector3 targetPoint, float moveSpeed)
    {
        InMove = true;
        _moveTarget = targetPoint;
        _moveSpeed = moveSpeed;
    }

    private void UpdateMove()
    {
        if(InMove)
        {
            var moveX = (_moveTarget - transform.position).normalized.x;

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

            _currPos = Vector3.Lerp(_currPos, _moveTarget, GameSystem.FrameTime * _moveSpeed);
            Rigid2D.MovePosition(_currPos);

            if(MathUtility.DistanceXY(_currPos, _moveTarget) < 0.05f)
            {
                StopMove();
            }
        }
        else
        {
            AniStyle = EnemyMoveStyle.Idle;
        }
    }

    private void StopMove()
    {
        InMove = false;
        AniStyle = EnemyMoveStyle.Idle;
    }

    private float _nextAnimationTime;
    private int _currAniIdx;

    private void UpdateAnimation()
    {
        if (Time.time < _nextAnimationTime) return;

        List<Sprite> sprites;
        if(SpriteDic.TryGetValue(AniStyle, out sprites))
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

        _nextAnimationTime = Time.time + Deploy.frameSpeed[(int)AniStyle] * GameSystem.FrameTime;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //玩家子弹击中怪物逻辑
        if (collider.gameObject.layer != Layers.PlayerBullet)
            return;

        var playerBullet = collider.GetComponent<Bullet>();
        OnEnemyHit(playerBullet.Deploy.atk);
        playerBullet.OnBulletHitEnemy();
    }

    protected override void OnDestroy()
    {
        if (MoveAI != null)
        {
            MoveAI.OnDestroy();
        }

        if (ShootAI != null)
        {
            ShootAI.OnDestroy();
        }
        base.OnDestroy();
    }


    //创建怪物
    public static IEnumerator Create(int enemyId)
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
        mainSprite.material = new Material(GameSystem.DefaultRes.BulletShader);
        model.transform.SetParent(gameObj.transform, false);
        yield return Yielders.Frame;

        //设置贴图
        var enemy = gameObj.AddComponent<Enemy>();
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

        //init
        enemy.Material = mainSprite.material;
        enemy.transform.position = Vector2Fight.New(0, 150);
        enemy.Init(mainSprite, deploy);
        gameObj.SetActiveSafe(true);
    }
}


public class EnemyDeploy : Conditionable
{
    public int id;
    public string name;
    public float scale;
    public float radius;
    public int[] frameSpeed;
    public int[] resoureIds;
    public int maxHp;
    public string MoveAI;
    public string ShootAI;
    public string deadEffect;
}

