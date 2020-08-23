
using UnityEngine;

public class Vector2Fight
{
    //战斗区域坐标, 将战斗区域换算为 100 X 120 的范围

    public static Vector2 Center = new Vector2(-4F, 0F);

    //换算 X: 100 -> 8  Y: 120 -> 9.6
    private static float _trans = 12.5f;    

    public static Vector2 New(float x, float y)
    {
        return new Vector2(x / _trans, y / _trans) + Center;
    }

}