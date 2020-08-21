

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public static class ApplicationUtility
{
    public static string GetDeviceId()
    {
        string device_id = "unknown device";

#if UNITY_IOS
        device_id = "idfa_" + Device.advertisingIdentifier;
        if (device_id.IndexOf("00000000-0000-0000-0000-000000000000") != -1)
        {
            device_id = "devId_" + SystemInfo.deviceUniqueIdentifier;
        }
#elif  UNITY_ANDROID
        var untiyAndroidUtility = new AndroidJavaClass("com.funova.common.UnityAndroidUtility");
        string deviceId = untiyAndroidUtility.CallStatic<string>("GetDeviceId");
        device_id = deviceId;
		untiyAndroidUtility.Dispose();
#else
        device_id = SystemInfo.deviceUniqueIdentifier;
#endif
        if(device_id.TrimEnd(new char[]{'0'}) == "imei_")
            device_id = SystemInfo.deviceUniqueIdentifier;
        if(device_id.TrimEnd(new char[]{'0', ':'}) == "mac_")
            device_id = SystemInfo.deviceUniqueIdentifier;    
        return device_id;
    }

    public static SystemLanguage GetSysLanguage()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
            var untiyAndroidUtility = new AndroidJavaClass("com.funova.common.UnityAndroidUtility");
            string languageStr = untiyAndroidUtility.CallStatic<string>("GetSysLanguage");
			untiyAndroidUtility.Dispose();
            SystemLanguage code;
            Debug.Log("languageStr=" + languageStr);
            languageStr = languageStr.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)[0];
            if (languageStr.StartsWith("zh-HK") || languageStr.StartsWith("zh-TW"))
            {
                code = SystemLanguage.ChineseTraditional;
            }
            else if (languageStr.StartsWith("zh-"))
            {
                code = SystemLanguage.ChineseSimplified;
            }
            else if (languageStr.StartsWith("th-"))
            {
                code = SystemLanguage.Thai;
            }
            else if (languageStr.StartsWith("in-"))
            {
                code = SystemLanguage.Indonesian;
            }
            else
            {
                code = SystemLanguage.English;
            }
#else
        SystemLanguage code = Application.systemLanguage;
#endif
        return code;
    }

    public static int CalcAscii(string str)
    {
        int ret = 0;
        byte[] asciiBytes = Encoding.ASCII.GetBytes(str);
        for (int i = 0; i < asciiBytes.Length; i++)
        {
            ret += asciiBytes[1];
        }
        return ret;
    }

    public static string GetPlatform()
    {
        var platform = "";
#if !UNITY_EDITOR && UNITY_IOS
		platform = "ios";
#elif !UNITY_EDITOR && UNITY_ANDROID
        platform = "android";
#else
        platform = "windows";
#endif
        return platform;
    }
}