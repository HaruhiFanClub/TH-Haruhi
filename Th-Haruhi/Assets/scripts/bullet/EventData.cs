using System;
using UnityEngine;


public class EventData 
{
    public enum EventType
    {
        TimeChangeSpeed,
    }

    public class ChangeSpeedData
    {
        public float Speed;
        public float Acceleration;
        public float EndSpeed;
    }


    public EventType Type;
    public float DelayTime;
    public ChangeSpeedData SpeedData;

    public static EventData NewDelay_ChangeSpeed(float delayTime, float speed, float acceleration = 0, float endSpeed = 0)
    {
        var e = new EventData();
        e.Type = EventType.TimeChangeSpeed;
        e.DelayTime = delayTime;
        e.SpeedData = new ChangeSpeedData
        {
            Speed = speed,
            Acceleration = acceleration,
            EndSpeed = endSpeed
        };
        return e;
    }

}
