
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System;

public static class ApplictionManager
{
	private static string AppVersion = System.String.Empty;
	public static string Version = System.String.Empty;			
	public static LitJson.JsonData gameConfig = null;

#if UNITY_ANDROID


    public static void GetInstallApplists()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        AndroidJavaClass Player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject Activity = Player.GetStatic<AndroidJavaObject>("currentActivity");
       
        AndroidJavaObject PackageManager = Activity.Call<AndroidJavaObject>("getPackageManager");
        //int GET_SIGNATURES = PackageManager.GetStatic<int>("GET_SIGNATURES");
        //int ntemp = PackageManager.GetStatic<int>("GET_UNINSTALLED_PACKAGES");
        int GET_UNINSTALLED_PACKAGES = 8192; // Constant Value: 8192 (0x00002000) GET_UNINSTALLED_PACKAGES
        AndroidJavaObject appInfoLists = PackageManager.Call<AndroidJavaObject>("getInstalledApplications", GET_UNINSTALLED_PACKAGES);

        int nSize = appInfoLists.Call<int>("size");
        dict["device_id"] = ApplicationUtility.GetDeviceId();
        int nIndex = 1;
        for (int i = 0; i < nSize;i++)
        {
            AndroidJavaObject appinfo = appInfoLists.Call<AndroidJavaObject>("get", i);
            string key = "appk" + nIndex;
            dict[key] = appinfo.Get<string>("className");
            if (dict[key] == null)
            {
                continue;
            }
            nIndex++;
            if (i % 15 == 0 && i > 0)
            {
                dict.Clear();
                dict["device_id"] = ApplicationUtility.GetDeviceId();
            }
        }
    }
#endif
 }
