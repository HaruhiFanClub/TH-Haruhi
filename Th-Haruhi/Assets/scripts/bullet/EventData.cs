﻿using System;
using UnityEngine;


public class EventData 
{
    public enum EventType
    {
        Frame_ChangeSpeed,
        Frame_ChangeForward,
        Frame_Destroy,
        Frame_Update,
        Frame_AimToPlayer,
    }

    public class ChangeSpeedData
    {
        public float Speed;
        public float Acceleration;
        public float EndSpeed;
    }
    public class ChangeForwardData
    {
        public Vector3? Forward;
        public MoveData.EHelixToward HelixToward;  //螺旋方向 0:不螺旋 1：右边 -1：左边
        public int HelixRefretFrame;      //螺旋翻转时间
        public float EulurPerFrame;       //螺旋每帧改变角度 < 0 左   > 0 向右
    }

    public class AimPlayerData
    {
        public float Speed;
        public float Angle;
    }

    public EventType Type;
    public int FrameCount;
    public int UpdateInterval;
    public int UpdateTimes;
    public float Distance;
    public ChangeSpeedData SpeedData;
    public ChangeForwardData ForwardData;
    public AimPlayerData AimToPlayerData;
    public Action<Bullet> OnUpdate;
    
    public static EventData NewFrame_Destroy(int frame)
    {
        var e = new EventData();
        e.Type = EventType.Frame_Destroy;
        e.FrameCount = frame;
        return e;
    }

    public static EventData Frame_AimToPlayer(int frame, float speed, float angleMin, float angleMax)
    {
        var e = new EventData();
        e.Type = EventType.Frame_AimToPlayer;
        e.FrameCount = frame;
        e.AimToPlayerData = new AimPlayerData
        {
            Speed = speed,
            Angle = UnityEngine.Random.Range(angleMin, angleMax),
        };
        return e;
    }

    public static EventData NewFrame_Update(int startFrame, int updateInterval, Action<Bullet> onUpdate = null, int updateTimes = -1)
    {
        var e = new EventData();
        e.Type = EventType.Frame_Update;
        e.FrameCount = startFrame;
        e.UpdateInterval = updateInterval;
        e.OnUpdate = onUpdate;
        e.UpdateTimes = updateTimes;
        return e;
    }

    public static EventData NewFrame_ChangeForward(int frameCount, Vector3? forward = null, MoveData.EHelixToward helixToward = MoveData.EHelixToward.None, float eulurPerFrame = 0, int helixRefretFrame = 0)
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
