using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : EntityBase
{
    public Dictionary<EMoveStyle, List<Sprite>> SpriteDic = new Dictionary<EMoveStyle, List<Sprite>>();
   
    public enum EMoveStyle
    {
        LeftIdle,
        LeftMove,
        Idle,
        RightMove,
        RightIdle,
    }

    public SpriteRenderer MainRenderer { private set; get; }
    
    public PlayerDeploy Deploy { private set; get; }

    public bool InSlow { private set; get; }
    public bool InShoot { private set; get; }

    private Vector3 _moveFoward;
    private ControllerActions _actions;
    private SlowPoint _redPoint;

    private bool _inMove;
   
   
    private int _currAniIdx;


    private EMoveStyle _aniStyle;
    private EMoveStyle AniStyle
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

    //僚机管理器
    private PlayerSupportMgr _supportMgr;

    public void Init(SpriteRenderer renderer, SlowPoint point, PlayerDeploy deploy)
    {
        transform.SetLayer(Layers.Player);

        Deploy = deploy;
        MainRenderer = renderer;

        _redPoint = point;
        _actions = ControllerPc.GetActions();
        _aniStyle = EMoveStyle.Idle;

        transform.localScale = Vector3.one * deploy.scale;

        _supportMgr = new PlayerSupportMgr();
        _supportMgr.Init(this);
        AddSupport();
    }

    public void AddSupport()
    {
        _supportMgr.AddSupport();
        _supportMgr.AddSupport();
        _supportMgr.AddSupport();
        _supportMgr.AddSupport();
    }

    private void UpdateOperation()
    {
        if (_actions == null) return;

        //move
        if (_actions.Move.IsPressed)
        {
            Move(_actions.Move.Value.normalized);
        }
        else if (_actions.Move.WasReleased)
        {
            StopMove();
        }

        //slow
        InSlow = _actions.Get(EControllerBtns.SlowMove).IsPressed;
        _redPoint.SetVisible(InSlow);


        //shoot
        InShoot = _actions.Get(EControllerBtns.Shoot).IsPressed;
    }


    private float _nextShootTime = 0;
    private void UpdateShoot()
    {
        if (!InShoot) return;

        if (Time.time < _nextShootTime) return;
        _nextShootTime = Time.time + Deploy.shootFrame * GameSystem.FrameTime;

        for (int i = 0; i < Deploy.shootPos.Length; i++)
        {
            var pos = transform.position + new Vector3(Deploy.shootPos[i][0], Deploy.shootPos[i][1]);

            BulletFactory.CreateBullet(Deploy.normalBulletId, transform, gameObject.layer,  bullet =>
            {
                bullet.Shoot(pos, Vector3.up);
            });
        }

        Sound.PlayUiAudioOneShot(Deploy.shootSound);
    }

    protected override void Update()
    {
        base.Update();

        if (GamePause.InPause == false)
        {
            UpdateOperation();
            UpdateAnimation();
            UpdateShoot();
            _supportMgr.Update();
        }
    }

    public void Move(Vector3 dir)
    {
        _inMove = true;
        _moveFoward = dir;

        var moveSpeed = InSlow ? Deploy.slowSpeed : Deploy.speed;
        var targetPos = transform.position + _moveFoward * GameSystem.FrameTime * moveSpeed;
        Rigid2D.MovePosition(targetPos);

        if (MathUtility.FloatEqual(_moveFoward.x, 0))
        {
            AniStyle = EMoveStyle.Idle;
            return;
        }

         if (_moveFoward.x > 0 && AniStyle != EMoveStyle.RightIdle)
            AniStyle = EMoveStyle.RightMove;

        if (_moveFoward.x < 0 && AniStyle != EMoveStyle.LeftIdle)
            AniStyle = EMoveStyle.LeftMove;
    }

    public void StopMove()
    {
        _inMove = false;
        _moveFoward = Vector3.zero;
        AniStyle = EMoveStyle.Idle;
    }

    private float _nextAnimationTime;
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
                    if (AniStyle == EMoveStyle.LeftMove)
                        AniStyle = EMoveStyle.LeftIdle;
                    else if(AniStyle == EMoveStyle.RightMove)
                        AniStyle = EMoveStyle.RightIdle;
                    else
                        _currAniIdx = 0;
                }
            }
        }

        _nextAnimationTime = Time.time + Deploy.frameSpeed[(int)AniStyle] * GameSystem.FrameTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
    }
  
    private void OnCollisionStay2D(Collision2D collision)
    {
       
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    //static create
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

        model.transform.SetParent(playerObject.transform, false);
        yield return Yielders.Frame;

        //设置贴图
        var player = playerObject.AddComponent<Player>();
        for (int i = 0; i < deploy.resoureIds.Length; i++)
        {
            var resourceId = deploy.resoureIds[i];
            yield return TextureUtility.LoadResourceById(resourceId, sprites =>
            {
                player.SpriteDic[(EMoveStyle)i] = sprites;
            });
        }

        yield return Yielders.Frame;

        //Collider
        var collider = playerObject.AddComponent<CircleCollider2D>();
        collider.radius = 0.04f;


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
}

