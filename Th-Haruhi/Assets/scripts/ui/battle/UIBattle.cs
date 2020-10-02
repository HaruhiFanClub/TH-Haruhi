
using System;
using System.Collections;
using UnityEngine;

public class UIBattle : UiInstance
{
    public static UIBattle Instance;

    private UIBattleComponent _bind;

    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _bind = GetComponent<UIBattleComponent>();
        Instance = this;

        _bind.BossCard.SetActiveSafe(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }

    protected override void Update()
    {
        base.Update();
        UpdateBossTime();
    }

    private float _bossTimeLeft;
    private bool _bossTimeActive;
    public void SetBossTimeActive(bool b, float leftSec = 0)
    {
        _bossTimeActive = b;
        _bossTimeLeft = leftSec;
        _bind.BossCard.SetActiveSafe(b);
    }

    private void UpdateBossTime()
    {
        if (!_bossTimeActive) return;
        _bossTimeLeft -= Time.deltaTime;
        if (_bossTimeLeft <= 0)
        {
            _bossTimeLeft = 0;
        }

        var sec = Mathf.FloorToInt(_bossTimeLeft);
        var sec2 = (_bossTimeLeft - sec) * 100;
        _bind.BossCardSec1.text = string.Format("{0:00}.", sec);
        _bind.BossCardSec2.text = string.Format("{0:00}", sec2);
    }
}
