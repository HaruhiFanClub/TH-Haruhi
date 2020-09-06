using System;
using UnityEngine;


public class GameSetting
{
    //memory setting 
    public static bool CacheSound = true;
    public static bool CacheAtlas = true;

    /// <summary>
    /// 分辨率
    /// </summary>
    public enum EnumResolution
    {
        Low = 0,
        Mid = 1,
        High = 2
    }

    public enum EQuality
    {
        Low = 0,
        Mid = 1,
        High = 2
    }

    private static GameSettingData _settingData;
    public static void InitSetting()
    {
        _settingData = SystemDataMgr.Data.SettingData;
       _defaultScreenHeight = Screen.height;

        //initDefault
        RefreshQualityState(Quality);

#if UNITY_IOS || UNITY_ANDROID
        RefreshGameResolutions(XResolution);
#endif
    }

    private static void RefreshQualityState(EQuality q)
    {
       // QualitySettings.masterTextureLimit = q >= EQuality.High ? 0 : 1;
       Application.targetFrameRate = 60;
    }

    public static EQuality Quality
    {
        get { return _settingData.DefaultQuality; }
        set
        {
            if(_settingData.DefaultQuality != value)
            {
                _settingData.DefaultQuality = value;
                RefreshQualityState(value);
                _settingData.Save();
            }
        }
    }

    public static EnumResolution XResolution
    {
        get { return _settingData.Resolution; }
        set
        {
            if(_settingData.Resolution != value)
            {
                _settingData.Resolution = value;
                RefreshGameResolutions(value);
                _settingData.Save();
            }
        }
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

    public static int GetXResolution(EnumResolution xResolution)
    {
        switch (xResolution)
        {
            case EnumResolution.Low:
                return 480;
            case EnumResolution.Mid:
                return 720;
            case EnumResolution.High:
                return 1080;
        }
        return 720;
    }

    public static bool QualityLimit(EQuality e)
    {
        return Quality >= e;
    }

    private static Resolution _orgResolution;
    private static int _defaultScreenHeight;
    public static void RefreshGameResolutions(EnumResolution xResolution)
    {
        var origin = GetXResolution(xResolution);
        int designWidth = origin;
        int designHeigh = (int)(designWidth / (Screen.currentResolution.width / (float)Screen.currentResolution.height));
     
        if (Screen.currentResolution.width == designWidth && Screen.currentResolution.height == designHeigh)
        {
            return;
        }

        if (_orgResolution.width == 0 && _orgResolution.height == 0)
        {
            _orgResolution = Screen.currentResolution;
        }

        Screen.SetResolution(designWidth, designHeigh, true);
        Debug.Log(string.Format("SetResolution: {0}x{1} dpi:{2}", designWidth, designHeigh, Screen.dpi) + "DefaultScreenHeight:" + _defaultScreenHeight);
        Debug.Log(string.Format("Screen: {0}x{1} dpi:{2}", Screen.width, Screen.height, Screen.dpi));
    }
}

