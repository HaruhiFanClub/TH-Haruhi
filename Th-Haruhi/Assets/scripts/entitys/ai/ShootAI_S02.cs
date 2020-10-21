using UnityEngine;


public class ShootAI_S02 : AI_Base
{
    private int _nextShootFrame;
    private int _shootIdx;

    protected override float ShootDelay => 2f;

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);
    }

    private Vector3 _nextShootForward;
    private bool _inWait = true;
    private float _lastWaitTIme;

    public override void OnFixedUpdate()
    {
        if (!CanShoot) return;

        if (Time.time - _lastWaitTIme < 2f)
        {
            return;
        }
    }

}