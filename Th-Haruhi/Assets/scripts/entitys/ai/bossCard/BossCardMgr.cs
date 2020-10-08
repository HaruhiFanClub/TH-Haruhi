using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum EBossCardPhase
{
    One,
    Two,
    Single
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
                var card = Common.CreateInstance(strClass) as BossCardBase;
                if (card != null)
                {
                    card.Init(Master, perCardHp);
                    _cardList.Add(card);
                }
            }
        }

        if(_cardList.Count == 1)
        {
            _cardList[0].Phase = EBossCardPhase.Single;
        }
        else
        {
            for (int i = 0; i < _cardList.Count; i++) 
            {
                _cardList[i].Phase = i % 2 == 0 ? EBossCardPhase.One : EBossCardPhase.Two;
            }
        }


        GameEventCenter.AddListener(GameEvent.OnPlayerDead, OnPlayerDead);
    }

    public bool IsSingleCard()
    {
        return _cardList.Count == 1;
    }   

    private void OnPlayerDead(object o)
    {
        if (_currCard != null)
        {
            _currCard.BanShoot = true;
            Master.StartCoroutine(ResetBanShoot());
        }
    }

    private IEnumerator ResetBanShoot()
    {
        yield return new WaitForSeconds(2f);
        if (_currCard != null)
        {
            _currCard.BanShoot = false;
        }
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
                    //第一阶段显示80%
                    return 0.2f + (_currCard.CurrentHp / (float)_currCard.MaxHp) * 0.8f;
                case EBossCardPhase.Two:
                    //第二阶段显示后面20%
                    return (_currCard.CurrentHp / (float)_currCard.MaxHp) * 0.2f;
                case EBossCardPhase.Single:
                    //如果boss总共就1个阶段，直接显示
                    return _currCard.CurrentHp / (float)_currCard.MaxHp;
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
        if(_currCard != null)
        {
            //销毁子弹
            BulletExplosion.Create(Master.transform.position, 0.02f);

            //播放音效(success or failed)
            //todo
            Sound.PlayUiAudioOneShot(106);

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
        GameEventCenter.RemoveListener(GameEvent.OnPlayerDead, OnPlayerDead);
    }
}