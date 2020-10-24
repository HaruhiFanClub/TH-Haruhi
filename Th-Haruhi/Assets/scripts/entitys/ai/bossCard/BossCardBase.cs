using UnityEngine;
using System.Collections;
using System;

public abstract class BossCardBase
{
    //符卡名称
    public abstract string CardName { get; }

    //符卡时间
    public abstract float TotalTime { get; }

    //符卡状态阶段
    public EBossCardPhase Phase = EBossCardPhase.One;
    public EBossCardPhase PrevCardPhase = EBossCardPhase.None;
    public EBossCardPhase NextCardPhase = EBossCardPhase.None;

    //难度设置
    protected abstract void InitDifficult(ELevelDifficult diff);

    //出生位置
    public virtual Vector3 StartPos { get { return Vector3.zero; } }

    public int CurrentHp { set;  get; }
    public int MaxHp { set; get; }

    public bool Inited { private set; get; }
    
    protected Boss Master;

    public virtual void Init(Boss enemy, int maxHp)
    {
        Master = enemy;
        CurrentHp = MaxHp = maxHp;
        InitPhase();
        InitDifficult(StageMgr.Data.Difficult);
    }

    protected abstract void InitPhase();

    public void OnEnable(bool isFirstCard)
    {
        Master.StartCoroutine(DoEnable(isFirstCard));

        if(StartPos != Vector3.zero)
        {
            Master.MoveToPos(StartPos, 60, MovementMode.MOVE_NORMAL);
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
                Master.ShowHpCircle(true, NextCardPhase == EBossCardPhase.Two);
                break;

            case EBossCardPhase.Two:
                UIBossBg.Show(Master.Deploy.BossDraw);
                UIBattle.ShowBossCard(CardName);
                Master.ShowHpCircle(PrevCardPhase != EBossCardPhase.One , PrevCardPhase == EBossCardPhase.One);
                break;
        }

        yield return new WaitForSeconds(isFirstCard ? 0.5f : 1.5f);

        Master.Invisible = false;

        yield return new WaitForSeconds(0.8f);

        Master.ShowCircleRaoDong(true);
        Inited = true;

        Start();
    }

    public void OnDisable()
    {
        Master.HideHpCircle();
        Master.RemoveAllTask();
        Master.ShowCircleRaoDong(false);
        UIBossBg.FadeOut();
        UIBattle.ShowBossTime(false);
        UIBattle.HideBossCard();
        Stop();
    }


    protected virtual void Start()
    {
    }

    protected virtual void Stop()
    {
    }


    public virtual void OnFixedUpdate()
    {
       
    }

    public virtual void OnDestroy()
    {
        
    }
}