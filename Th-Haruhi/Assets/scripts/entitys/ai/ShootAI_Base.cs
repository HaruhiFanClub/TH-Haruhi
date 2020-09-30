using UnityEngine;
using System.Collections;

public abstract class AI_Base 
{
    protected Enemy Master;
    protected bool CanShoot;
    protected abstract float ShootDelay { get; }

    public virtual void Init(Enemy enemy)
    {
        Master = enemy;
        GameSystem.CoroutineStart(SetCanShoot());
    }

    private IEnumerator SetCanShoot()
    {
        yield return new WaitForSeconds(ShootDelay);
        CanShoot = true;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnDestroy()
    {

    }
}