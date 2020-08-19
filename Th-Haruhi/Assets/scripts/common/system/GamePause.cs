//////////////////////////////////////////////////////////////////////////
//
//   FileName : GamePause.cs
//     Author : zhoumeng
// CreateTime : 2014-11-26
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

[Flags]
public enum EPauseFrom
{
    None = 0,
    Esc = 1,
}


public static class GamePause
{
    private static EPauseFrom _stateFrom = EPauseFrom.None;
    public static bool isPlayViedo = false;

    public static void SetPauseState(EPauseFrom state, bool b)
    {
        if (b)
        {
            _stateFrom |= state;
        }
        else
        {
            _stateFrom &= ~state;
        }
    }

    public static bool InPause
    {
        get { return _stateFrom != EPauseFrom.None; }
    }

    public static void Revert()
    {
        _stateFrom = EPauseFrom.None;
        TimeScaleManager.SetTimeScaleForPause(1);
    }

    public static void PauseGame(EPauseFrom from)
    {
        Debug.Log("PauseGame:" + from);
        SetPauseState(from, true);
        if (_stateFrom != EPauseFrom.None)
        {
            TimeScaleManager.SetTimeScaleForPause(0f);
        }
    }

    public static void DoContionueGame(EPauseFrom from)
    {
        Debug.Log("DoContionueGame:" + from + " " + _stateFrom);
        SetPauseState(from, false);

        if (_stateFrom == EPauseFrom.None)
        {
            TimeScaleManager.SetTimeScaleForPause(1);
        }
    }
}
