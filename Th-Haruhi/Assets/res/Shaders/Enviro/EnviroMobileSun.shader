// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Enviro/Mobile/Sun"
{
	Properties
	{
		_MainTex ("Sun Texture", 2D) = "white" {}
		_Color   ("Color", Color) = (1,1,1,1)
        _Brightness ("Brightness",   float) = 1
        _Contrast ("Contrast",    float) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue"="Background+1"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}

		Pass
		{
			Cull Back
			ZWrite Off
			ZTest LEqual
			Blend One One
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform float _Contrast;
			uniform float _Brightness;
			uniform sampler2D _MainTex;

			struct v2f {
				float4 position : SV_POSITION;
				float3 tex      : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;

				o.position = UnityObjectToClipPos(v.vertex);
				o.tex.xy   = v.texcoord;
				return o;
			}

			half4 frag(v2f i) : COLOR 
			{
				half4 color = half4(1, 1, 1, 1);
				half alpha = tex2D(_MainTex, i.tex.xy).a;
				alpha = alpha * _Brightness;
				alpha = pow(alpha, _Contrast);
				color.rgb = (_Color.rgb * _Color.rgb* _Color.rgb) * alpha;
				return color;
			}

			ENDCG
		}
	}

	Fallback Off
}
