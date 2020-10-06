using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Stage1 : StageBase
{
    protected override void Awake()
    {
        base.Awake();
        GameEventCenter.AddListener(GameEvent.OnEnemyDie, OnEnemyDie);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEventCenter.RemoveListener(GameEvent.OnEnemyDie, OnEnemyDie);
    }

    //测试用
    private void OnEnemyDie(object o)
    {
        if(!_bGameOver)
        {
            _bGameOver = true;
            StartCoroutine(DoGameOver());
        }
       // StartCoroutine(TestCreateEnemy());
    }

    private bool _bGameOver;
    private IEnumerator DoGameOver()
    {
        yield return new WaitForSeconds(0.2f);
        UiManager.Show<UIStageClear>();
        yield return new WaitForSeconds(2f);
        if(UIStageClear.Instance != null)
        {
            UIStageClear.Instance.FadeOutClose();
        }

        yield return new WaitForSeconds(1f);
        UiManager.Show<UIStageAllClear>();
    }

    protected override IEnumerator LoopLevel()
    {
        //create boss
        yield return new WaitForSeconds(3f);
        yield return Enemy.Create(1001, 80, 150);
    }
}
