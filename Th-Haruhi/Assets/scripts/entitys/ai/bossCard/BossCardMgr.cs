﻿using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum EBossCardPhase
{
    None,
    One,
    Two,
}

public class BossCardMgr
{
    protected Boss Master;

    private BossCardBase _currCard;
    private List<BossCardBase> _cardList = new List<BossCardBase>();
    private float _cardStartTime;

    public void Init(Boss enemy, int maxHp)
    {
        Master = enemy;

        var deploy = Master.Deploy;
        _cardList.Clear();

        int perCardHp = maxHp / deploy.BossCard.Length;

        for (int i = 0; i < deploy.BossCard.Length; i++)
        {
            var strClass = deploy.BossCard[i];
            if (!string.IsNullOrEmpty(strClass))
            {
                BossCardBase card = Common.CreateInstance(strClass) as BossCardBase;
                if (card != null)
                {
                    card.Init(Master, perCardHp);
                    _cardList.Add(card);
                }
            }
        }

        //设置前后符卡的状态信息
        for(int i = 0; i < _cardList.Count; i++)
        {
            var prev = EBossCardPhase.None;
            var next = EBossCardPhase.None;
            if (i > 0) 
            {
                prev = _cardList[i - 1].Phase;
            }
            if (i < _cardList.Count - 1) 
            {
                next = _cardList[i + 1].Phase;
            }
            _cardList[i].PrevCardPhase = prev;
            _cardList[i].NextCardPhase = next;
        }
    }

    public bool IsSingleCard()
    {
        return _cardList.Count == 1;
    }   

    public void CalculateHp(int atk)
    {
        if(_currCard != null)
        {
            _currCard.CurrentHp -= atk;
            if(_currCard.CurrentHp <= 0)
            {
                ChangeToNextCard();
            }
        }
    }

    public float GetHpPercent()
    {
        if (_currCard != null)
        {
            switch (_currCard.Phase)
            {
                case EBossCardPhase.One:

                    if(_currCard.NextCardPhase != EBossCardPhase.Two)
                    {
                        return _currCard.CurrentHp / (float)_currCard.MaxHp;
                    }
                    else
                    {
                        //第一阶段显示80%
                        return 0.2f + (_currCard.CurrentHp / (float)_currCard.MaxHp) * 0.8f;
                    }
                    
                case EBossCardPhase.Two:

                    if(_currCard.PrevCardPhase != EBossCardPhase.One)
                    {
                        return _currCard.CurrentHp / (float)_currCard.MaxHp;
                    }
                    else
                    {
                        //第二阶段显示后面20%
                        return (_currCard.CurrentHp / (float)_currCard.MaxHp) * 0.2f;
                    }
            }
        }
        return 1f;
    }

    public void OnStartFight()
    {
        ChangeToNextCard();
    }

    private void ChangeToNextCard()
    {
        //销毁当前Card
        var isFirstCard = true;
        EBossCardPhase _prevCardPhase = EBossCardPhase.None;
        if(_currCard != null)
        {
            //销毁子弹
            BulletExplosion.Create(Master.transform.position, 0.02f);

            //播放音效(success or failed)
            //todo
            Sound.PlayTHSound("cardget");

            _prevCardPhase = _currCard.Phase;
            _currCard.OnDisable();
            _currCard.OnDestroy();
            _currCard = null;
            isFirstCard = false;
        }
  
        //有剩余符卡，切换到下一个
        if (_cardList.Count > 0)
        {
            _currCard = _cardList[0];
            _cardList.RemoveAt(0);
            _currCard.OnEnable(isFirstCard);
            _cardStartTime = Time.time;


            //播放收缩or扩大特效
            if(!isFirstCard)
            {
                if (_currCard.Phase == EBossCardPhase.Two)
                {
                    Master.PlayShirnkEffect();
                }
                else
                {
                    Master.PlayShirnkEffect(true);
                }
            }
        }
        else
        {
            //所有符卡用完了，直接死亡
            Master.SelfDie();
        }
    }

    public void OnFixedUpdate()
    {
        if(_currCard != null)
        {
            _currCard.OnFixedUpdate();
            if(Time.time - _cardStartTime > _currCard.TotalTime)
            {
                ChangeToNextCard();
            }
        }
    }


   
    public void OnDestroy()
    {
        _cardList.Clear();
        _currCard?.OnDestroy();
        _currCard = null;
    }
}