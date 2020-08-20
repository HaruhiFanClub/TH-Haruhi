using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnviroWeatherPrefab : MonoBehaviour {

	public string Name;
    [HideInInspector] public List<float> effectEmmisionRates = new List<float>();

    [Header("Fog Settings")]
    public bool useFog;
	public float linearStartDistance;
	public float linearEndDistance;
	public float expDensity;

	[Header("Weather Settings")]
	public List<ParticleSystem> effectParticleSystems = new List<ParticleSystem>();

	public float sunLightMod = 0.0f;
	[Range(0,1)]
	public float WindStrenght = 0.5f;
	[Range(0,1)]
	public float wetnessLevel = 0f;
	[Range(0,1)]
	public float snowLevel = 0f;
	public bool isLightningStorm;
    public float ShadowStrengthFac = 1f;

	[Header("Audio Settings")]
	public AudioClip DaySfx;
    public AudioClip NightSfx;


    private bool _defaultEnableFog;
    private float _defaultFogStart;
    private float _defaultFogEnd;
    private float _defaultExpDensity;

    void Awake()
    {
        _defaultEnableFog = useFog;
        _defaultFogStart = linearStartDistance;
        _defaultFogEnd = linearEndDistance;
        _defaultExpDensity = expDensity;
    }

    public void ForceEnableFog(float linearStart, float linearEnd, float desteny)
    {
        useFog = true;
        linearStartDistance = linearStart;
        linearEndDistance = linearEnd;
        expDensity = desteny;

    }

    public void RevertFog()
    {
        useFog = _defaultEnableFog;
        linearStartDistance = _defaultFogStart;
        linearEndDistance = _defaultFogEnd;
        expDensity = _defaultExpDensity;
    }
}

