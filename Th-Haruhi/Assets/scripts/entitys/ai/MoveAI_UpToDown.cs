using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class MoveAI_UpToDown : MoveAI_Base
{

    private Vector2 BornPosFight = new Vector2(0, 144f);
    protected override Vector2 BornPos => Vector2Fight.NewWorld(BornPosFight.x, BornPosFight.y);

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);
        var f1 = Quaternion.Euler(0, 0, -90f) * Vector3.down;
    }

}