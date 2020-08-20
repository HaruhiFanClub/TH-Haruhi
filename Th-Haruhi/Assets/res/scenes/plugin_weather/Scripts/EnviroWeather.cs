////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////              EnviroWeather - Customizable Weather Engine                            ///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Enviro/Weather Manager")]
public class EnviroWeather : MonoBehaviour {

    [HideInInspector]
    public EnviroWeatherPrefab CurrWeather;
    public List<GameObject> WeatherEffectsPrefabs = new List<GameObject>();
	public EnviroWeatherPrefab currentActiveWeatherID;
	private EnviroWeatherPrefab lastActiveWeatherID;

	public WindZone windZone;
	public List<AudioClip> ThunderSFX = new List<AudioClip> ();
	public EnviroLightning LightningGenerator;
	[HideInInspector]
	public float wetness;
	private float curWetness;
	[HideInInspector]
 	public float SnowStrenght;
	private float curSnowStrenght;

	private int thundersfx;
    private GameObject VFX;
    void Start()
    {
        VFX = new GameObject();
        VFX.name = "VFX";
        VFX.transform.SetParent(EnviroMgr.instance.EffectsHolder.transform);
        VFX.transform.localPosition = Vector3.zero;
    }


    public void CreateCurrWeather(int weatherId)
    {
        if (weatherId >= WeatherEffectsPrefabs.Count)
        {
            Debug.LogError("初始化天气失败，weaterId错误："+weatherId);
            return;
        }
        GameObject templ = Instantiate(WeatherEffectsPrefabs[weatherId]);
        templ.transform.SetParent(VFX.transform);
        templ.transform.localPosition = Vector3.zero;
        templ.transform.localRotation = Quaternion.identity;
        CurrWeather = templ.GetComponent<EnviroWeatherPrefab>();


        for (int i2 = 0; i2 < CurrWeather.effectParticleSystems.Count; i2++)
        {
            CurrWeather.effectEmmisionRates.Add(GetEmissionRate(CurrWeather.effectParticleSystems[i2]));
            SetEmissionRate(CurrWeather.effectParticleSystems[i2], 0f);
        }
    }

	public static float GetEmissionRate (ParticleSystem system)
	{
		return system.emission.rateOverTime.constantMax;
	}

	public static void SetEmissionRate (ParticleSystem sys, float emissionRate)
	{
		var emission = sys.emission;
		var rate = emission.rateOverTime;
		rate.constantMax = emissionRate;
		emission.rateOverTime = rate;
	}

	void UpdateAudioSource (EnviroWeatherPrefab i)
	{
	    if (EnviroMgr.instance.Envirosky.IsNight)
	    {
	        if (i.NightSfx != null)
	        {
	            EnviroMgr.instance.AudioSourceAmbient.clip = i.NightSfx;
	            EnviroMgr.instance.AudioSourceAmbient.loop = true;
	            EnviroMgr.instance.AudioSourceAmbient.Play();
            }
	    }
	    else
	    {
	        if (i.DaySfx != null)
	        {
	            EnviroMgr.instance.AudioSourceAmbient.clip = i.DaySfx;
	            EnviroMgr.instance.AudioSourceAmbient.loop = true;
	            EnviroMgr.instance.AudioSourceAmbient.Play();
            }
	    }
	}


	void UpdateFog (EnviroWeatherPrefab i)
	{
	    RenderSettings.fog = i.useFog && EnviroMgr.instance.Envirosky.EnableFog;
	    RenderSettings.fogMode = EnviroMgr.instance.Envirosky.Fog.Fogmode;
	    if (EnviroMgr.instance.Envirosky.Fog.Fogmode == FogMode.Linear)
	    {
	        RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, i.linearStartDistance, 0.01f);
	        RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, i.linearEndDistance, 0.01f);
	    }
	    else
	        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, i.expDensity, 0.01f);
  
	}

	void UpdateEffectSystems (EnviroWeatherPrefab id)
	{
		for (int i = 0; i < id.effectParticleSystems.Count; i++) 
		{
			// Set EmisionRate
		    var emission = id.effectParticleSystems[i].emission;
		    float target = id.effectEmmisionRates[i] * EnviroMgr.instance.Quality.GlobalParticleEmissionRates;
		    if (emission.rateOverTimeMultiplier < target - 0.05f)
		    {
		        emission.rateOverTimeMultiplier = Mathf.Lerp(emission.rateOverTimeMultiplier, target, 0.1f);
		    }
		}

		for (int i = 0; i < lastActiveWeatherID.effectParticleSystems.Count; i++) 
		{
            // Set EmisionRates
		    var emission = lastActiveWeatherID.effectParticleSystems[i].emission;
		    if (emission.rateOverTimeMultiplier > 0.05f)
		    {
		        emission.rateOverTimeMultiplier = Mathf.Lerp(emission.rateOverTimeMultiplier, 0f,0.1f);
		    }
	    }

        windZone.windMain = id.WindStrenght; // Set Wind Strenght

		curWetness = wetness;
		wetness = Mathf.Lerp (curWetness, id.wetnessLevel, 0.1f * Time.deltaTime);
		wetness = Mathf.Clamp(wetness,0f,1f);

		curSnowStrenght = SnowStrenght;
		SnowStrenght = Mathf.Lerp (curSnowStrenght, id.snowLevel, 0.05f * Time.deltaTime);
		SnowStrenght = Mathf.Clamp(SnowStrenght,0f,1f);

	}

	IEnumerator PlayThunderRandom()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(10,20));
		int i = UnityEngine.Random.Range(0,ThunderSFX.Count);
		EnviroMgr.instance.AudioSourceThunder.clip = ThunderSFX[i];
		EnviroMgr.instance.AudioSourceThunder.loop = false;
		EnviroMgr.instance.AudioSourceThunder.Play ();
		LightningGenerator.Lightning ();
		thundersfx = 0;
	}

    /// <summary>
    /// 改变天气接口
    /// </summary>
    /// <param name="weatherId"></param>
	public void SetWeatherOverwrite (int weatherId)
	{
		if (CurrWeather != currentActiveWeatherID)
		{
		    currentActiveWeatherID = CurrWeather;
            lastActiveWeatherID = currentActiveWeatherID;
		    UpdateWeather(currentActiveWeatherID);
        }
    }

    
	void LateUpdate()
	{
	    if (currentActiveWeatherID == null || lastActiveWeatherID == null) return;

	    EnviroMgr.instance.Envirosky.Lighting.SunWeatherMod = currentActiveWeatherID.sunLightMod;
        UpdateFog (currentActiveWeatherID);
	
		//Play ThunderSFX
		if ( thundersfx == 0 && currentActiveWeatherID.isLightningStorm)
		{
			thundersfx = 1;
			StartCoroutine(PlayThunderRandom());
		}

		UpdateEffectSystems (currentActiveWeatherID);
	}
	
	void UpdateWeather (EnviroWeatherPrefab ID) 
	{
		EnviroMgr.instance.NotifyWeatherChanged (ID);
		UpdateAudioSource (ID);
	}
}
