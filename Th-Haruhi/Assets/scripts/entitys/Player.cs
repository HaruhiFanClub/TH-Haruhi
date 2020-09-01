using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : EntityBase
{
    public enum PlayerMoveStyle
    {
        LeftIdle,
        LeftMove,
        Idle,
        RightMove,
        RightIdle,
    }

    public override EEntityType EntityType => EEntityType.Player;

    //动画帧缓存
    public Dictionary<PlayerMoveStyle, List<Sprite>> SpriteDic = new Dictionary<PlayerMoveStyle, List<Sprite>>();

    public SpriteRenderer SpriteRenderer { private set; get; }
    
    //配置
    public PlayerDeploy Deploy { private set; get; }

    //是否慢速移动
    public bool InSlow { private set; get; }

    //是否在射击
    public bool InShoot { private set; get; }

    //操作指令控制
    public ControllerActions Actions { private set; get; }

    //判定点
    public SlowPoint RedPoint { private set; get; }

    //僚机管理器
    public PlayerSupportMgr SupportMgr { private set; get; }

    //是否无敌
    public bool Invincible { private set; get; }

    //当前动画帧id
    private int _currAniIdx;

    //下一帧动画时间
    private float _nextAnimationTime;

    //下次射击时间
    private float _nextShootTime;


    private PlayerMoveStyle _aniStyle;
    private PlayerMoveStyle AniStyle
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

    public void Init(SpriteRenderer renderer, SlowPoint point, PlayerDeploy deploy)
    {
        transform.SetLayer(Layers.Player);

        Deploy = deploy;
        SpriteRenderer = renderer;
       // Material = renderer.material;

        RedPoint = point;
        Actions = ControllerPc.GetActions();
        _aniStyle = PlayerMoveStyle.Idle;

        transform.localScale = Vector3.one * deploy.scale;
        InitRigid();

        SupportMgr = new PlayerSupportMgr();
        SupportMgr.Init(this);
        AddSupport();
    }

    protected override void Update()
    {
        base.Update();

        if (GamePause.InPause != false)
        {
            return;
        }

        UpdateOperation();
        UpdateAnimation();
        UpdateShoot();
        UpdateInvicibleTime();
        SupportMgr.Update();
    }

    //设置X秒无敌时间
    private float _recoverInvicibleTime;
    public void SetInvincibleTime(float sec)
    {
        _recoverInvicibleTime = Time.time + sec;

        if (!Invincible)
        {
            Invincible = true;
            SpriteRenderer.gameObject.AddComponent<AutoBrightness>().Speed = 3;
        }
    }

    private void UpdateInvicibleTime()
    {
        if(Invincible && Time.time > _recoverInvicibleTime)
        {
            Invincible = false;
            SpriteRenderer.gameObject.RemoveComponent<AutoBrightness>();
        }
    }

    //增加一个僚机
    public void AddSupport()
    {
        SupportMgr.AddSupport();
    }

    //操作按钮逻辑
    private void UpdateOperation()
    {
        if (UILoading.InLoading) return;
        if (Actions == null) return;

        //move
        if (Actions.Move.IsPressed)
        {
            Move(Actions.Move.Value.normalized);
        }
        else if (Actions.Move.WasReleased)
        {
            StopMove();
        }

        //slow
        InSlow = Actions.Get(EControllerBtns.SlowMove).IsPressed;
        RedPoint.SetVisible(InSlow);

        //shoot
        InShoot = Actions.Get(EControllerBtns.Shoot).IsPressed;
    }

    //射击逻辑
    private void UpdateShoot()
    {
        if (!InShoot) return;

        if (Time.time < _nextShootTime) return;
        _nextShootTime = Time.time + Deploy.shootFrame * GameSystem.FrameTime;

        for (int i = 0; i < Deploy.shootPos.Length; i++)
        {
            var pos = transform.position + new Vector3(Deploy.shootPos[i][0], Deploy.shootPos[i][1]);
            BulletFactory.CreateBullet(Deploy.normalBulletId, transform, Layers.PlayerBullet,  bullet =>
            {
                bullet.Shoot(pos, Vector3.up);
            });
        }

        Sound.PlayUiAudioOneShot(Deploy.shootSound);
    }

    //移动
    private void Move(Vector3 dir)
    {
        var moveSpeed = InSlow ? Deploy.slowSpeed : Deploy.speed;
        var targetPos = transform.position + dir * Time.fixedDeltaTime * moveSpeed;
        Rigid2D.MovePosition(targetPos);

        if (MathUtility.FloatEqual(dir.x, 0))
        {
            AniStyle = PlayerMoveStyle.Idle;
            return;
        }

         if (dir.x > 0 && AniStyle != PlayerMoveStyle.RightIdle)
            AniStyle = PlayerMoveStyle.RightMove;

        if (dir.x < 0 && AniStyle != PlayerMoveStyle.LeftIdle)
            AniStyle = PlayerMoveStyle.LeftMove;
    }

    //停止移动
    private void StopMove()
    {
        AniStyle = PlayerMoveStyle.Idle;
    }

    //移动动画处理
    private void UpdateAnimation()
    {
        if (Time.time < _nextAnimationTime) return;

        List<Sprite> sprites;
        if(SpriteDic.TryGetValue(AniStyle, out sprites))
        {
            if (sprites.Count > 0)
            {
                SpriteRenderer.sprite = sprites[_currAniIdx];
                _currAniIdx++;


                if (_currAniIdx >= sprites.Count)
                {
                    if (AniStyle == PlayerMoveStyle.LeftMove)
                        AniStyle = PlayerMoveStyle.LeftIdle;
                    else if(AniStyle == PlayerMoveStyle.RightMove)
                        AniStyle = PlayerMoveStyle.RightIdle;
                    else
                        _currAniIdx = 0;
                }
            }
        }

        _nextAnimationTime = Time.time + Deploy.frameSpeed[(int)AniStyle] * GameSystem.FrameTime;

    }

    //死亡处理
    private void OnDead()
    {
        //音效
        Sound.PlayUiAudioOneShot(Deploy.deadSound);

        //特效
        EffectFactory.PlayEffectOnce(Deploy.deadEffect, transform.position);

        //销毁僚机
        SupportMgr.Clear();

        //销毁自己
        Destroy(gameObject);

        //发事件
        GameEventCenter.Send(GameEvent.OnPlayerDead);
    }

    //伤害判定
    private void OnTriggerEnter2D(Collider2D c)
    {
        //无敌中不处理
        if (Invincible) return;

        //碰到怪或怪物子弹就死
        if(c.gameObject.layer == Layers.EnemyBullet || c.gameObject.layer == Layers.Enemy)
        {
            OnDead();
        }
    }

    //创建角色 static
    public static IEnumerator Create(int playerId, Action<Player> callBack)
    {
        var deploy = TableUtility.GetDeploy<PlayerDeploy>(playerId);

        var playerObject = new GameObject(deploy.name);
        playerObject.SetActiveSafe(false);
        playerObject.transform.SetParent(null);
        yield return Yielders.Frame;

        //创建显示模型
        var model = new GameObject(deploy.name + "_model");
        var mainSprite = model.AddComponent<SpriteRenderer>();
        mainSprite.sortingOrder = SortingOrder.Player;
        mainSprite.material = new Material(GameSystem.DefaultRes.BulletShader);
        model.transform.SetParent(playerObject.transform, false);
        yield return Yielders.Frame;


        //设置贴图
        var player = playerObject.AddComponent<Player>();
        for (int i = 0; i < deploy.resoureIds.Length; i++)
        {
            var resourceId = deploy.resoureIds[i];
            yield return TextureUtility.LoadResourceById(resourceId, sprites =>
            {
                player.SpriteDic[(PlayerMoveStyle)i] = sprites;
            });
        }
        yield return Yielders.Frame;


        //Collider
        var collider = playerObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.2f;

        //判定点
        var pointObj = ResourceMgr.LoadImmediately("player/point.prefab");
        var point = ResourceMgr.Instantiate(pointObj);
        point.transform.SetParent(playerObject.transform, false);
        var script = point.GetComponent<SlowPoint>();

        player.Init(mainSprite, script, deploy);
        playerObject.SetActiveSafe(true);
        callBack(player);
    }
}


public class PlayerDeploy : Conditionable
{
    public int id;
    public string name;
    public float scale;
    public float speed;
    public float slowSpeed;
    public int[] frameSpeed;
    public int[] resoureIds;
    public int normalBulletId;
    public int shootFrame;
    public float[][] shootPos;
    public int shootSound;
    public int supprotId;
    public float[] supportUp;
    public float[] supportDown;
    public float[] supportUpSlow;
    public float[] supportDownSlow;
    public float supportDownRota;
    public float supportDownRotaSlow;
    public string deadEffect;
    public int deadSound;
}

