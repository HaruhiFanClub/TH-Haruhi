using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Scene : MonoBehaviour
{
    public Transform Block1;
    public Transform Block2;

    private Transform CurBlock;

    private void Awake()
    {
        CurBlock = Block1;
    }

    void Update()
    {
        var speed = 7;
        var delta = Time.deltaTime;
        var pos1 = Block1.position;
        var pos2 = Block2.position;
        pos1.z -= delta * speed;
        pos2.z -= delta * speed;

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
