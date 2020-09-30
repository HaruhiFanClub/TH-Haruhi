using UnityEngine;


public class AI_Boss1 : AI_Base
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