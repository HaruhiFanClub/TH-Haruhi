using UnityEngine;
public class RelayInterval
{
    private float _interValTime;
    private float _lastSendTime;
    public RelayInterval(float time)
    {
        SetInterval(time);
    }

    public void SetInterval(float time)
    {
        _interValTime = time;
    }

    public float GetInterval()
    {
        return _interValTime;
    }

    public void SetLastTime(float time)
    {
        _lastSendTime = time;
    }

    public bool NextTime()
    {
        float time = Time.time;
        if (time - _lastSendTime > _interValTime)
        {
            _lastSendTime = time;
            return true;
        }
        return false;
    }
}