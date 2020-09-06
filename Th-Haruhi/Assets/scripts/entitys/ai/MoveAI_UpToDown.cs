using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class MoveAI_UpToDown : MoveAI_Base
{

    private Vector2 BornPosFight = new Vector2(0, 100);
    protected override Vector2 BornPos => Vector2Fight.New(BornPosFight.x, BornPosFight.y);

    public override void Init(Enemy enemy)
    {
        base.Init(enemy);
        var f1 = Quaternion.Euler(0, 0, -90f) * Vector3.down;

        var moveData = MoveData.New(Master.transform.position, f1);
        moveData.HelixToward = MoveData.EHelixToward.Right;
        moveData.HelixRefretFrame = 40;
        moveData.EulurPerFrame = 3f;

        Master.Move(moveData, 5); 
    }

}