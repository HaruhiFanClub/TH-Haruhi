using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EntityBase
{
    public override EEntityType EntityType => EEntityType.Enemy;
    public Dictionary<EnemyMoveStyle, List<Sprite>> SpriteDic = new Dictionary<EnemyMoveStyle, List<Sprite>>();
    public int HP = 1000;

    public enum EnemyMoveStyle
    {
        Idle,
        Move,
        MoveIdle,
    }

    public SpriteRenderer MainRenderer { private set; get; }
    
    public EnemyDeploy Deploy { private set; get; }
    
    public bool InShoot { private set; get; }

    public Material Material { private set; get; }

    private bool _inMove;
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
        _aniStyle = EnemyMoveStyle.Idle;
        _currPos = transform.position;
    }


    private float _nextShootTime;
    private int _shootIdx;
    private void UpdateShoot()
    {
        if(Time.time > _nextShootTime)
        {
            var f1 = Quaternion.Euler(0, 0, _shootIdx * 29) * transform.up;
            BulletFactory.CreateBulletAndShoot(1001 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, f1);

            /*
            var f2 = Quaternion.Euler(0, 0, _shootIdx * 13) * transform.up;
            BulletFactory.CreateBulletAndShoot(1005 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, f2);

            var f3 = Quaternion.Euler(0, 0, _shootIdx * 11) * transform.up;
            BulletFactory.CreateBulletAndShoot(1009 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, f3);

            BulletFactory.CreateBulletAndShoot(1013 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, Quaternion.Euler(0, 0, _shootIdx * 9) * transform.up);

            BulletFactory.CreateBulletAndShoot(1017 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, Quaternion.Euler(0, 0, _shootIdx * 7) * transform.up);

            BulletFactory.CreateBulletAndShoot(1021 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, Quaternion.Euler(0, 0, _shootIdx * 17) * transform.up);

            BulletFactory.CreateBulletAndShoot(1025 + _shootIdx % 4, transform, Layers.EnemyBullet, transform.position, Quaternion.Euler(0, 0, _shootIdx * 23) * transform.up);
            */
            _shootIdx++;
            if (_shootIdx > 10000000)
                _shootIdx = 0;

            _nextShootTime = Time.time + GameSystem.FrameTime;
        }
    }

    private float _nextMoveTime;
    private bool _textMoveLeft;
    protected override void Update()
    {
        base.Update();
        UpdateHitBrightness();

        if (GamePause.InPause == false)
        {
            UpdateAnimation();
            UpdateMove();
            UpdateShoot();

            //test
            if(Time.time > _nextMoveTime)
            {
                _textMoveLeft = !_textMoveLeft;
                _nextMoveTime = Time.time + 5f;
                if (_textMoveLeft)
                {
                    Move(Vector2Fight.New(-75f, 80f), 0.3f);
                }
                else
                    Move(Vector2Fight.New(75f, 80f), 0.3f);
            }

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
        //闪白
        SetBrightness();

        //扣血
        HP -= atk;

        if (HP < 0) 
        {
            OnEmemyDie();
        }
    }

    private void OnEmemyDie()
    {
        Destroy(gameObject);
    }

    private Vector3 _moveTarget;
    private Vector3 _currPos;
    private float _moveSpeed;
    public void Move(Vector3 targetPoint, float moveSpeed)
    {
        _inMove = true;
        _moveTarget = targetPoint;
        _moveSpeed = moveSpeed;
    }

    private void UpdateMove()
    {
        if(_inMove)
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

    public void StopMove()
    {
        _inMove = false;
        AniStyle = EnemyMoveStyle.Idle;
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

    //static create
    public static IEnumerator Create(int enemyId, Vector2 startPos, Action<Enemy> callBack)
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
        gameObj.transform.position = startPos;
        enemy.Material = mainSprite.material;
        enemy.Init(mainSprite, deploy);
        gameObj.SetActiveSafe(true);
        callBack(enemy);
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
}

