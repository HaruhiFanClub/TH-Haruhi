using UnityEngine;
using System.Collections;

public abstract class BossCardBase
{
    protected Boss Master;
    public abstract float TotalTime { get; }

    public bool CanShoot { set; get; }


    public virtual void Init(Boss enemy)
    {
        Master = enemy;
        GameEventCenter.AddListener(GameEvent.DisableEnemyShoot, DisableEnemyShoot);
        GameEventCenter.AddListener(GameEvent.EnableEnemyShoot, EnableEnemyShoot);
    }

    private void DisableEnemyShoot(object o)
    {
        CanShoot = false;
    }

    private void EnableEnemyShoot(object o)
    {
        CanShoot = true;
    }

    public virtual void OnEnable()
    {
        CanShoot = true;
        UIBattle.Instance?.SetBossTimeActive(true, TotalTime);
    }

    public virtual void OnDisable()
    {
        UIBattle.Instance?.SetBossTimeActive(false);
    }

    public virtual void OnFixedUpdate()
    {
  
    }

    public virtual void OnDestroy()
    {

        GameEventCenter.RemoveListener(GameEvent.DisableEnemyShoot, DisableEnemyShoot);
        GameEventCenter.RemoveListener(GameEvent.EnableEnemyShoot, EnableEnemyShoot);
    }
}