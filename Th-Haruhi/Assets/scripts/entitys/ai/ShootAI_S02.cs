using UnityEngine;


public class ShootAI_S02 : AI_Base
{
    private int _nextShootFrame;
    private int _shootIdx;

    protected override float ShootDelay => 2f;

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);
        Master.MoveToTarget(Vector2Fight.New(0, 144f), 3);
    }

    private Vector3 _nextShootForward;
    private bool _inWait = true;
    private float _lastWaitTIme;

    public override void OnFixedUpdate()
    {
        if (!CanShoot) return;

        if(Time.time - _lastWaitTIme < 2f)
        {
            return;
        }

        if(_inWait)
        {
            if (Player.Instance)
            {
                _nextShootForward = Player.Instance.transform.position - Master.transform.position;
                _nextShootForward = _nextShootForward.normalized;
            }
            _inWait = false;
        }

        if(!_inWait && GameSystem.FixedFrameCount > _nextShootFrame)
        {
            var f1 = Quaternion.Euler(0, 0, -60f) * _nextShootForward;
            var bulletMoveData = MoveData.New(Master.transform.position, f1, 10f);
            bulletMoveData.HelixToward = MoveData.EHelixToward.Right;
            bulletMoveData.HelixRefretFrame = 20;
            bulletMoveData.EulurPerFrame = 6f;

            BulletFactory.CreateBulletShoot(1009, Master.transform, Layers.EnemyBullet, bulletMoveData);

            var f2 = Quaternion.Euler(0, 0, 60f) * _nextShootForward;
            var bulletMoveData2 = MoveData.New(Master.transform.position, f2, 10f);
            bulletMoveData2.HelixToward = MoveData.EHelixToward.Left;
            bulletMoveData2.HelixRefretFrame = 20;
            bulletMoveData2.EulurPerFrame = 6f;
            BulletFactory.CreateBulletShoot(1009, Master.transform, Layers.EnemyBullet, bulletMoveData2);

            _nextShootFrame = GameSystem.FixedFrameCount + 2;

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