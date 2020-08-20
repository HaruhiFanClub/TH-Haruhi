using System.Collections.Generic;
using UnityEngine;

public class Character : EntityBase
{
    [SerializeField] public GameObject JudgePoint;

    [SerializeField] public List<Sprite> IdleSprites;
    [SerializeField] public List<Sprite> LeftSprites;
    [SerializeField] public List<Sprite> RightSprites;
    [SerializeField] public List<Sprite> LeftMoveSprites;
    [SerializeField] public List<Sprite> RightMoveSprites;

    [SerializeField] public Transform ShootSlot1;
    [SerializeField] public Transform ShootSlot2;

    private enum EMoveStyle
    {
        Idle,
        MoveLeft,
        MoveRight,
        IdleLeft,
        IdleRight,
    }

    private SpriteRenderer _sr;
    private Rigidbody2D _rg;
    private Vector3 _moveFoward;
    private ControllerActions _actions;
    private bool _inMove;
    private bool _inSlow;
    private bool _inShoot;
    private int _currAniIdx;
    private Shooter _shooter;
    private Dictionary<EMoveStyle, float> _aniSpeed = new Dictionary<EMoveStyle, float>();

    private EMoveStyle _aniStyle;
    private EMoveStyle AniStyle
    {
        get { return _aniStyle; }
        set 
        {
            if (_aniStyle != value)
                _currAniIdx = 0;
            _aniStyle = value;
        }
    }

    

    protected override void Awake()
    {
        base.Awake();
        _rg = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _actions = ControllerPc.GetActions();
        _aniStyle = EMoveStyle.Idle;
        _shooter = new Shooter(this);

        //动画速度
        _aniSpeed[EMoveStyle.Idle] = 0.1f;
        _aniSpeed[EMoveStyle.MoveLeft] = 0.05f;
        _aniSpeed[EMoveStyle.MoveRight] = 0.05f;
        _aniSpeed[EMoveStyle.IdleLeft] = 0.1f;
        _aniSpeed[EMoveStyle.IdleRight] = 0.1f;
    }


    private void UpdateOperation()
    {
        if (_actions == null) return;

        //move
        if (_actions.Move.IsPressed)
        {
            Move(_actions.Move.Value);
        }
        else if (_actions.Move.WasReleased)
        {
            StopMove();
        }

        //slow
        _inSlow = _actions.Get(EControllerBtns.SlowMove).IsPressed;
        if (JudgePoint)
            JudgePoint.SetActiveSafe(_inSlow);



        //shoot
        var inShoot = _actions.Get(EControllerBtns.Shoot).IsPressed;

        if (!_inShoot && inShoot)
            _shooter.StartShoot();
        else if (_inShoot && !inShoot)
            _shooter.EndShoot();

        _inShoot = inShoot;
    }

    protected override void Update()
    {
        base.Update();

        if (GamePause.InPause == false)
        {
            UpdateOperation();
            UpdateAnimation();
            _shooter.Update();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateMove();
    }


    public void Move(Vector3 dir)
    {
        _inMove = true;
        _moveFoward = dir;

        if(MathUtility.FloatEqual(_moveFoward.x, 0))
        {
            AniStyle = EMoveStyle.Idle;
            return;
        }

        if (_moveFoward.x > 0 && AniStyle != EMoveStyle.IdleRight)
            AniStyle = EMoveStyle.MoveRight;

        if (_moveFoward.x < 0 && AniStyle != EMoveStyle.IdleLeft)
            AniStyle = EMoveStyle.MoveLeft;
    }

    public void StopMove()
    {
        _inMove = false;
        _moveFoward = Vector3.zero;
        AniStyle = EMoveStyle.Idle;
    }


    private void UpdateMove()
    {
        var deltaTime = Time.deltaTime;
        if (_inMove)
        {
            var targetPos = transform.position + _moveFoward * deltaTime * GetMoveSpeed();
            _rg.MovePosition(targetPos);
        }
    }

    private float GetMoveSpeed()
    {
        return _inSlow ? 7f : 14f;
    }

    private float _nextAnimationTime;
    private void UpdateAnimation()
    {
        if (Time.time < _nextAnimationTime) return;

        switch (AniStyle)
        {
            case EMoveStyle.Idle:
                if (IdleSprites.Count > 0)
                {
                    _sr.sprite = IdleSprites[_currAniIdx];
                    _currAniIdx++;
                    if (_currAniIdx >= IdleSprites.Count)
                        _currAniIdx = 0;
                }
                break;
            case EMoveStyle.MoveLeft:
                if (LeftMoveSprites.Count > 0)
                {
                    _sr.sprite = LeftMoveSprites[_currAniIdx];
                    _currAniIdx++;
                    if (_currAniIdx >= LeftMoveSprites.Count)
                    {
                        AniStyle = EMoveStyle.IdleLeft;
                    }
                }
                break;
            case EMoveStyle.MoveRight:
                if (RightMoveSprites.Count > 0)
                {
                    _sr.sprite = RightMoveSprites[_currAniIdx];
                    _currAniIdx++;
                    if (_currAniIdx >= RightMoveSprites.Count)
                    {
                        AniStyle = EMoveStyle.IdleRight;
                    }
                }
                break;
            case EMoveStyle.IdleLeft:
                if (LeftSprites.Count > 0)
                {
                    _sr.sprite = LeftSprites[_currAniIdx];
                    _currAniIdx++;
                    if (_currAniIdx >= LeftSprites.Count)
                        _currAniIdx = 0;
                }
                break;
            case EMoveStyle.IdleRight:
                if (RightSprites.Count > 0)
                {
                    _sr.sprite = RightSprites[_currAniIdx];
                    _currAniIdx++;
                    if (_currAniIdx >= RightSprites.Count)
                        _currAniIdx = 0;
                }
                break;
        }

        _nextAnimationTime = Time.time + _aniSpeed[AniStyle];
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
    public static Character CreateCharacter(int chrId)
    {
        var chrDeploy = GameCfgList.GetDeploy<CharacterDeploy>(chrId);

        var obj = ResourceMgr.LoadImmediately(chrDeploy.resource);
        var chrObj = ResourceMgr.Instantiate(obj);
        chrObj.transform.SetParent(Level.Root);
        return chrObj.GetComponent<Character>();
    }

}


public class CharacterDeploy : Conditionable
{
    public int id;
    public string name;
    public string resource;
}