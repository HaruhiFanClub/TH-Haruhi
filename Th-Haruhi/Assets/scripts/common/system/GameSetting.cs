using System;
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
        var h = Mathf.CeilToInt(Screen.currentResolution.height * 0.88888F);
        var w = Mathf.CeilToInt(1.333333f * h);
        Screen.SetResolution(w, h, false);
    }
}

