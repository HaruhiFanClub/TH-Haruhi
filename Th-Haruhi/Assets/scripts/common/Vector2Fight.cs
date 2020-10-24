
using UnityEngine;

public class Vector2Fight
{
    public static float Left = -192f;
    public static float Right = 192f;
    public static float Up = 224f;
    public static float Down = -224f;

    public static float ScaleFactor = 23.2f;

    /*

    public static Vector3 Center = new Vector2(-4F, 0F);

    //-192 192  -224 224
    private static float _trans = 24f;    

    public static Vector3 NewWorld(float x, float y)
    {
        return new Vector3(x / _trans, y / _trans) + Center;
    }

    public static Vector3 NewLocal(float x, float y)
    {
        return new Vector3(x / _trans, y / _trans);
    }

    public static Vector3 WorldPosToFightPos(Vector3 pos)
    {
        return new Vector3(pos.x * _trans, pos.y * _trans) - Center * _trans;
    }
    */
    public static Vector2 FightPosToUiPos(Vector3 pos)
    {
        var canvas = UiManager.GetCanvas(UiLayer.Battle);
        var w = canvas.sizeDelta.x / 2f;
        var h = canvas.sizeDelta.y /2f;

        //xRange : -192 --> 192
        //yRange : -224 --> 224

        //xRange : -220 --> 400
        //yRange : -230 --> 230

        var xRatio = w / 310f;
        var yRatio = h / 230f;
        return new Vector3((pos.x - 90f) * xRatio, pos.y * yRatio);

    }
}