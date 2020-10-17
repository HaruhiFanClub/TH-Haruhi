//////////////////////////////////////////////////////////////////////////
//
//   FileName : TimeScaleManager.cs
//     Author : hxm
// CreateTime : 2015-09-24
//       Desc :
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public static class TimeScaleManager
{
    private static float timeScaleBulletTime = 1;       //用于子弹时间
    private static float timeScaleForPause = 1;         //用于暂停
    private static float timeScaleForDebug = 1;         //用于debug

    public static void SetTimeScaleForPause(float value)
    {
        timeScaleForPause = value;
        CalcTimeScale();
    }
    public static void SetTimeScaleForDebug(float value)
    {
        timeScaleForDebug = value;
        CalcTimeScale();
    }
    public static void SetTimeScaleForBulletTime(float value)
    {
        timeScaleBulletTime = value;
        CalcTimeScale();
    }

    private static void CalcTimeScale()
    {
        //Debug.Log("SetTimeScale:" + timeScaleBulletTime + "*" + timeScaleForRunCover + "*" + timeScaleForPause);
        Time.timeScale = timeScaleBulletTime  * timeScaleForPause * timeScaleForDebug;
        GameEventCenter.Send(GameEvent.TimeScaleChanged, Time.timeScale);
    }

    private static bool _inBulletTime;
    private static float _endBulletTime;
    private static float _bulletTimeStartScale;

    public static void SetBulletTime(float timeScale, float sec)
    {
        if (_inBulletTime)
        {
            Debug.LogError("子弹之间还未结束，不能重复激活");
            return;
        }
        _inBulletTime = true;
        _endBulletTime = Time.realtimeSinceStartup + sec;
        SetTimeScaleForBulletTime(timeScale);
    }

    public static void Update()
    {
        if (!_inBulletTime) return;

        if (Time.realtimeSinceStartup > _endBulletTime)
        {
            _inBulletTime = false;
            SetTimeScaleForBulletTime(1);
        }
    }

    public static float GetTimeScale()
    {
        return Time.timeScale;
    }

    public static void ResetTimeScale()
    {
        timeScaleBulletTime = timeScaleForPause = timeScaleForDebug =  1;
        Time.timeScale = 1;
    }
}
