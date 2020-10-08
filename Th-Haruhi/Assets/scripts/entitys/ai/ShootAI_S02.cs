using UnityEngine;


public class ShootAI_S02 : AI_Base
{
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