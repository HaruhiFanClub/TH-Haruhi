using UnityEngine;
public class FrameInterval
{
    private int _interValFrame;
    private int _lastExeFrame;

    public FrameInterval(int frame)
    {
        SetInterval(frame);
    }

    public void SetInterval(int frame)
    {
        _interValFrame = frame;
    }

    public int GetInterval()
    {
        return _interValFrame;
    }

    public void SetLastTime(int frame)
    {
        _lastExeFrame = frame;
    }

    public bool Next()
    {
        int curFrame = GameSystem.FixedFrameCount;
        if (curFrame - _lastExeFrame > _interValFrame)
        {
            _lastExeFrame = curFrame;
            return true;
        }
        return false;
    }
}