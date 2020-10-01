using UnityEngine;

public class MoveData : IPool
{
    public static MoveData New(Vector3 startPos, Vector3 forward, float speed)
    {
        var bulletData = Pool.New<MoveData>() as MoveData;
        bulletData.StartPos = startPos;
        bulletData.Forward = forward;
        bulletData.Speed = speed;
        return bulletData;
    }

    public enum EHelixToward
    { 
        None = 0,
        Right = 1,
        Left = -1
    }

    public float Speed;          //初速度
    public Vector3 StartPos;
    public Vector3 Forward;
    public EHelixToward HelixToward;  //螺旋方向 0:不螺旋 1：右边 -1：左边
    public int HelixRefretFrame;      //螺旋翻转时间
    public float EulurPerFrame;       //螺旋每帧改变角度 < 0 左   > 0 向右

    public override void Init()
    {
        Reset();
    }

    public override void OnDestroy()
    {
        Reset();
    }

    public override void Recycle()
    {
        Reset();
    }

    public override void Reset()
    {
        StartPos = Vector3.zero;
        Forward = Vector3.zero;
        HelixRefretFrame = 0;
        EulurPerFrame = 0;
        HelixToward = EHelixToward.None;
    }
}
