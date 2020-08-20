/////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroMgr - Manage all Enviro Instances and Seasons.      	             	 ////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EnviroQualitySettings
{
	[Range(0,1)]
	public float GlobalParticleEmissionRates = 1f;
    public float UpdateInterval = 0.5f; //Attention: lower value = smoother growth and more frequent updates but more perfomance hungry!
}

[Serializable]
public class AudioVariables // AudioSetup variables
{
	public GameObject SFXHolderPrefab;
}


[AddComponentMenu("Enviro/Enviro Manager")]
public class EnviroMgr : MonoBehaviour
{
    private static EnviroMgr _instance;

    public static EnviroMgr instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<EnviroMgr>()); }
    }

	public GameObject Player;
	public Camera PlayerCamera;
    [HideInInspector]
	public EnviroSky Envirosky; // Use to access it from other scripts
    [HideInInspector]
    public EnviroWeather EnviroWeather; // Use to access it from other scripts


	public AudioVariables Audio = null;
    public EnviroQualitySettings Quality = null;

	[HideInInspector]
	public GameObject EffectsHolder;
	[HideInInspector]
	public AudioSource AudioSourceAmbient;
	[HideInInspector]
	public AudioSource AudioSourceThunder;

	// Used from other Enviro componets
	[HideInInspector]
	public float currentHour;
	[HideInInspector]
	public float currentDay;
	[HideInInspector]
	public float currentYear;
	[HideInInspector]
	public float currentTimeInHours;

    public delegate void WeatherChanged(EnviroWeatherPrefab weatherType);

	public event WeatherChanged OnWeatherChanged;

	void Awake ()
	{
	    _instance = this;
        if (Envirosky == null)
            Envirosky = GetComponent<EnviroSky>();
        if (EnviroWeather == null)
            EnviroWeather = GetComponent<EnviroWeather>();

        CreateEffects ();
	}

    public void RefreshShadowState()
    {
        Envirosky.RefreshShadowState();
    }

	public virtual void NotifyWeatherChanged(EnviroWeatherPrefab type)
	{
		if (OnWeatherChanged != null) OnWeatherChanged.Invoke(type);
		Envirosky.EnableCastShadow(type.ShadowStrengthFac);
	}

	void Start()
	{
		currentTimeInHours = GetInHours (Envirosky.GameTime.Hours, Envirosky.GameTime.Days, Envirosky.GameTime.Years);
		InvokeRepeating("UpdateEnviroment", 0, Quality.UpdateInterval);// Starts a repeating Method with custom intervall
	}

	void CreateEffects ()
	{
	    EffectsHolder = new GameObject {name = "WeatherEffect"};
	    EffectsHolder.transform.SetParent(Player.transform);
	    EffectsHolder.transform.localPosition = Player.transform.forward * 40f;
	    EffectsHolder.transform.localRotation = Quaternion.identity;
        var sfx = Instantiate (Audio.SFXHolderPrefab, Vector3.zero, Quaternion.identity);
		sfx.transform.SetParent(EffectsHolder.transform);
		EnviroAudioSource[] srcs = sfx.GetComponentsInChildren<EnviroAudioSource> ();
			
		for (int i = 0; i < srcs.Length; i++) 
		{
			switch (srcs [i].myFunction) {
			case EnviroAudioSource.AudioSourceFunction.Ambient:
				AudioSourceAmbient = srcs [i].audiosrc;
			    AudioSourceAmbient.volume = 0f;

                break;

			case EnviroAudioSource.AudioSourceFunction.Thunder:
				AudioSourceThunder = srcs [i].audiosrc;
			    AudioSourceThunder.volume = 0f;

                break;

			}
		}
	}
		
	// Check for correct Setup
	void OnEnable ()
	{
		if(Envirosky == null)
		{
			Debug.LogError("Please assign the EnviroSky component in Inspector!");
			this.enabled = false;
		}
	}

	public float GetInHours (float hours,float days, float years)
	{
		float inHours  = hours + (days*24f) + ((years * 365) * 24f);
		return inHours;
		
	}


	void UpdateEnviroment () // Update the all GrowthInstances
	{
		if (Envirosky != null)
		{
			currentHour = Envirosky.GameTime.Hours;
			currentDay = Envirosky.GameTime.Days;
			currentYear = Envirosky.GameTime.Years;
			currentTimeInHours = GetInHours (currentHour, currentDay, currentYear);
		}

	}

    public void ForceEnabledFog(float startDistance, float endDistance, float density)
    {
         Envirosky.InAirFog = true;
        if (EnviroWeather.currentActiveWeatherID)
        {
            EnviroWeather.currentActiveWeatherID.ForceEnableFog(startDistance, endDistance, density);
        }
    }

    public void RevertFog()
    {
        Envirosky.InAirFog = false;
        if (EnviroWeather.currentActiveWeatherID)
        {
            EnviroWeather.currentActiveWeatherID.RevertFog();
        }
    }
    void OnDestroy()
    {
        _instance = null;
    }
}
