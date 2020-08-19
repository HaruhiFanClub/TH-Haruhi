

using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public static class SystemInfoUtils 
{
    public enum IosType
    {
        Unknow = 0,
        Phone = 1,
        Pad = 2,
    }

    public static bool GetDefaultShadow()
    {
#if UNITY_IOS
        int version;
        var iosType = GetIosTypeAndVersion(out version);
        if (iosType == IosType.Phone)
        {
            return true;
        }
        if (iosType == IosType.Pad)
        {
            if (version >= 5) return true;
        }
        return false;
#endif

#if UNITY_ANDROID
        
        var gpuLevel = GetAndroidGpuLevel();
        if (gpuLevel == EGpuLevel.VeryLow || gpuLevel == EGpuLevel.Low)
            return false;
        return true;
#endif

        return  true;
    }

    public static GameSetting.EnumResolution GetDefaultResolution()
    {
#if UNITY_IOS
        int version;
        var iosType = GetIosTypeAndVersion(out version);
        Debug.Log("GetDefaultResolution, iosType:" + iosType + " version:" + version + "  devName:" + SystemInfo.deviceModel);
        if (iosType == IosType.Phone)
        {
            return GameSetting.EnumResolution.High;
        }
        if (iosType == IosType.Pad)
        {
            if (version >= 5) return GameSetting.EnumResolution.High;
        }
#endif

#if UNITY_ANDROID
        var gpuLevel = GetAndroidGpuLevel();
        if (gpuLevel == EGpuLevel.VeryLow || gpuLevel == EGpuLevel.Low)
            return GameSetting.EnumResolution.Low;
        return GameSetting.EnumResolution.High;
#endif
#if UNITY_EDITOR
        return GameSetting.EnumResolution.High;
#else
        return GameSetting.EnumResolution.Mid;
#endif
    }

    public static GameSetting.EQuality GetDefaultQuailty()
    {
#if UNITY_IOS
int version;
        var iosType = GetIosTypeAndVersion(out version);
        if(version > 0)
        {
            if (iosType == IosType.Phone)
            {
                if (version <= 6)
                    return GameSetting.EQuality.Low;
                if (version <= 9)
                    return GameSetting.EQuality.Mid;
                return GameSetting.EQuality.High;
            }
            if (iosType == IosType.Pad)
            {
                if (version <= 3)
                    return GameSetting.EQuality.Low;
                if (version <= 4)
                    return GameSetting.EQuality.Mid;
                return GameSetting.EQuality.High;
            }
        }
#endif


#if UNITY_ANDROID
        var gpuLevel = GetAndroidGpuLevel();
        if(gpuLevel == EGpuLevel.VeryHigh)
             return GameSetting.EQuality.High;
        if (gpuLevel == EGpuLevel.High)
            return GameSetting.EQuality.Mid;
        return GameSetting.EQuality.Low;
#endif
        return GameSetting.EQuality.High;
    }

    //只有iphone7及之后
    public static bool CanShake()
    {
#if UNITY_IOS
        int version;
        var iosType = GetIosTypeAndVersion(out version);
        if (iosType == IosType.Phone)
	    {
	        if (version >= 9) return true; 
	    }
#endif
        return false;
    }

    /// <summary>
    /// 可否进入游戏
    /// </summary>
    /// <returns></returns>
    public static bool CanEnterGame()
    {
#if UNITY_ANDROID
       // var gpuLevel = GetAndroidGpuLevel();
       // if (gpuLevel == EGpuLevel.VeryLow) return false;
#endif


        return true;
    }

	public static string GetSysInfoData()
	{
		var info = new Dictionary<string, int>
		{
			{"graphicsMemorySize", SystemInfo.graphicsMemorySize},
			{"supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount},
			{"graphicsShaderLevel", SystemInfo.graphicsShaderLevel},
			{"supportsInstancing", SystemInfo.supportsInstancing ? 1 : 0},
			{"supportsComputeShaders", SystemInfo.supportsComputeShaders ? 1 : 0},
			{"processorCount", SystemInfo.processorCount},
			{"graphicsDeviceType", (int) SystemInfo.graphicsDeviceType},
			{"graphicsMultiThreaded", SystemInfo.graphicsMultiThreaded ? 1 : 0}
		};
		return JsonMapper.ToJson(info);
	}


    public static IosType GetIosTypeAndVersion(out int version)
    {
        version = 0;
#if UNITY_IOS
        string generation = SystemInfo.deviceModel;

        if (generation.Substring(0, 6) == "iPhone")
        {
            var s = generation.Replace("iPhone", "");
            var arr = s.Split(',');
            if (arr.Length == 2)
            {
                version = int.Parse(arr[0]);
                return IosType.Phone;
            }
            else
            {
                Debug.LogError("Get iPhone IosType Error:" + generation+" s:"+s);
            }
        }

        if (generation.Substring(0, 4) == "iPad")
        {
            var s = generation.Replace("iPad", "");
            var arr = s.Split(',');
            if (arr.Length == 2)
            {
                version = int.Parse(arr[0]);
                return IosType.Pad;
            }
            else
            {
                Debug.LogError("Get iPad IosType Error:" + generation + " s:" + s);
            }
        }
       
#endif

        return IosType.Unknow;
    }
    
    public static EGpuLevel GetAndroidGpuLevel()
    {
        try
        { 
            //高通类型Gpu
            var gpuName = SystemInfo.graphicsDeviceName;
            if (gpuName.Contains("Adreno (TM)"))
            {
                int fullVer = int.Parse(gpuName.Replace("Adreno (TM)", "").Replace(" ", ""));
                int sonVer = fullVer % 100;

                //6系列以后
                if (fullVer >= 800)
                {
                    return EGpuLevel.VeryHigh;
                }
                if (fullVer >= 600)
                {
                    return EGpuLevel.High;
                }
                //5系列
                if (fullVer >= 500)
                {
                    if (sonVer >= 30)
                        return EGpuLevel.Mid;
                    if (sonVer >= 9)
                        return EGpuLevel.Mid;
                    return EGpuLevel.Low;
                }

                //4系列
                if (fullVer >= 400)
                {
                    if (sonVer >= 30)
                        return EGpuLevel.Low;   //MID?
                }

                //3系列
                if (fullVer >= 300)
                {
                    if (sonVer >= 30)
                        return EGpuLevel.Low;  //MID?
                        
                }
            }

            //MaliGpu判断

            if (gpuName.Contains("Mali-G"))
            {
                int ver = int.Parse(gpuName.Replace("Mali-G", "").Replace(" ", ""));
                if (ver > 70)
                    return EGpuLevel.High;
                if (ver > 50)
                    return EGpuLevel.Low;
            }
            else
            {
                if (gpuName.Contains("Mali-T"))
                {
                    int ver = int.Parse(gpuName.Replace("Mali-T", "").Replace(" ", ""));
                    if (ver >= 800)
                    {
                        return EGpuLevel.Low;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
       
        return EGpuLevel.VeryLow;
    }

    public enum EGpuLevel
    {
        VeryLow = 1,    //超低配、超低分辨率、不显示阴影 
        Low = 2,        //低配、低分辨率、不显示阴影
        Mid = 3,        //低配、高分辨率、显示阴影
        High = 4,       //中配、高分辨率、显示阴影
        VeryHigh = 5,   //高配、高分辨率、显示阴影
    }
}
