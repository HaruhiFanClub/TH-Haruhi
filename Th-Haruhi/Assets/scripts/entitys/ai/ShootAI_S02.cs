using UnityEngine;


public class ShootAI_S02 : AI_Base
{
    private float _nextShootTime;
    private int _shootIdx;

    protected override float ShootDelay => 2f;

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);

    }

    private Vector3 _nextShootForward;
    private bool _inWait = true;
    private float _lastWaitTIme;

    public override void OnUpdate()
    {
        if (!CanShoot) return;

        if(Time.time - _lastWaitTIme < 2f)
        {
            return;
        }

        if(_inWait)
        {
            if(Player.Instance)
                _nextShootForward = Player.Instance.transform.position - Master.transform.position;
            _inWait = false;
        }

        if(!_inWait && Time.time > _nextShootTime)
        {
            var f1 = Quaternion.Euler(0, 0, -60f) * _nextShootForward;
            var bulletMoveData = MoveData.New(Master.transform.position, f1, 5f);
            bulletMoveData.HelixToward = MoveData.EHelixToward.Right;
            bulletMoveData.HelixRefretFrame = 20;
            bulletMoveData.EulurPerFrame = 6f;

            BulletFactory.CreateBulletAndShoot(201, Master.transform, Layers.EnemyBullet, bulletMoveData);

            var f2 = Quaternion.Euler(0, 0, 60f) * _nextShootForward;
            var bulletMoveData2 = MoveData.New(Master.transform.position, f2, 5f);
            bulletMoveData2.HelixToward = MoveData.EHelixToward.Left;
            bulletMoveData2.HelixRefretFrame = 20;
            bulletMoveData2.EulurPerFrame = 6f;
            BulletFactory.CreateBulletAndShoot(201, Master.transform, Layers.EnemyBullet, bulletMoveData2);

            _nextShootTime = Time.time + GameSystem.FrameTime * 5;

            _shootIdx++;
            //每20发子弹停2秒
            if (_shootIdx % 20 == 0)
            {
                _inWait = true;
                _lastWaitTIme = Time.time;
            }
        }
    }

}