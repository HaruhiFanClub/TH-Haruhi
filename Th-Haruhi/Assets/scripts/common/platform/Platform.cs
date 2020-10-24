public enum EPlatform
{
	UNKNOWN,
	UNITY_IOS,
	UNITY_ANDROID,
	UNITY_STANDALONE,
	UNITY_WEBGL
}

public static class Platform
{
	public static string DeviceId { get; private set; }
	public static EPlatform Plat;
	public static bool InEditor;

	static Platform()
	{
		DeviceId = ApplicationUtility.GetDeviceId();
#if UNITY_EDITOR
		InEditor = true;
#endif

#if UNITY_IOS
	Plat = EPlatform.UNITY_IOS;
#elif UNITY_ANDROID
	Plat = EPlatform.UNITY_ANDROID;
#elif UNITY_WEBGL
	Plat = EPlatform.UNITY_WEBGL;
#elif UNITY_STANDALONE
	Plat = EPlatform.UNITY_STANDALONE;
#else
	Plat = EPlatform.UNKNOWN;
#endif
	}

}