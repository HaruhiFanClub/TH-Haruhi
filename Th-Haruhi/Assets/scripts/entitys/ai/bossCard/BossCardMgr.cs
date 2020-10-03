using System.Collections.Generic;
using UnityEngine;

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



        GameEventCenter.AddListener(GameEvent.DisableEnemyShoot, DisableEnemyShoot);
        GameEventCenter.AddListener(GameEvent.EnableEnemyShoot, EnableEnemyShoot);
    }

    private void DisableEnemyShoot(object o)
    {
        if (_currCard != null)
        {
            _currCard.CanShoot = false;
        }
    }

    private void EnableEnemyShoot(object o)
    {
        if (_currCard != null)
        {
            _currCard.CanShoot = true;
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
            return _currCard.CurrentHp / (float)_currCard.MaxHp;
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
        if(_currCard != null)
        {
            _currCard.OnDisable();
            _currCard.OnDestroy();
            _currCard = null;

            //销毁子弹
            BulletExplosion.Create(Master.transform.position, 0.15f);
        }

        //有剩余符卡，切换到下一个
        if (_cardList.Count > 0)
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

    public void OnDestroy()
    {
        _cardList.Clear();
        _currCard?.OnDestroy();
        _currCard = null;
        GameEventCenter.RemoveListener(GameEvent.DisableEnemyShoot, DisableEnemyShoot);
        GameEventCenter.RemoveListener(GameEvent.EnableEnemyShoot, EnableEnemyShoot);
    }
}