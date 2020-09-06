using System.Collections;
using UnityEngine;

public class Stage2 : StageBase
{
    protected override void Awake()
    {
        base.Awake();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }



    protected override IEnumerator LoopLevel()
    {
        yield return new WaitForSeconds(2f);
        yield return Enemy.Create(101);
    }
}
