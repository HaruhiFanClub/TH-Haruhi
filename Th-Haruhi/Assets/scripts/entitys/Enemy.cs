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


    public override void SetRenderer(Renderer r)
    {
        base.SetRenderer(r);
        MainRenderer = (SpriteRenderer)r;
    }

    public virtual IEnumerator Init(EnemyDeploy deploy)
    {
        transform.SetLayer(Layers.Enemy);

        transform.localScale = Vector3.one * deploy.scale;

        Deploy = deploy;

        HP = deploy.maxHp;
        HPMax = HP;

        _aniStyle = EnemyMoveStyle.Idle;

        this.AddRigidBody(); 

        if(!string.IsNullOrEmpty(deploy.AIScript))
        {
            AIMoudle = Common.CreateInstance(deploy.AIScript) as AI_Base;
            if (AIMoudle != null)
            {
                AIMoudle.Init(this);
            }
        }
        yield break;
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
        MainRenderer.material.SetFloat("_Brightness", _curBrightness);
        _curBrightness = Mathf.Lerp(_curBrightness, 1f, Time.deltaTime * 30f);
    }

    private void SetBrightness()
    {
        _curBrightness = 1.5f;
        MainRenderer.material.SetFloat("_Brightness", 1.5f);
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

        Sound.PlayTHSound("enep00");

        GameEventCenter.Send(GameEvent.OnEnemyDie);

        Destroy(gameObject);
    }

    private void UpdateMoveStyle()
    {
        if(InMove)
        {
            var moveX = (MoveTarget - CacheTransform.position).normalized.x;

            if (moveX > 0.01f)
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
        gameObj.SetActiveSafe(true);
        enemy.transform.position = Vector2Fight.NewWorld(bornX, bornY);
        enemy.SetRenderer(mainSprite);
        yield return enemy.Init(deploy);
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

