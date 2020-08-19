using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public static class Platform
{
	public static string DeviceId { get; private set; }
	public static string Channel { get; private set; }
	public static string Country { get; private set; }

	public static bool ForceOnline = false;  
	public static string AdChannel = "douyin-test";
	public static string AdCodeIdIOS = "945192461";
	public static string AdCodeId = "945161785";

	public static bool Lqsdk
	{
		get
		{
#if LQSDK
		return true;
#else
			return false;
#endif
		}
	}

	static Platform()
	{
		Debug.Log("LQSDK --> " + Lqsdk);
	    Channel = "";
		Init();
	}

	public static void Init()
	{
		DeviceId = ApplicationUtility.GetDeviceId();
		SetChannel("_FPSLocal");
	}

	public static void SetChannel(string channel)
	{
		Channel = channel;
	}

	public static void SetCountry(string country)
	{
		Country = country;
	}
}