////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////        EnviroSky- Renders a SkyDome with sun,moon,clouds and weather.          ////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using UnityEngine.Rendering;

[Serializable]
public class ObjectVariables // References - setup these in inspector! Or use the provided prefab.
{
	public GameObject Sun        = null;
	public GameObject Moon       = null;
	public GameObject Atmosphere = null;
    public GameObject Cloud = null;
}

[Serializable]
public class TimeVariables // GameTime variables
{
	[Header("Date and Time")]
	public bool ProgressTime = true;
    public float Hours  = 12f;  // 0 -  24 Hours day
    public float Days = 1f; //  1 - 365 Days of the year
	public float Years = 1f; // Count the Years maybe usefull!
	public float DayLengthInMinutes = 30; // DayLenght in realtime minutes
	[Range(0,24)]
	public float NightTimeInHours = 18f; 
	[Range(0,24)]
	public float MorningTimeInHours = 5f; 
	[Header("Location")]
    public float Latitude   = 0f;   // -90,  90   Horizontal earth lines
    public float Longitude  = 0f;   // -180, 180  Vertical earth line

}

[Serializable] 
public class LightVariables // All Lightning Variables
{
	[Header("Direct Light")]
    public float SunIntensity = 0.75f;
	public float MoonIntensity = 0.5f;
	public float MoonPhase = 0.0f;

	public Gradient DirectLightColor;
	public Gradient SunDiskColor;

    [Header("Ambient Light")]
    public Gradient EquatorColor;
	public Gradient AmbientLightColor;
    public Gradient GroundColor;
	public AnimationCurve ambientLightIntenisty;

	[Header("Stars")]
	public AnimationCurve starsIntensity;
	[HideInInspector]public float SunWeatherMod = 0.0f;
}


[Serializable]
public class FogSettings 
{
	public FogMode Fogmode;
	public Gradient baseFogColor;
}

[ExecuteInEditMode]
[AddComponentMenu("Enviro/Sky System")]
public class EnviroSky : MonoBehaviour
{
    private float _smooth = 3f;
    private float _sunSmooth = 10f;

    // Parameters
	public ObjectVariables Components = null;
	public TimeVariables GameTime  = null;
	public LightVariables  Lighting   = null;
	public FogSettings Fog = null;

	// Private Variables
	private int domeSeg = 32;
	public bool IsNight { private set; get; }

    //Some Pointers
    private Transform _domeTransform;
    private Renderer _atmosphereRenderer;
	private Material  _atmosphereShader;

    private Transform _sunTransform;
	private Renderer  _sunRenderer;
	private Material  _sunShader;
	private Light     _sunLight;

    private Transform _moonTransform;
	private Renderer  _moonRenderer;
	private Material  _moonShader;
	private Light     _moonLight;


    private float _defualtSunIntensity;
    private float _defaultMoonIntensity;

    private float _currSunIntensity;
    private float _currMoonIntensity;
    private Gradient _currDirectLightColor;
    private Gradient _currSunDiskColor;
    private Gradient _currEquatorColor;
    private Gradient _currAmbientLightColor;
    private Gradient _currGroundColor;
    private float _currSunFac = 1f;
    private float _currMoonFac = 1f;
    private Color _tempAmbientColor;
    public bool EnableFog = true;

	private float OrbitRadius
    {
        get { return _domeTransform.localScale.x; }
    }

    private float CurrSunIntensity
    {
        get { return _currSunIntensity * _currSunFac; }
    }

    private float CurrMoonIntersity
    {
        get { return _currMoonIntensity * _currMoonFac; }
    }

    private float _initHoursTime;
    private float _initHours;
    public void InitHours(float hours)
    {
        _initHoursTime = Time.realtimeSinceStartup;
        GameTime.Hours = hours;
        _initHours = hours;

        if (GameTime.Hours >= GameTime.NightTimeInHours || GameTime.Hours <= GameTime.MorningTimeInHours)
        {
            IsNight = true;
            
        }
        else
        {
            IsNight = false;
        }
        Components.Cloud.SetActiveSafe(!IsNight);
    }
    // PI
    const float Pi = Mathf.PI;

    void OnEnable()
    {
        _domeTransform = transform;

		// Setup Fog
		RenderSettings.fogMode = Fog.Fogmode;
		//Check for needed Objects and define startup vars 
		if (Components.Atmosphere)
		{
		    _atmosphereRenderer = Components.Atmosphere.GetComponent<Renderer>();
            _atmosphereShader = _atmosphereRenderer.sharedMaterial;

			MeshFilter filter = Components.Atmosphere.GetComponent<MeshFilter>();

            if (filter != null)
            {
                if (filter.sharedMesh != null)
                {
                    DestroyImmediate(filter.sharedMesh);
                }
				// Create the gradientDome mesh and assign it
                Mesh mesh = new Mesh();
				CreateDome(mesh, domeSeg);
                filter.sharedMesh = mesh;
            }
        }

		if (Components.Sun)
        {
			_sunTransform = Components.Sun.transform;
			_sunRenderer = Components.Sun.GetComponent<Renderer>();
            _sunShader = _sunRenderer.sharedMaterial;
			_sunLight = Components.Sun.GetComponent<Light>();
          //  _sunLight.cullingMask = LayersMask.DirectionalLightCullingMask;
        }
        else
        {
            Debug.LogError("Please set Sun object in inspector!");
            this.enabled = false;
        }

        if (Components.Moon)
        {
			_moonTransform = Components.Moon.transform;
			_moonRenderer = Components.Moon.GetComponent<Renderer>();
            _moonShader = _moonRenderer.sharedMaterial;
			_moonLight = Components.Moon.GetComponent<Light>();
           // _moonLight.cullingMask = LayersMask.DirectionalLightCullingMask;
        }
        else
        {
			Debug.LogError("Please set Moon object in inspector!");
            this.enabled = false;
        }

        SaveDefaultLightValue();
        //EnableLight(XGameSetting.EnableShadow);
    }

    private void SaveDefaultLightValue()
    {
        _defualtSunIntensity = Lighting.SunIntensity;
        _defaultMoonIntensity = Lighting.MoonIntensity;

        RevertAmbientLights();
    }


    private void UpdateAmbientLight ()
	{
	    Lighting.SunIntensity = Mathf.Lerp(Lighting.SunIntensity, CurrSunIntensity, Time.deltaTime * _sunSmooth);
	    Lighting.MoonIntensity = Mathf.Lerp(Lighting.MoonIntensity, CurrMoonIntersity, Time.deltaTime * _sunSmooth);

	    _tempAmbientColor = _currAmbientLightColor.Evaluate(GameTime.Hours / 24f);

	    if (_fadingEnterRoom)
	    {
	        if (Lighting.SunIntensity - CurrSunIntensity < 0.001f)
	        {
	            _currSunIntensity = _defualtSunIntensity;
	            _fadingEnterRoom = false;
	        }
	    }
	    if (_fadingOutRoom)
	    {
            if (CurrSunIntensity - Lighting.SunIntensity < 0.001f)
	        {
	            _currSunIntensity = _defualtSunIntensity;
                _fadingOutRoom = false;
            }
	    }

        RenderSettings.ambientIntensity = Lighting.ambientLightIntenisty.Evaluate (GameTime.Hours / 24f);
        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, _tempAmbientColor, Time.deltaTime);
	    var fogColor = InAirFog ? Fog.baseFogColor.Evaluate(GameTime.Hours / 24f) : Color.white;
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColor, Time.deltaTime * _smooth);
        RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, _currEquatorColor.Evaluate(GameTime.Hours / 24f), Time.deltaTime * _smooth);
        RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, _currGroundColor.Evaluate(GameTime.Hours / 24f), Time.deltaTime * _smooth);
    }

    public bool InAirFog;
	
    void LateUpdate()
    {
        if (_banLight) return;

        UpdateTime();
		ValidateParameters();
		UpdateAmbientLight ();

        // Calculates the Solar latitude
        float latitudeRadians = Mathf.Deg2Rad * GameTime.Latitude;
        float latitudeRadiansSin = Mathf.Sin(latitudeRadians);
        float latitudeRadiansCos = Mathf.Cos(latitudeRadians);

		// Calculates the Solar longitude
		float longitudeRadians = Mathf.Deg2Rad * GameTime.Longitude;

        // Solar declination - constant for the whole globe at any given day
		float solarDeclination = 0.4093f * Mathf.Sin(2f * Pi / 368f * (GameTime.Days - 81f));
        float solarDeclinationSin = Mathf.Sin(solarDeclination);
        float solarDeclinationCos = Mathf.Cos(solarDeclination);

        // Calculate Solar time
		float timeZone = (int)(GameTime.Longitude / 15f);
        float meridian = Mathf.Deg2Rad * 15f * timeZone;
		float solarTime = GameTime.Hours + 0.170f * Mathf.Sin(4f * Pi / 373f * (GameTime.Days - 80f)) - 0.129f * Mathf.Sin(2f * Pi / 355f * (GameTime.Days - 8f))  + 12f / Pi * (meridian - longitudeRadians);
        float solarTimeRadians = Pi / 12f * solarTime;
        float solarTimeSin = Mathf.Sin(solarTimeRadians);
        float solarTimeCos = Mathf.Cos(solarTimeRadians);

        // Solar altitude angle between the sun and the horizon
        float solarAltitudeSin = latitudeRadiansSin * solarDeclinationSin - latitudeRadiansCos * solarDeclinationCos * solarTimeCos;
        float solarAltitude = Mathf.Asin(solarAltitudeSin);

        // Solar azimuth angle of the sun around the horizon
        float solarAzimuthY = -solarDeclinationCos * solarTimeSin;
        float solarAzimuthX = latitudeRadiansCos * solarDeclinationSin - latitudeRadiansSin * solarDeclinationCos * solarTimeCos;
        float solarAzimuth = Mathf.Atan2(solarAzimuthY, solarAzimuthX);

        // Convert to spherical coords
        float coord = Pi / 2 - solarAltitude;
        float phi = solarAzimuth;

        // Update sun position
        _sunTransform.position = _domeTransform.position + _domeTransform.rotation * SphericalToCartesian(OrbitRadius, coord, phi);
        _sunTransform.LookAt(_domeTransform.position);

        // Update moon position
        _moonTransform.position = _domeTransform.position + _domeTransform.rotation * SphericalToCartesian(OrbitRadius, coord + Pi, phi);
        _moonTransform.LookAt(_domeTransform.position);

        // Update sun and fog color according to the new position of the sun
        SetupSunAndMoonColor(coord);
        SetupShader();

        if (!IsNight && (GameTime.Hours >= GameTime.NightTimeInHours || GameTime.Hours <= GameTime.MorningTimeInHours))
		{
			IsNight = true;

		   

        }
		else if (IsNight && (GameTime.Hours <= GameTime.NightTimeInHours && GameTime.Hours >= GameTime.MorningTimeInHours))
        {
			IsNight = false;
        }

       
    }

	// Setup the Shaders with correct information
    private Color _sunDiskColor;
    private void SetupShader()
    {

        Color sunHaloColor = _sunLight.color;
		sunHaloColor.a *= _sunLight.intensity / (Lighting.SunIntensity + Lighting.SunWeatherMod);

        if (_atmosphereShader != null)
        {
            _atmosphereShader.SetColor("_SunColor", _sunLight.color);
            _atmosphereShader.SetVector("_SunDirection", _sunTransform.forward);
			_atmosphereShader.SetMatrix ("_Rotation", _sunTransform.worldToLocalMatrix);
        }

        if (_moonShader != null)
        {
			_moonShader.SetFloat("_Phase", Lighting.MoonPhase);
        }

        if (_sunShader != null)
        {
            _sunDiskColor = Color.Lerp(_sunDiskColor, _currSunDiskColor.Evaluate(GameTime.Hours / 24f),
                Time.deltaTime * _smooth);

            _sunShader.SetColor("_Color", _sunDiskColor);
        }
    }

	// Update the GameTime
    private void UpdateTime()
    {
        var sceonds = Time.realtimeSinceStartup - _initHoursTime;
        var realHourAfter = sceonds / 3600f * (1440 / GameTime.DayLengthInMinutes);

        
		float oneDay = GameTime.DayLengthInMinutes * 60f;
        float moonTime = Time.deltaTime / (30f * oneDay) * 2f;

        if (GameTime.ProgressTime) // Calculate Time
        {
            GameTime.Hours = (_initHours + realHourAfter) % 24;

            Lighting.MoonPhase += moonTime;

			if (Lighting.MoonPhase < -1) Lighting.MoonPhase += 2;
			else if (Lighting.MoonPhase > 1) Lighting.MoonPhase -= 2;

        }
    }

    // Calculate sun and moon color
    private void SetupSunAndMoonColor(float setup)
    {
        var finalColor = _currDirectLightColor.Evaluate(GameTime.Hours / 24f);
        _sunLight.color = Color.Lerp(_sunLight.color, finalColor, Time.deltaTime * _smooth);
        _moonLight.color = Color.Lerp(_moonLight.color, finalColor, Time.deltaTime * _smooth);


        // Sun altitude and intensity dropoff angle
        float altitude = Pi/2 - setup;
        float altitudeAbs = Mathf.Abs(altitude);
        float dropoff_rad = 10 * Mathf.Deg2Rad;
		

        // Set sun and moon intensity
        if (altitude > 0)
        {
			if(!_sunLight.enabled) _sunLight.enabled = true;
			if(_moonLight.enabled) _moonLight.enabled = false;
			float sunIntensityMax = Lighting.SunIntensity + Lighting.SunWeatherMod;
           
			if (altitudeAbs < dropoff_rad)
            {
                _sunLight.intensity = Mathf.Lerp(0, sunIntensityMax, altitudeAbs / dropoff_rad);
            }
			else _sunLight.intensity = Mathf.Lerp(_sunLight.intensity, sunIntensityMax,0.01f);

        }
        else
        {
			if(_sunLight.enabled) _sunLight.enabled = false;
			if(!_moonLight.enabled) _moonLight.enabled = true;

			float moonIntensityMax = Lighting.MoonIntensity * Mathf.Clamp01(1 - Mathf.Abs(Lighting.MoonPhase));
           
			if (altitudeAbs < dropoff_rad)
            {
                _moonLight.intensity = Mathf.Lerp(0, moonIntensityMax, altitudeAbs / dropoff_rad);
            }
            else 
				_moonLight.intensity = moonIntensityMax;
        }
    }

    private bool _banLight;
    public void EnableLight(bool b)
    {
        _banLight = !b;
        _sunLight.enabled = b;
        _moonLight.enabled = b;

        if (!b)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.ambientLight = Color.white;
        }
        else
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
        }
    }

    // Make the parameters stay in reasonable range
    private void ValidateParameters()
    {
        // Keep GameTime Parameters right!
		GameTime.Hours = Mathf.Repeat(GameTime.Hours, 24);
		//GameTime.Days = Mathf.Repeat(GameTime.Days - 1, (EnvMgr.seasons.SpringInDays + EnvMgr.seasons.SummerInDays + EnvMgr.seasons.AutumnInDays + EnvMgr.seasons.WinterInDays)) + 1;
		GameTime.Longitude = Mathf.Clamp(GameTime.Longitude, -180, 180);
		GameTime.Latitude = Mathf.Clamp(GameTime.Latitude, -90, 90);

		#if UNITY_EDITOR
		if (GameTime.DayLengthInMinutes == 0)
		{
			GameTime.Hours = 12f;
			Lighting.MoonPhase = 0f;
		}
		#endif

		// Give correct preview in EditoMode:
        // Sun
        #if UNITY_EDITOR
		Lighting.SunIntensity = Mathf.Max(0, Lighting.SunIntensity);
        #endif

        // Moon
        #if UNITY_EDITOR
		Lighting.MoonIntensity = Mathf.Max(0, Lighting.MoonIntensity);
		Lighting.MoonPhase = Mathf.Clamp(Lighting.MoonPhase, -1, +1);
        #endif
    }

    // Convert spherical coordinates to cartesian coordinates
    private Vector3 SphericalToCartesian(float radius, float theta, float phi)
    {
        Vector3 res;

        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float sinPhi   = Mathf.Sin(phi);
        float cosPhi   = Mathf.Cos(phi);

        res.x = radius * sinTheta * cosPhi;
        res.y = radius * cosTheta;
        res.z = radius * sinTheta * sinPhi;

        return res;
    }

    //Create the skyDome mesh
    void CreateDome (Mesh mesh, int segments)
    {
        Vector3[] vertices = new Vector3[segments * (segments - 1) + 2];
        Vector3[] normals = new Vector3[segments * (segments - 1) + 2];
        Vector2[] uv = new Vector2[segments * (segments - 1) + 2];

        int[] indices = new int[2 * segments * (segments - 1) * 3];

        float deltaLatitude = Pi / (float)segments;
        float deltaLongitude = Pi * 2.0f / (float )segments;

        // Generate the rings
        int index = 0;
        for (int i = 1; i < segments; i++) 
		{
            float r0 = Mathf.Sin (i * deltaLatitude);
            float y0 = Mathf.Cos (i * deltaLatitude);

            for (int j = 0; j < segments; j++) 
			{
                float x0 = r0 * Mathf.Sin (j * deltaLongitude);
                float z0 = r0 * Mathf.Cos (j * deltaLongitude);

                vertices[index].x = x0;
                vertices[index].y = y0;
                vertices[index].z = z0;

                normals[index].x = -x0;
                normals[index].y = -y0;
                normals[index].z = -z0;

                uv[index].x = 0;
                uv[index].y = 1 - y0;

                index++;
            }
        }

        // Generate the UPside
        vertices[index].x = 0;
        vertices[index].y = 1;
        vertices[index].z = 0;

        normals[index].x = 0;
        normals[index].y = -1;
        normals[index].z = 0;

        uv[index].x = 0;
        uv[index].y = 0;

        index++;

        vertices[index].x = 0;
        vertices[index].y = -1;
        vertices[index].z = 0;

        normals[index].x = 0;
        normals[index].y = 1;
        normals[index].z = 0;

        uv[index].x = 0;
        uv[index].y = 2;

        index = 0;
        // Generate the midSide
        for (int i = 0; i < segments - 2; i++) 
		{
            for (int j = 0; j < segments; j++) 
			{
                indices[index++] = segments * i + j;
                indices[index++] = segments * i + (j + 1) % segments;
                indices[index++] = segments * (i + 1) + (j + 1) % segments;
                indices[index++] = segments * i + j;
                indices[index++] = segments * (i + 1) + (j + 1) % segments;
                indices[index++] = segments * (i + 1) + j;
            }
        }

        // Generate the upper cap
        for (int i = 0; i < segments; i++) 
		{
            indices[index++] = segments * (segments - 1);
            indices[index++] = (i + 1) % segments;
            indices[index++] = i;
        }

        // Generate the lower cap
        for (int i = 0; i < segments; i++)
		{
            indices[index++] = segments * (segments - 1) + 1;
            indices[index++] = segments * (segments - 2) + i;
            indices[index++] = segments * (segments - 2) + (i + 1) % segments;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = indices;
    }

    private float _currShadowFac;

    public void EnableCastShadow(float shadowFac)
    {
        var b = shadowFac > 0;
        if (_sunLight != null)
        {
            _sunLight.shadows = b ? LightShadows.Hard : LightShadows.None;
            if(b) _sunLight.shadowStrength = shadowFac;
        }

        if (_moonLight != null)
        {
            _moonLight.shadows = b ? LightShadows.Hard : LightShadows.None;
            if(b) _moonLight.shadowStrength = 0.35f * shadowFac;
        }
                
        _currShadowFac = shadowFac;
    }

    public void RefreshShadowState()
    {
        EnableCastShadow(_currShadowFac);
    }

    public void ChangeAmbientLights(float sunFac, float moonFac, Gradient directLightColor, Gradient sunDiskColor,
        Gradient equatorColor, Gradient ambientLightColor, Gradient groundColor)
    {
        _currSunFac = sunFac;
        _currMoonFac = moonFac;
        _currDirectLightColor = directLightColor;
        _currSunDiskColor = sunDiskColor;
        _currEquatorColor = equatorColor;
        _currAmbientLightColor = ambientLightColor;
        _currGroundColor = groundColor;
    }

    public void RevertAmbientLights()
    {
        _currSunFac = 1f;
        _currMoonFac = 1f;
        _currSunIntensity = _defualtSunIntensity;
        _currMoonIntensity = _defaultMoonIntensity;

        _currDirectLightColor = Lighting.DirectLightColor;
        _currSunDiskColor = Lighting.SunDiskColor;
        _currEquatorColor = Lighting.EquatorColor;
        _currAmbientLightColor = Lighting.AmbientLightColor;
        _currGroundColor = Lighting.GroundColor;
    }


    private bool _fadingEnterRoom;
    private bool _fadingOutRoom;

    public void EnterRoom()
    {
        if (IsNight) return;
        _fadingOutRoom = false;
        _fadingEnterRoom = true;
        _currSunIntensity = 0.105f;
    }

    public void ExitRoom()
    {
        if (IsNight) return;
        
        _fadingEnterRoom = false;
        _fadingOutRoom = true;

        var fac = (Mathf.Abs(GameTime.Hours - 12) + 12) / 12f;
        _currSunIntensity = _defualtSunIntensity * fac * 2.5f;
    }

    

    public void EnableSkyObject(bool b)
    {
        if (_moonRenderer) _moonRenderer.enabled = b;
        if (_sunRenderer) _sunRenderer.enabled = b;
        if (_atmosphereRenderer) _atmosphereRenderer.enabled = b;
        if (Components.Cloud)
        {
            Components.Cloud.SetActiveSafe(b);
        }
    }
}
