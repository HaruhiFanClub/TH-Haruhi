using UnityEngine;


public class ShootAI_S01 : ShootAI_Base
{
    private float _nextShootTime;
    private int _shootIdx;

    protected override float ShootDelay => 2f;

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);

    }

    public override void OnUpdate()
    {
        if (!CanShoot) return;

        if (Time.time <= _nextShootTime) return;

        var f1 = Quaternion.Euler(0, 0, _shootIdx * 29) * Master.transform.up;
        var data = MoveData.New(Master.transform.position, f1);
        BulletFactory.CreateBulletAndShoot(1001 + _shootIdx % 4, Master.transform, Layers.EnemyBullet, data);

        _shootIdx++;
        if (_shootIdx > 10000000)
            _shootIdx = 0;

        _nextShootTime = Time.time + GameSystem.FrameTime;
    }


    private void UpdateShoot()
    {
        if (Time.time > _nextShootTime)
        {
           

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
            
        }
    }

}