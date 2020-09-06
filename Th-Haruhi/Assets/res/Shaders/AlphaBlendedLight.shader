

Shader "Fps/Effects/Alpha Blended Light" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_ColorStrength ("Color strength", Float) = 1.0
	_DiffuseThreshold ("Lighting Threshold", Range(-1.1,1)) = 0.1
    _Diffusion ("Diffusion", Range(0.1,10)) = 1
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Float) = 0.5
	_LightDirSelf("LightDirSelf",vector) = (0,1,0,0)
	_LightColorSelf ("LightColorSelf",Color) = (1,1,1,1)
	
}

SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Pass {
			Fog { Mode Off}
			Blend SrcAlpha OneMinusSrcAlpha
		
			ZWrite Off
			Cull Off 
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			fixed4 _LightColorSelf;
			fixed _DiffuseThreshold;
            fixed _Diffusion;
			fixed _ColorStrength;
			fixed4 _LightDirSelf;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				fixed3 normalDir : TEXCOORD1;

                fixed3 viewDir : TEXCOORD3;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD4;
				#endif

			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				
				
				
				o.normalDir = normalize( mul( half4( v.normal, 0.0 ), unity_WorldToObject ).xyz );
                half4 posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = normalize( _WorldSpaceCameraPos.xyz - posWorld.xyz );
                half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - posWorld.xyz;
                

				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : COLOR
			{
				

				fixed nDotL = saturate(dot(i.normalDir, _LightDirSelf.xyz)) * _LightColorSelf.xyz;
				fixed diffuseCutoff = saturate( ( max(_DiffuseThreshold, nDotL) - _DiffuseThreshold ) * _Diffusion);

				fixed4 col = 2.0f * i.color * tex2D(_MainTex, i.texcoord);
				col.rgb *= _TintColor.rgb;
				col.a = saturate(col.a * _TintColor.a);
				fixed gray = 0.299*col.r + 0.587*col.g + 0.114*col.b;
				fixed3 lightCol = lerp(col.rgb * diffuseCutoff  * _LightColorSelf.rgb * _LightColorSelf.w * 4, col.rgb, saturate(gray));
				
				
				return fixed4(lightCol*_ColorStrength, col.a);
			}
			ENDCG 
		}
	}	
}