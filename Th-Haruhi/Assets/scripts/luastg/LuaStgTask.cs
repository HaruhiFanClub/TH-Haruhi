
using System;
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

public class LuaStgTask
{
    public enum TaskType
    {
        Wander,
        PlaySound,
    }

    public class AutoTaskExecuse
    {
        public TaskType Type;
        public WanderData WanderData;
    }

    public class WanderData
    {
        public int nFrame;
        public Vector2 xRange;
        public Vector2 yRange;
        public Vector2 xAmplitude;
        public Vector2 yAmplitude;
        public MovementMode MovementMode;
        public DirectionMode DirectionMode;
    }

    public class SoundData
    {
        public EShootSound Name;
        public float Volume;
    }

    public class CreateBullet
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

    public class TaskParms
    {
        public string name;
        public float value;
        public float increment;
    }

    public enum TaskExecuseType
    {
        Sequence = 1,
        All = 2
    }

    public LuaStgTask(EntityBase master, int waitFrame, int interval, int repeat, LuaStgTask parent, TaskExecuseType sonExeType = TaskExecuseType.Sequence)
    {
        Master = master;
        WaitFrame = waitFrame;
        Interval = interval;
        Repeat = repeat;
        SonExeType = sonExeType;

        if (parent != null)
        {
            parent.SonTasks.Add(this); 
        }

        if (Repeat <= 0 && Interval <= 0)
        {
            Debug.LogError("Task 得Interval 和 Repeat 不能同时等于0!");
            Repeat = 1;
        }
    }

    private EntityBase Master;
    private int WaitFrame;
    private int Interval;
    private int Repeat;
    private TaskExecuseType SonExeType;
    private List<AutoTaskExecuse> AutoExecuse = new List<AutoTaskExecuse>();
    private List<TaskParms> Params = new List<TaskParms>();
    private List<LuaStgTask> SonTasks = new List<LuaStgTask>();
    public Action Execuse;


    //自動執行的行爲
    private void ChecAutoExecuse()
    {
        for(int i = 0; i < AutoExecuse.Count; i++)
        {
            var e = AutoExecuse[i];
            switch (e.Type)
            {
                case TaskType.Wander:
                    var data = e.WanderData;
                    Master.MoveToPlayer(data.nFrame, data.xRange, data.yRange, data.xAmplitude, data.yAmplitude, data.MovementMode, data.DirectionMode);
                    break;
                case TaskType.PlaySound:
                    break;
            }
        }
    }

    public void AddWander(int frame, float xMin, float xMax, float yMin, float yMax, float xAmpMin, float xAmpMax, float yAmpMin, float yAmpMax, MovementMode mMode, DirectionMode dMode)
    {
        var wanderData = new WanderData
        {
            nFrame = frame,
            xRange = Vector2Fight.NewLocal(xMin, xMax),
            yRange = Vector2Fight.NewLocal(yMin, yMax),
            xAmplitude = Vector2Fight.NewLocal(xAmpMin, xAmpMax),
            yAmplitude = Vector2Fight.NewLocal(yAmpMin, yAmpMax),
            MovementMode = mMode,
            DirectionMode = dMode
        };
        AutoExecuse.Add(new AutoTaskExecuse { Type = TaskType.Wander, WanderData = wanderData });
    }

    //設置參數
    public void SetP(string name, float value, float increment = 0)
    {
        Params.Add(new TaskParms
        {
            name = name,
            value = value,
            increment = increment
        });
    }

    //獲取參數
    public float GetP(string name)
    { 
        for(int i = 0; i < Params.Count; i++)
        {
            if (Params[i].name == name)
                return Params[i].value;
        }
        Debug.LogError("GetParamsValue Error, Cant Find :" + name);
        return 0f;
    }


    private int _alreadyRepeatCount;
    private int _execusedFrame;
    public void OnExecuse()
    {
        ChecAutoExecuse();
        Execuse?.Invoke();
        for (int i = 0; i < Params.Count; i++)
        {
            Params[i].value += Params[i].increment;
        }
    }

    public bool OnUpdate()
    {
        bool result = false;
        if (_execusedFrame >= WaitFrame)
        {
            //无间隔时间，一次执行完
            if(Interval <= 0)
            {
                for(int i = 0; i < Repeat; i++)
                {
                    _alreadyRepeatCount++;
                    OnExecuse();
                }
            }
            else
            {
                if(_execusedFrame % Interval == 0)
                {
                    _alreadyRepeatCount++;
                    OnExecuse();
                }
            }

            if (Repeat > 0 && _alreadyRepeatCount >= Repeat)
            {
                result = true;
            }
        }

        UpdateSonTasks();
        _execusedFrame++;
        return result;
    }

    private void UpdateSonTasks()
    {
        if (SonTasks.Count > 0)
        {
            //顺序执行
            if(SonExeType == TaskExecuseType.Sequence)
            {
                var sonTask = SonTasks[0];
                var bOver = sonTask.OnUpdate();
                if (bOver) 
                {
                    SonTasks.RemoveAt(0);
                }
            }
            //一起执行
            else
            {
                for (int i = SonTasks.Count - 1; i >= 0; i--)
                {
                    var bOver = SonTasks[i].OnUpdate();
                    if (bOver)
                    {
                        SonTasks.RemoveAt(i);
                    }
                }
            }
        }
    }
}
