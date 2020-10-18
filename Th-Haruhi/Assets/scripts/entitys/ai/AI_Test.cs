using UnityEngine;


public class AI_Test : AI_Base
{
    private float _nextShootTime;
    private int _shootIdx;

    protected override float ShootDelay => 2f;

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);


    }

    public override void OnFixedUpdate()
    {
        if (!CanShoot) return;

        
    }
}