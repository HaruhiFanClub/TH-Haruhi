using System.Collections;
using UnityEngine;

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
       // StartCoroutine(TestCreateEnemy());
    }


    private static IEnumerator TestCreateEnemy()
    {
        yield return new WaitForSeconds(3f);
        var enemyId = 102;
        yield return Enemy.Create(enemyId);
    }
    protected override IEnumerator LoopLevel()
    {
        yield return new WaitForSeconds(3f);
        yield return Enemy.Create(102, 0, 70);
    }
}
