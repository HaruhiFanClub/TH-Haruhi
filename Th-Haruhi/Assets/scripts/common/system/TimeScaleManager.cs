using UnityEngine;

public static class TimeScaleManager
{
    public static void SetTimeScale(float value)
    {
        Debug.Log("SetTimeScale:" + value);
        Time.timeScale = value;
    }

    private static bool _inBulletTime;
    private static float _endBulletTime;
    private static float _bulletTimeStartScale;

    public static void SetBulletTime(float timeScale, float sec)
    {
        if(_inBulletTime)
        {
            Debug.LogError("子弹之间还未结束，不能重复激活");
            return;
        }

        _bulletTimeStartScale = GetTimeScale();
        _inBulletTime = true;
        _endBulletTime = Time.realtimeSinceStartup + sec;
        SetTimeScale(timeScale);
    }

    public static void Update()
    {
        if (!_inBulletTime) return;

        if(Time.realtimeSinceStartup > _endBulletTime)
        {
            _inBulletTime = false;
            SetTimeScale(_bulletTimeStartScale);
            _bulletTimeStartScale = 1f;
        }        
    }

    public static float GetTimeScale()
    {
        return Time.timeScale;
    }

    public static void ResetTimeScale()
    {
        Time.timeScale = 1;
    }
}
