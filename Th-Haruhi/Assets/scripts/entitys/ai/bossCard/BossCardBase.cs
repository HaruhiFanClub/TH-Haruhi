using UnityEngine;
using System.Collections;

public abstract class BossCardBase
{
    protected Boss Master;

    public EBossCardPhase Phase { set; get; }

    //符卡时间
    public abstract float TotalTime { get; }
    //出生位置
    public virtual Vector3 StartPos { get { return Vector3.zero; } }

    public int CurrentHp { set;  get; }
    public int MaxHp { set; get; }
    public bool CanShoot { set; get; }
    protected int ShootIdx { private set; get; }


    public virtual void Init(Boss enemy, int maxHp)
    {
        Master = enemy;
        CurrentHp = MaxHp = maxHp;
    }

    public void OnEnable(bool isFirstCard)
    {
        //3秒后开始
        Master.StartCoroutine(DoEnable(isFirstCard));

        if(StartPos != Vector3.zero)
        {
            Master.MoveToTarget(StartPos, 3f);
        }
    }

    private IEnumerator DoEnable(bool isFirstCard)
    {
        Master.Invisible = true;
        UIBattle.ShowBossTime(true, TotalTime);

        //血条 and boss背景处理
        switch (Phase)
        {
            case EBossCardPhase.One:
                Master.SetHpHudPointActive(true);
                Master.SetHpHudActive(true);
                break;
            case EBossCardPhase.Two:
                UIBossBg.Show(Master.Deploy.BossDraw);
                Master.SetHpHudPointActive(false);
                break;
            case EBossCardPhase.Single:
                Master.SetHpHudActive(true);
                break;
        }

        yield return new WaitForSeconds(isFirstCard ? 0.5f : 1.5f);

        Master.Invisible = false;

        yield return new WaitForSeconds(0.5f);

        Master.ShowCircleRaoDong(true);
        CanShoot = true;
    }

    public void OnDisable()
    {
        if(Phase == EBossCardPhase.One)
        {
            Master.SetHpHudPointActive(false);
        }
        else
        {
            Master.SetHpHudActive(false);
        }

        Master.ShowCircleRaoDong(false);
        UIBossBg.FadeOut();
        UIBattle.ShowBossTime(false);
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