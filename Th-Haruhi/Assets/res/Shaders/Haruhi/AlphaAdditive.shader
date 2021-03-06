// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Haruhi/AlphaAdditive" {
Properties {

	_MainTex("Particle Texture", 2D) = "white" {}
	_TintColor ("Tint Color", Color) = (1, 1, 1, 1)
	_BrightnessColor("Brightness Color", Color) = (1, 1, 1, 1)
	_Gamma ("Gamma", Range(0, 8)) = 1
	_Brightness ("Brightness", Range(0, 8)) = 1
	_AlphaGamma ("AlphaGamma", Range(0, 20)) = 1
	_AlphaScale ("AlphaScale", Range(0, 2)) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _TintColor;
			fixed4 _BrightnessColor;
			half _Gamma;
			half _Brightness;
			half _AlphaGamma;
			half _AlphaScale;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				col.rgb = pow(col.rgb, _Gamma) * _Brightness;
				col.a = pow(col.a, _AlphaGamma) * _AlphaScale;
				return col;
			}
			ENDCG 
		}
	}	
}
}
