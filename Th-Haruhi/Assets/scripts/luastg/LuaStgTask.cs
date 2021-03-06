﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementMode
{
    MOVE_NORMAL = 0,
    MOVE_ACCEL = 1,
    MOVE_DECEL = 2,
    MOVE_ACC_DEC = 3
}

public enum DirectionMode
{
    MOVE_TOWARDS_PLAYER = 0,
    MOVE_X_TOWARDS_PLAYER = 1,
    MOVE_Y_TOWARDS_PLAYER = 2,
    MOVE_RANDOM = 3
}

public class TaskParms
{
    public string name;
    public float value;
    public float increment;

    public delegate List<TaskParms> NewParamFunc();
  
    public static List<TaskParms> New(string name1, float value1, float increment1,
                                string name2 = null, float value2 = 0, float increment2 = 0,
                                string name3 = null, float value3 = 0, float increment3 = 0,
                                string name4 = null, float value4 = 0, float increment4 = 0,
                                string name5 = null, float value5 = 0, float increment5 = 0)
    {
        List<TaskParms> list = new List<TaskParms>();
        list.Add(new TaskParms { name = name1, value = value1, increment = increment1 });

        if (!string.IsNullOrEmpty(name2))
            list.Add(new TaskParms { name = name2, value = value2, increment = increment2 });

        if (!string.IsNullOrEmpty(name3))
            list.Add(new TaskParms { name = name3, value = value3, increment = increment3 });

        if (!string.IsNullOrEmpty(name4))
            list.Add(new TaskParms { name = name4, value = value4, increment = increment4 });

        if (!string.IsNullOrEmpty(name5))
            list.Add(new TaskParms { name = name5, value = value5, increment = increment5 });
        return list;
    }
}


public enum ActionType
{
    Wander,
    PlaySound,
    Wait,
    AddRepeat,
    MoveTo,
    Custom
}

public class TaskExecuse
{
    public ActionType Type;
    public TaskWander WanderData;
    public TaskRepeat RepeatData;
    public TaskMoveTo MoveToData;
    public TaskWait WaitData;
    public TaskSound SoundData;
    public Action CustomAction;
}

public class TaskWait
{
    public int Frame;
    public Action AfterAction;
}

public class TaskSound
{
    public string Name;
    public float Volume;
}

public class TaskBullet
{
    public int BulletId;
    public Vector2 Position;
    public float Velocity;
    public float Angle;
    public bool AimToPlayer;
    public float RotationVelocity;
    public bool StayOnCreate;
    public bool Destroyable;
    public float Time;
    public bool Rebound;
    public float Acceleration;
    public float MaxVelocity;
    public bool Shuttle;
}


public class TaskWander
{
    public int nFrame;
    public Vector2 xRange;
    public Vector2 yRange;
    public Vector2 xAmplitude;
    public Vector2 yAmplitude;
    public MovementMode MovementMode;
    public DirectionMode DirectionMode;
}

public class TaskMoveTo
{
    public int nFrame;
    public float X;
    public float Y;
    public MovementMode MovementMode;
    public bool SetRotation;
}


public class TaskRepeat
{
    public class RepeatParams
    {
        public TaskParms.NewParamFunc InitParams;
        public List<TaskParms> Params = new List<TaskParms>();
        public Dictionary<string, TaskParms> ParamsDic = new Dictionary<string, TaskParms>();

        public TaskRepeat Repeat;

        public RepeatParams(TaskRepeat repeat)
        {
            Repeat = repeat;
        }

        //獲取參數
        public float Get(string name)
        {
            if (Repeat.Root.Params.ParamsDic.TryGetValue(name, out var param))
            {
                return param.value;
            }
            Debug.LogError("GetParamsValue Error, Cant Find :" + name);
            return 0f;
        }

        public void UpdateParams()
        {
            //参数 
            for (int i = 0; i < Params.Count; i++)
            {
                Params[i].value += Params[i].increment;
            }
        }

        public void ResetParams()
        {
            Params.Clear();

            if (InitParams != null)
            {
                Params = InitParams();

                for (int i = 0; i < Params.Count; i++)
                {
                    Repeat.Root.Params.ParamsDic[Params[i].name] = Params[i];
                }
            }
        }
    }

    public int Times;
    public int Interval;
    public Action<RepeatParams> Execuse;
    public int ExecuseTimes;

    public List<TaskExecuse> AutoExecuse = new List<TaskExecuse>();
    public RepeatParams Params;

    public Dictionary<string, float> Variables = new Dictionary<string, float>();

    public TaskRepeat Root;

    private EntityBase Master;
    public TaskRepeat(EntityBase master)
    {
        Master = master;
        Params = new RepeatParams(this);
    }

    public float Get(string name)
    {
        return Params.Get(name);
    }

    public void SetVariable(string name, float value)
    {
        Variables[name] = value;
    }

    public float GetVariable(string name)
    {
        if(Variables.TryGetValue(name, out var v))
        {
            return v;
        }
        Debug.LogError("Get Variable Error, name:" + name);
        return 0;
    }

    public void AddWait(int frame, Action afterAction = null)
    {
        AutoExecuse.Add(TaskTools.NewWait(frame, afterAction));
    }

    public void AddCustom(Action customAction)
    {
        AutoExecuse.Add(TaskTools.NewCustom(customAction));
    }

    public TaskRepeat AddRepeat(int times, int interval, TaskParms.NewParamFunc paramsFunc = null, Action<RepeatParams> execuse = null)
    {
        var e = TaskTools.NewRepeat(Master, times, interval, paramsFunc, execuse);
        e.RepeatData.Root = Root;
        AutoExecuse.Add(e);
        return e.RepeatData;
    }

    public void AddWander(int frame, float xMin, float xMax, float yMin, float yMax, float xAmpMin, float xAmpMax, float yAmpMin, float yAmpMax, MovementMode mMode, DirectionMode dMode)
    {
        AutoExecuse.Add(TaskTools.NewWander(frame, xMin, xMax, yMin, yMax, xAmpMin, xAmpMax, yAmpMin, yAmpMax, mMode, dMode));
    }

    public void AddMoveTo(int frame, float x, float y, MovementMode moveType, bool setRotation = false)
    {
        AutoExecuse.Add(TaskTools.NewMoveTo(frame, x, y, moveType, setRotation));
    }

    public void AddSoundPlay(string name, float volume)
    {
        AutoExecuse.Add(TaskTools.NewSoundPlay(name, volume));
    }
}



public class TaskTools
{
    public static IEnumerator UpdateExecuse(EntityBase Master, TaskExecuse e)
    {
        switch (e.Type)
        {
            //wait
            case ActionType.Wait:

                yield return Yielders.WaitFrame(e.WaitData.Frame);
                e.WaitData.AfterAction?.Invoke();
                break;

            //repeat
            case ActionType.AddRepeat:

                yield return DoRepeat(Master, e.RepeatData);
                break;

            //wander
            case ActionType.Wander:

                var data = e.WanderData;
                yield return Master.MoveToPlayer(data.nFrame, data.xRange, data.yRange, data.xAmplitude, data.yAmplitude, data.MovementMode, data.DirectionMode);
                break;

            //moveto
            case ActionType.MoveTo:
                var md = e.MoveToData;
                yield return Master.MoveTo(new Vector3(md.X, md.Y), md.nFrame, md.MovementMode, md.SetRotation);
                break;

            //playsound noWait
            case ActionType.PlaySound:
                Sound.PlayTHSound(e.SoundData.Name, true, e.SoundData.Volume);
                break;

            //custome noWait
            case ActionType.Custom:
                e.CustomAction?.Invoke();
                break;
        }
    }

    private static IEnumerator DoRepeat(EntityBase Master, TaskRepeat data)
    {
        //防死循环处理
        if (data.Interval == 0 && data.Times == 0)
        {
            data.Interval = 1;
        }

        //一直循环处理
        var loopTimes = data.Times;
        if (data.Times == 0)
        {
            loopTimes = int.MaxValue;
        }

        data.Params.ResetParams();

        for (int i = 0; i < loopTimes; i++)
        {
            //执行子任务
            for (int j = 0; j < data.AutoExecuse.Count; j++)
            {
                var execuse = data.AutoExecuse[j];
                yield return UpdateExecuse(Master, execuse);
            }

            //执行自己
            data.Execuse?.Invoke(data.Params);
            data.Params.UpdateParams();
            data.ExecuseTimes++;

            //interval, 最后一次不等待
            if (data.Interval > 0)
            {
                yield return Yielders.WaitFrame(data.Interval);
            }
        }

        //remove
        if(data.Root.Times > 0 && data.Root.ExecuseTimes >= data.Root.Times)
            data.AutoExecuse.Clear();
    }


    public static TaskExecuse NewWait(int frame, Action afterAction = null)
    {
        return new TaskExecuse { Type = ActionType.Wait, WaitData = new TaskWait { Frame = frame, AfterAction = afterAction }};
    }

    public static TaskExecuse NewCustom(Action customAction)
    {
        return new TaskExecuse { Type = ActionType.Custom, CustomAction = customAction };
    }

    public static TaskExecuse NewRepeat(EntityBase master, int times, int interval, TaskParms.NewParamFunc paramsFunc = null, Action<TaskRepeat.RepeatParams> execuse = null)
    {
        var repeatData = new TaskRepeat(master)
        {
            Times = times,
            Interval = interval,
            Execuse = execuse,
        };
        repeatData.Params.InitParams = paramsFunc;
        return new TaskExecuse { Type = ActionType.AddRepeat, RepeatData = repeatData };
    }

    public static TaskExecuse NewWander(int frame, float xMin, float xMax, float yMin, float yMax, float xAmpMin, float xAmpMax, float yAmpMin, float yAmpMax, MovementMode mMode, DirectionMode dMode)
    {
        var wanderData = new TaskWander
        {
            nFrame = frame,
            xRange = new Vector3(xMin, xMax),
            yRange = new Vector3(yMin, yMax),
            xAmplitude = new Vector3(xAmpMin, xAmpMax),
            yAmplitude = new Vector3(yAmpMin, yAmpMax),
            MovementMode = mMode,
            DirectionMode = dMode
        };
        return new TaskExecuse { Type = ActionType.Wander, WanderData = wanderData };
    }

    public static TaskExecuse NewMoveTo(int frame, float x, float y, MovementMode moveType, bool setRotation)
    {
        var moveData = new TaskMoveTo
        {
            nFrame = frame,
            MovementMode = moveType,
            X = x,
            Y = y,
            SetRotation = setRotation
        };
        return new TaskExecuse { Type = ActionType.MoveTo, MoveToData = moveData };
    }

    public static TaskExecuse NewSoundPlay(string name, float volume)
    {
        var soundData = new TaskSound
        {
            Name = name,
            Volume = volume
        };
        return new TaskExecuse { Type = ActionType.PlaySound, SoundData = soundData };
    }
}


public class LuaStgTask : MonoBehaviour
{
    public string Name;
    public List<TaskExecuse> AutoExecuse = new List<TaskExecuse>();

    private EntityBase Master;
    private void Awake()
    {
        Master = GetComponent<EntityBase>();
        StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while (true)
        {
            if (AutoExecuse.Count > 0)
            {
                var execuse = AutoExecuse[0];
                AutoExecuse.RemoveAt(0);
                yield return TaskTools.UpdateExecuse(Master, execuse);
            }
            else
            {
                yield return Yielders.FixedFrame;
            }
        }
    }

    public void AddWait(int frame, Action afterAction = null)
    {
        AutoExecuse.Add(TaskTools.NewWait(frame, afterAction));
    }

    public void AddCustom(Action customAction)
    {
        AutoExecuse.Add(TaskTools.NewCustom(customAction));
    }

    public TaskRepeat AddRepeat(int times, int interval, TaskParms.NewParamFunc paramsFunc = null, Action<TaskRepeat.RepeatParams> execuse = null)
    {
        var e = TaskTools.NewRepeat(Master, times, interval, paramsFunc, execuse);
        e.RepeatData.Root = e.RepeatData;
        AutoExecuse.Add(e);
        return e.RepeatData;
    }

    public void AddWander(int frame, float xMin, float xMax, float yMin, float yMax, float xAmpMin, float xAmpMax, float yAmpMin, float yAmpMax, MovementMode mMode, DirectionMode dMode)
    {
        AutoExecuse.Add(TaskTools.NewWander(frame, xMin, xMax, yMin, yMax, xAmpMin, xAmpMax, yAmpMin, yAmpMax, mMode, dMode));
    }

    public void AddMoveTo(int frame, float x, float y, MovementMode moveType, bool setRotation = false)
    {
        AutoExecuse.Add(TaskTools.NewMoveTo(frame, x, y, moveType, setRotation));
    }
    public void AddSoundPlay(string name, float volume)
    {
        AutoExecuse.Add(TaskTools.NewSoundPlay(name, volume));
    }
}
