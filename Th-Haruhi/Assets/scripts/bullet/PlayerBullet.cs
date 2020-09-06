using UnityEngine;
using System.Collections;

public class PlayerBullet : Bullet
{
    private bool _bHitEnemy;
    private float _hitEnemyTime;
    public override void OnBulletHitEnemy()
    {
        _bHitEnemy = true;
        _hitEnemyTime = Time.time;

        //击中敌人后速度减慢
        BulletSpeed = 10f;
        Renderer.material.SetFloat("_Brightness", 2f);
    }

    protected override void Update()
    {
        base.Update();
        if(_bHitEnemy && Time.time - _hitEnemyTime > 0.1f)
        {
            base.OnBulletHitEnemy();
        }
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        _bHitEnemy = false;
        Renderer.material.SetFloat("_Brightness", 1f);
    }
}
