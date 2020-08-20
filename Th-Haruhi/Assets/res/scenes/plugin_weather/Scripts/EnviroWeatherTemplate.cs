using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnviroWeatherTemplate {

	public string Name;
	public bool Spring = true;
	public float possibiltyInSpring = 100f;
	public bool Summer = true;
	public float possibiltyInSummer = 100f;
	public bool Autumn = true;
	public float possibiltyInAutumn = 100f;
	public bool winter = true;
	public float possibiltyInWinter = 100f;

	public List<ParticleSystem> effectParticleSystems = new List<ParticleSystem>();
	public List<float> effectEmmisionRates = new List<float>();
	public bool isLightningStorm;
	public EnviroWeatherCloudConfig cloudConfig;
	public float fogDistance;
	public float sunLightMod = 0.0f;
	public float WindStrenght = 0.5f;
	public float wetnessLevel = 0f;
	public float snowLevel = 0f;
	public AudioClip Sfx;
}

[System.Serializable]
public class EnviroWeatherCloudConfig {
	
	public Color BaseColor = Color.white;
	public Color ShadingColor = Color.gray;
	public float Coverage  = 1.0f;
	[Range(0.0001f,0.1f)]
	public float Sharpness = 0.001f;
	public Vector2 windDir;
}


