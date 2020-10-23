using System.Collections;
using UnityEngine;

public class Stage2 : StageBase
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
        if (!_bGameOver)
        {
            _bGameOver = true;
            StartCoroutine(DoGameOver());
        }
    }

    private bool _bGameOver;
    private IEnumerator DoGameOver()
    {
        yield return new WaitForSeconds(3f);
        GameSystem.ShowTitle();
    }

    protected override IEnumerator LoopLevel()
    {
        yield return new WaitForSeconds(2f);
        yield return Enemy.Create(1002);
    }
}
