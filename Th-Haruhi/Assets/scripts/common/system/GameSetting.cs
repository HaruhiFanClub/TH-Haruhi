using System;
using System.Collections.Generic;
using UnityEngine;


public class GameSetting
{
    //memory setting 
    public static bool CacheSound = true;
    public static bool CacheAtlas = true;

    private static GameSettingData _settingData;
    public static void InitSetting()
    {
        _settingData = SystemDataMgr.Data.SettingData;


        RefreshGameResolutions();
        Application.targetFrameRate = 60;
    }


    public static float MusicVolume
    {
        set
        {
            if(_settingData.MusicVolume != value)
            {
                _settingData.MusicVolume = Math.Max(0f, value);
                _settingData.Save();
                Sound.GlobalMusicVolume = _settingData.MusicVolume;
            }
        }
        get { return _settingData.MusicVolume; }
    }

    public static float AudioVolume
    {
        set
        {
            if(_settingData.AudioVolume != value)
            {
                _settingData.AudioVolume = Math.Max(0f, value);
                _settingData.Save();
                Sound.GlobalAudioVolume = _settingData.AudioVolume;
            }
        }
        get { return _settingData.AudioVolume; }
    }

    public static void RefreshGameResolutions()
    {
        SetGameResolutions(GetDefaultResolution());
    }

    public static void SetGameResolutions(EResolution e)
    {
        var h = (int)e;
        if (h > Screen.currentResolution.height)
            h = Screen.currentResolution.height;
        var w = Mathf.CeilToInt(1.3333333f * h);
        Screen.SetResolution(w, h, false);
    }

    private static EResolution GetDefaultResolution()
    {
        var screenHeight = Screen.currentResolution.height;
        if (screenHeight > 1920)
            return EResolution.R1920;
        if (screenHeight > 1440)
            return EResolution.R1440;
        if (screenHeight > 960)
            return EResolution.R960;
        if (screenHeight > 720)
            return EResolution.R720;
        //if (screenHeight > 480)
        return EResolution.R480;
    }

    public enum EResolution
    {
        R1920 = 1920,
        R1440 = 1440,
        R960 = 960,
        R720 = 720,
        R480 = 480
    }
}

