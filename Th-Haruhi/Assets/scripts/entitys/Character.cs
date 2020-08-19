using UnityEngine;

public class Character : EntityBase
{
    //角色属性
    public CharacterProperty Data { private set; get; }

    public Animator Animator { private set; get; }

    private Rigidbody2D _rg;

    protected override void Awake()
    {
        base.Awake();
        Animator = GetComponent<Animator>();
        _rg = GetComponent<Rigidbody2D>();
        Data = GetComponent<CharacterProperty>();

        GameEventCenter.AddListener(GameEvent.StartMove, OnStartMove);
        GameEventCenter.AddListener(GameEvent.StopMove, OnStopMove);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEventCenter.RemoveListener(GameEvent.StartMove, OnStartMove);
        GameEventCenter.RemoveListener(GameEvent.StopMove, OnStopMove);
    }


    //根据摇杆方向决定移动方向
    private Vector2 _prevMoveFwd;
    private EMoveDir _lastMoveDir;
    private EMoveDir GetMoveDir(Vector2 forward)
    {
        EMoveDir moveDir;

        var absX = Mathf.Abs(forward.x);
        var absY = Mathf.Abs(forward.y);

        //同时按2个方向, 以后按的为准
        if (Mathf.Abs(_prevMoveFwd.x) < absX)
        {
            _lastMoveDir = forward.x < 0 ? EMoveDir.Left : EMoveDir.Right;
        }
        if (Mathf.Abs(_prevMoveFwd.y) < absY)
        {
            _lastMoveDir = forward.y < 0 ? EMoveDir.Down : EMoveDir.Up;
        }

        //left, right
        if (absX > absY)
        {
            moveDir = forward.x < 0 ? EMoveDir.Left : EMoveDir.Right;
        }
        //updown
        else if (absX < absY)
        {
            moveDir = forward.y < 0 ? EMoveDir.Down : EMoveDir.Up;
        }
        else
        {
            //同时按2个方向, 以后按的为准
            moveDir = _lastMoveDir;
        }
        _prevMoveFwd = forward;
        return moveDir;
    }

    //收到移动事件
    private void OnStartMove(object o)
    {
        var forward = (Vector2)o;
        Move(GetMoveDir(forward));
    }

    private void OnStopMove(object o)
    {
        _prevMoveFwd = Vector2.zero;
        StopMove();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateMove();
    }

    private bool _inMove;
    private Vector3 _moveFoward = Vector3.zero;

    public EMoveDir MoveDir
    {
        set
        {
            switch (value)
            {
                case EMoveDir.Idle:
                    _moveFoward = Vector3.zero;
                    break;
                case EMoveDir.Up:
                    Animator.Play("MoveUp");
                    _moveFoward.x = 0;
                    _moveFoward.y = 1;
                    break;
                case EMoveDir.Down:
                    Animator.Play("MoveDown");
                    _moveFoward.x = 0;
                    _moveFoward.y = -1;
                    break;
                case EMoveDir.Left:
                    Animator.Play("MoveLeft");
                    _moveFoward.x = -1;
                    _moveFoward.y = 0;
                    break;
                case EMoveDir.Right:
                    Animator.Play("MoveRight");
                    _moveFoward.x = 1;
                    _moveFoward.y = 0;
                    break;
            }
        }
    }

    public void Move(EMoveDir dir)
    {
        _inMove = true;
        MoveDir = dir;
    }

    public void StopMove()
    {
        MoveDir = EMoveDir.Idle;
        _inMove = false;
        Animator.SetFloat("MoveSpeedFactor", 0f);
    }

    private void UpdateMove()
    {
        var deltaTime = Time.deltaTime;
        if (_inMove)
        {
            var moveSpeed = GetMoveSpeed();
            var targetPos = transform.position + _moveFoward * deltaTime * GetMoveSpeed();
            _rg.MovePosition(targetPos);
            Animator.SetFloat("MoveSpeedFactor", moveSpeed  / 8f);
        }
    }

    private float GetMoveSpeed()
    {
        if (_inSand) return Data.MoveSpeed / 2f;
        return Data.MoveSpeed;
    }


    private bool _inSand;
    private void OnCollisionEnter2D(Collision2D collision)
    {
      
    }
  
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Layers.Sand)
        {
            _inSand = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Layers.Sand)
        {
            _inSand = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var region = collision.GetComponent<RegionBase>();
        if(region)
        {
            switch (region.Type)
            {
                case ERegionType.Born:
                    break;
                case ERegionType.ChangeScene:
                    GameWorld.EnterScene(region.SceneUrl, region.BornId);
                    break;
            }
        }
    }
}