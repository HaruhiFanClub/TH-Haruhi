// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Enviro/Mobile/AtmosphereShader" 
{ 
    Properties
    {
        _SunColor       ("Sun Color", Color) = (0, 0, 0, 0)
        _SunDirection      ("Sun Direction",  Vector) = (0, 0, 0, 0)
		_GradientsMap 	   ("GradientsMap", 2D)  = "black" {}
		_AtmRelativeDepth   ("AtmRelativeDepth", 2D) = "black" {}
		_StarsMap ("StarsMap", Cube) = "white" {}
		 
    }

CGINCLUDE
	float bias (float b, float x)
	{
		return pow (x, log (b) / log (0.5));
	}

	float4 sunlightInscatter
	(
		float4 sunColour,
		float absorption,
		float incidenceAngleCos,
		float sunlightScatteringFactor
	)
	{
		float scatteredSunlight = bias (sunlightScatteringFactor * 0.5, incidenceAngleCos);

		sunColour = sunColour * (1 - absorption) * float4 (0.9, 0.5, 0.09, 1);
		
		return sunColour * scatteredSunlight;
	}
ENDCG
	
    SubShader
    {
		Lod 300
        Tags
        {
            "Queue"="Background"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Cull Back
            ZWrite Off
            //ZTest On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

            float4 _SunColor;
            float3 _SunDirection;
			sampler2D _GradientsMap;
			sampler2D _AtmRelativeDepth;
			uniform samplerCUBE _StarsMap;
			uniform float4x4 _Rotation;

			struct VertexInput 
             {
                float4 vertex : POSITION;
                
                float2 texcoord : TEXCOORD0;
             };


            struct v2f {
                float4 position : POSITION;
                fixed4 color    : COLOR;
                float4 WorldPosition : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float3 stars : TEXCOORD2;
            };

            v2f vert(VertexInput v) {
                v2f o;

                float cosTheta  = _SunDirection;
				float elevation = _SunDirection.y * 0.5f + 0.5f;

                o.color = tex2Dlod(_GradientsMap, float4 (v.texcoord.x + elevation, 1 - v.texcoord.y, 0, 0));
                o.stars = mul((float3x3)_Rotation,v.vertex.xyz);

                o.position  = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.position);
				o.WorldPosition = normalize(mul((float4x4)unity_ObjectToWorld, v.vertex)).xyzw;

				// Sunlight inscatter
				if (-cosTheta > 0)
				{
					const float sunlightScatteringFactor = 0.05;
					const float sunlightScatteringLossFactor = 0.1;
					const float atmLightAbsorptionFactor = 0.2;
					
					o.color.rgb += sunlightInscatter (
							_SunColor, 
							clamp (atmLightAbsorptionFactor * (1 - tex2Dlod(_AtmRelativeDepth, float4(-_SunDirection.y,0,0,0)).r), 0, 1), 
							clamp (-cosTheta, 0, 1), 
							sunlightScatteringFactor).rgb * (1 - sunlightScatteringLossFactor);
				}
			
                return o;
            }

            fixed4 frag(v2f i) : COLOR 
            {
            	float3 viewDir = normalize(i.WorldPosition);
            	float4 _StarsMap_var = texCUBE(_StarsMap,i.stars);
                float4 nightSky = float4((20 * _StarsMap_var.rgb),1);

            	fixed4 fogC = i.color; 
            	UNITY_APPLY_FOG(i.fogCoord, fogC);
       
            	i.color = lerp(fogC,i.color,saturate(dot(viewDir.y, float3(0,1,0))));
 				i.color = i.color +  (1-i.color.a) * nightSky;

                return i.color;
            }

            ENDCG
        }
    }

}
