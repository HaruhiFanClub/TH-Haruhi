using UnityEngine;
using System.Collections;

public abstract class BossCardBase
{
    public enum ECardPhase
    {
        One,
        Two
    }

    protected Boss Master;
    public abstract float TotalTime { get; }
    public int CurrentHp { set;  get; }
    public int MaxHp { set; get; }

    public bool CanShoot { set; get; }

    protected int ShootIdx { private set; get; }


    public virtual void Init(Boss enemy, int maxHp)
    {
        Master = enemy;
        CurrentHp = MaxHp = maxHp;
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
        ShootIdx++;
        if (ShootIdx > 10000000)
            ShootIdx = 0;
    }

    public virtual void OnDestroy()
    {
        
    }
}