
using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIBattle : UiInstance
{
    
    //------------------- start static ------------------//

    public static UIBattle Instance;

    //显示隐藏剩余时间
    public static void ShowBossTime(bool b, float leftSec = 0)
    {
        if(Instance)
        {
            Instance.ShowLeftTime(b, leftSec);
        }
    }

    //显示隐藏boss标记
    public static void SetBossMarkPos(Vector3 bossPos)
    {
        if(Instance)
        {
            var uiPos = Vector2Fight.WordPosToUIPos(bossPos);
            var pos = Instance._bind.EnemyMark.anchoredPosition;
            pos.x = uiPos.x;
            Instance._bind.EnemyMark.anchoredPosition = pos;
        }
    }

    //更新boss标记位置
    public static void SetBossMarkActive(bool b)
    {
        if (Instance)
        {
            Instance._bind.EnemyMark.gameObject.SetActiveSafe(b);
        }
    }

    //显示boss符卡信息
    public static void ShowBossCard(string name)
    {
        if(Instance)
        {
            Instance.StartCoroutine(Instance.ShowCard(name));
        }
    }

    //隐藏boss符卡信息
    public static void HideBossCard()
    {
        if(Instance)
        {
            Instance.HideCard();
        }
    }


    //------------------- end static ------------------//


    private UIBattleComponent _bind;
    private Vector2 _defaultLeftTimePos;
    private Vector2 _defaultCardNamePos;
    private Vector2 _defaultCardBonusPos;

    private float _timeLeft;
    private bool _leftActive;
    private float _lastTimeOutTime;
    private bool _bShowingCardName;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIBattleComponent>();
        Instance = this;

        _bind.LeftTimeRoot.gameObject.SetActiveSafe(false);

        _defaultLeftTimePos = _bind.LeftTimeRoot.anchoredPosition;
        _defaultCardNamePos = _bind.CardNameRoot.anchoredPosition;
        _defaultCardBonusPos = _bind.CardBonusRoot.anchoredPosition;

        RefreshDifficult();
        RefreshLifeCount();

        GameEventCenter.AddListener(GameEvent.LifeCountChanged, RefreshLifeCount);

        InitDebug();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEventCenter.RemoveListener(GameEvent.LifeCountChanged, RefreshLifeCount);
        Instance = null;
    }

    private void RefreshLifeCount(object o = null)
    {
        var curLifeCount = StageMgr.Data.LeftLifeCount;
        for(int i = 0; i < _bind.LifeList.Count; i++)
        {
            _bind.LifeList[i].SetActiveSafe(curLifeCount - 1 > i);
        }
    }

    private void RefreshDifficult()
    {
        var diff = StageMgr.Data.Difficult;
        switch (diff)
        {
            case ELevelDifficult.Easy:
                _bind.Difficult.text = "Easy";
                _bind.DifficultOutLine.effectColor = ColorUtility.EasyOutLine;
                break;
            case ELevelDifficult.Normal:
                _bind.Difficult.text = "Normal";
                _bind.DifficultOutLine.effectColor = ColorUtility.NormalOutLine;
                break;
            case ELevelDifficult.Hard:
                _bind.Difficult.text = "Hard";
                _bind.DifficultOutLine.effectColor = ColorUtility.HardOutLine;
                break;
            case ELevelDifficult.Lunatic:
                _bind.Difficult.text = "Lunatic";
                _bind.DifficultOutLine.effectColor = ColorUtility.LunaticOutLine;
                break;
            case ELevelDifficult.Extra:
                _bind.Difficult.text = "Extra";
                _bind.DifficultOutLine.effectColor = ColorUtility.LunaticOutLine;
                break;
        }

    }

    protected override void Update()
    {
        base.Update();
        UpdateLeftTime();
        UpdateGrazeCount();
    }

    private void UpdateGrazeCount()
    {
        _bind.Graze.text = StageMgr.Data.GrazeCount.ToString("N0");
    }

    private void ShowLeftTime(bool b, float leftSec = 0)
    {
        _leftActive = b;
        _timeLeft = leftSec;
        _bind.LeftTimeRoot.gameObject.SetActiveSafe(b);
    }

    private void UpdateLeftTime()
    {
        if (!_leftActive) return;
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0)
        {
            _timeLeft = 0;
        }

        var sec = Mathf.FloorToInt(_timeLeft);
        var sec2 = (_timeLeft - sec) * 100;
        var color = _timeLeft < 10f ? "red" : "white";

        _bind.LeftTimeSec1.text = string.Format("<color={1}>{0:00}.</color>", sec, color);
        _bind.LeftTimeSec2.text = string.Format("<color={1}>{0:00}</color>", sec2, color);

        //音效
        if(_timeLeft > 0 && _timeLeft < 10)
        {
            var cd = 1f;
            if (Time.time - _lastTimeOutTime > cd)
            {
                Sound.PlayUiAudioOneShot(1005);
                _lastTimeOutTime = Time.time;
            }
        }
    }

   
    public IEnumerator ShowCard(string name)
    {
        if(_bShowingCardName)
        {
            yield break;
        }
        _bShowingCardName = true;

        //时间下移
        _bind.LeftTimeRoot.DOAnchorPos(new Vector2(_defaultLeftTimePos.x, -120f), 0.5f);

        _bind.CardName.text = name;

        //动画过程
        _bind.CardNameRoot.gameObject.SetActiveSafe(true);
        _bind.CardBonusRoot.gameObject.SetActiveSafe(true);
        _bind.CardBonusRoot.anchoredPosition = _defaultCardBonusPos;

        var startPos = new Vector2(-800f, -300f);
        var posTarget1 = new Vector2(_defaultCardNamePos.x, -300f);

        _bind.CardBonusAlpha.alpha = 0;
        _bind.CardNameAlpha.alpha = 0;

        _bind.CardNameRoot.anchoredPosition = startPos;
        _bind.CardNameRoot.localScale = Vector3.one * 5f;

        var aniSec1 = 0.5f;
        _bind.CardNameAlpha.DOFade(1f, aniSec1);
        _bind.CardNameRoot.DOAnchorPos(posTarget1, aniSec1);
        _bind.CardNameRoot.DOScale(Vector3.one, aniSec1);

        yield return new WaitForSeconds(aniSec1 + 1f);

        _bind.CardNameRoot.DOAnchorPos(_defaultCardNamePos, 0.5f);

        yield return new WaitForSeconds(0.3f);
        _bind.CardBonusAlpha.DOFade(1f, 0.3f);
    }

    public void HideCard()
    {
        if(!_bShowingCardName)
        {
            return;
        }

        _bShowingCardName = false;

        //时间下移
        _bind.LeftTimeRoot.DOAnchorPos(_defaultLeftTimePos, 0.5f);

        var pos1 = _defaultCardNamePos;
        pos1.x += 500;
        _bind.CardNameRoot.DOAnchorPos(pos1, 0.5f).onComplete = () =>
        {
            _bind.CardNameRoot.gameObject.SetActiveSafe(false);
        };

        var pos2 = _defaultCardBonusPos;
        pos2.x += 500;
        _bind.CardBonusRoot.DOAnchorPos(pos1, 0.5f).onComplete = () =>
        {
            _bind.CardBonusRoot.gameObject.SetActiveSafe(false);
        };
    }

    //debug

    private bool _inInvincible;
    private bool _inSlow;

    private void InitDebug()
    {
        /*
        if (!Debug.isDebugBuild)
        {
            _bind.DebugWudi.SetActiveSafe(false);
            return;
        }
        */

        RefreshDebugBtn();
        _bind.DebugTest.onClick.AddListener(() =>
        {
          
        });

        _bind.DebugSlow.onClick.AddListener(() =>
        {
            _inSlow = !_inSlow;
            TimeScaleManager.SetTimeScaleForDebug(_inSlow ? 0.1f : 1f);
            RefreshDebugBtn();
        });

        _bind.DebugWudi.onClick.AddListener(() =>
        {
            if (StageMgr.MainPlayer != null)
            {
                _inInvincible = !_inInvincible;
                if (_inInvincible)
                {
                    StageMgr.MainPlayer.SetInvincibleTime(100000);
                }
                else
                {
                    StageMgr.MainPlayer.Invincible = false;
                }
                RefreshDebugBtn();
            }
        });
    }

    private void RefreshDebugBtn()
    {
        var txt = _bind.DebugWudi.GetComponentInChildren<UiText>();
        if(txt)
        {
            txt.color = _inInvincible ? Color.green : Color.white;
            txt.text = _inInvincible ? "无敌ON" : "无敌OFF";
        }

        var txt2 = _bind.DebugSlow.GetComponentInChildren<UiText>();
        if (txt2)
        {
            txt2.color = _inSlow ? Color.green : Color.white;
            txt2.text = _inSlow ? "Slow On" : "Slow Off";
        }
    }
}
