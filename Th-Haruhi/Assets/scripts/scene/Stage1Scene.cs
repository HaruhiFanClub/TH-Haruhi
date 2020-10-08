using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Scene : StageSceneBase
{
    public Transform Block1;
    public Transform Block2;

    private Transform CurBlock;

    protected override void Awake()
    {
        base.Awake();
        CurBlock = Block1;
    }

    protected override void Update()
    {
        base.Update();

        var delta = Time.deltaTime;
        var pos1 = Block1.position;
        var pos2 = Block2.position;
        pos1.z -= delta * Speed;
        pos2.z -= delta * Speed;

        Block1.position = pos1;
        Block2.position = pos2;

        if(CurBlock.position.z < 60)
        {
            var p = CurBlock.position;
            p.z = 200;
            CurBlock.position = p;

            if (CurBlock == Block1)
                CurBlock = Block2;
            else if (CurBlock == Block2)
                CurBlock = Block1;
        }

    }
}
