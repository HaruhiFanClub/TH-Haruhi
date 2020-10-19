using UnityEngine;

public class MoveData
{
    public static MoveData New(Vector3 startPos, Vector3? forward = null, float speed = 0, float accelertion = 0, float endSpeed = 0)
    {
        var data = new MoveData();
        if(forward != null)
        {
            data.Forward = (Vector3)forward;
        }
       
        data.StartPos = startPos;
        data.Speed = speed;
        data.Acceleration = accelertion;
        data.EndSpeed = endSpeed;
        return data;
    }

    public enum EHelixToward
    { 
        None = 0,
        Right = 1,
        Left = -1
    }

    public class AngleAcclerationData
    {
        public float TargetAngle;
    }


    public float Speed;          //初速度
    public float Acceleration;
    public float EndSpeed;
    public bool AccToStop;
    public Vector3 StartPos;
    public Vector3 Forward;

    //角度加速度
    public AngleAcclerationData AngleData;

    public EHelixToward HelixToward;  //螺旋方向 0:不螺旋 1：右边 -1：左边
    public int HelixRefretFrame;      //螺旋翻转时间
    public float EulurPerFrame;       //螺旋每帧改变角度 < 0 左   > 0 向右
}
