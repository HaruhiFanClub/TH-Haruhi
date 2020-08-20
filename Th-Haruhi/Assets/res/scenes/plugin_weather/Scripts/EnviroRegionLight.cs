////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroSky- Renders a SkyDome with sun,moon,clouds and weather.          ////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;

public class EnviroRegionLight : MonoBehaviour
{
    [Header("Direct Light")]
    public float SunIntensityFac = 1f;
    public float MoonIntensityFac = 1f;

    public Gradient DirectLightColor;
    public Gradient SunDiskColor;

    [Header("Ambient Light")]
    public Gradient EquatorColor;
    public Gradient AmbientLightColor;
    public Gradient GroundColor;

    public void Active()
    {
        if (EnviroMgr.instance && EnviroMgr.instance.Envirosky)
        {
            var sky = EnviroMgr.instance.Envirosky;
            sky.ChangeAmbientLights(SunIntensityFac, MoonIntensityFac, DirectLightColor, SunDiskColor, EquatorColor,
                AmbientLightColor, GroundColor);
        }
    }

    public void DisActive()
    {
        if (EnviroMgr.instance && EnviroMgr.instance.Envirosky)
        {
            var sky = EnviroMgr.instance.Envirosky;
            sky.RevertAmbientLights();
        }
    }
}
