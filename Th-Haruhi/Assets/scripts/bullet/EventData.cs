using System;
using UnityEngine;


public class EventData 
{
    public enum EventType
    {
        Frame_ChangeSpeed,
        Frame_ChangeForward,
        Distance_ChangeFoward,
        Frame_Destroy
    }

    public class ChangeSpeedData
    {
        public float Speed;
        public float Acceleration;
        public float EndSpeed;
    }
    public class ChangeForwardData
    {
        public Vector3 Forward;
        public MoveData.EHelixToward HelixToward;  //螺旋方向 0:不螺旋 1：右边 -1：左边
        public int HelixRefretFrame;      //螺旋翻转时间
        public float EulurPerFrame;       //螺旋每帧改变角度 < 0 左   > 0 向右
    }

    public EventType Type;
    public int FrameCount;
    public float Distance;
    public ChangeSpeedData SpeedData;
    public ChangeForwardData ForwardData;

    public static EventData NewFrame_Destroy(int frame)
    {
        var e = new EventData();
        e.Type = EventType.Frame_Destroy;
        e.FrameCount = frame;
        return e;
    }

    public static EventData NewDistance_ChangeForward(float distance, Vector3 forward, MoveData.EHelixToward helixToward = MoveData.EHelixToward.None, float eulurPerFrame = 0, int helixRefretFrame = 0)
    {
        var e = new EventData();
        e.Type = EventType.Distance_ChangeFoward;
        e.Distance = distance;
        e.ForwardData = new ChangeForwardData
        {
            Forward = forward,
            HelixToward = helixToward,
            EulurPerFrame = eulurPerFrame,
            HelixRefretFrame = helixRefretFrame
        };
        return e;
    }

    public static EventData NewFrame_ChangeForward(int frameCount, Vector3 forward, MoveData.EHelixToward helixToward = MoveData.EHelixToward.None, float eulurPerFrame = 0, int helixRefretFrame = 0)
    {
        var e = new EventData();
        e.Type = EventType.Frame_ChangeForward;
        e.FrameCount = frameCount;
        e.ForwardData = new ChangeForwardData
        {
            Forward = forward,
            HelixToward = helixToward,
            EulurPerFrame = eulurPerFrame,
            HelixRefretFrame = helixRefretFrame
        };
        return e;
    }
    public static EventData NewFrame_ChangeSpeed(int frameCount, float speed, float acceleration = 0, float endSpeed = 0)
    {
        var e = new EventData();
        e.Type = EventType.Frame_ChangeSpeed;
        e.FrameCount = frameCount;
        e.SpeedData = new ChangeSpeedData
        {
            Speed = speed,
            Acceleration = acceleration,
            EndSpeed = endSpeed
        };
        return e;
    }

}
