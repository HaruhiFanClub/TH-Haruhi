using System.Collections.Generic;
using UnityEngine;

public class BossCardMgr
{
    protected Boss Master;

    private BossCardBase _currCard;
    private List<BossCardBase> _cardList = new List<BossCardBase>();
    private float _cardStartTime;

    public void Init(Boss enemy)
    {
        Master = enemy;

        var deploy = Master.Deploy;
        _cardList.Clear();

        for (int i = 0; i < deploy.BossCard.Length; i++)
        {
            var strClass = deploy.BossCard[i];
            if (!string.IsNullOrEmpty(strClass))
            {
                var card = Common.CreateInstance(strClass) as BossCardBase;
                if (card != null)
                {
                    card.Init(Master);
                    _cardList.Add(card);
                }
            }
        }
    }

    public void OnStartFight()
    {
        ChangeToNextCard();
    }

    private void ChangeToNextCard()
    {
        _currCard?.OnDisable();

        //有剩余符卡，切换到下一个
        if(_cardList.Count > 0)
        {
            _currCard = _cardList[0];
            _cardList.RemoveAt(0);
            _currCard.OnEnable();
            _cardStartTime = Time.time;
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

    public void OnDead()
    {
        _currCard?.OnDisable();
        _cardList.Clear();
        _currCard?.OnDestroy();
        _currCard = null;
    }

    public void OnDestroy()
    {
        _cardList.Clear();
        _currCard?.OnDestroy();
        _currCard = null;
    }
}