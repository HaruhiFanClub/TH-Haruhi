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
        DOVirtual.DelayedCall(2f, () =>
        {
            UiManager.Show<UIStageAllClear>();
        }, false);
        
       // StartCoroutine(TestCreateEnemy());
    }

    protected override IEnumerator LoopLevel()
    {
        //create boss
        yield return new WaitForSeconds(3f);
        yield return Enemy.Create(1001, 80, 150);
    }
}
