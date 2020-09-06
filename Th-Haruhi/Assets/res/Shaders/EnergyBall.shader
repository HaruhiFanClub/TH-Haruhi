// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


//////////////////////////////////////////////////////////////////////////
//
//   FileName : Character.shader
//     Author : Chiyer
// CreateTime : 2014-03-12
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

Shader "Fps/Effects/EnergyBall" {

	Properties {
		_MainTex("Main_Texture", 2D) = "white" {}
		_Color01("Main_Texture_Color", Color) = (1,1,1,1)
		_Blend_Texture("Blend_Texture", 2D) = "white" {}
		_Color02("Blend_Texture_Color", Color) = (1,1,1,1)
		_Blend_Texture01("Blend_Texture01", 2D) = "black" {}
		_Speedx("Main_Texutre_Speedx", Float) = 0
		_Speed01x("Blend_Texture_Speedx", Float) = 0
		_Speed02x("Blend_Texture01_Speedx", Float) = 0

		_Speedy("Main_Texutre_Speedy", Float) = 0
		_Speed01y("Blend_Texture_Speedy", Float) = 0
		_Speed02y("Blend_Texture01_Speedy", Float) = 0



		_Fresnel_Value("Fresnel_Value", Range(0,3) ) = 0.5
		_Lighten("Lighten", Float) = 1
	}

    SubShader {
	
		Tags { "Queue"="Transparent" "IgnoreProjector"="False" "RenderType"="Transparent" }

		Lighting Off
		ZWrite Off 
		Blend One One
		Fog { Mode Off }

        Pass 
        {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag                 
			#include "UnityCG.cginc"
               
			struct vsIn
			{
				float4 vertex	: POSITION;
				float3 normal   : NORMAL;
				float2 texcoord : TEXCOORD0;
			};
                
			struct vsOut
			{
				float4  pos		: SV_POSITION;
				float2  uv1		: TEXCOORD0;           
				float2  uv0		: TEXCOORD1;           
				float2  uv2		: TEXCOORD2;           
				float3  normal  : TEXCOORD3;                
				float3  viewdir : TEXCOORD4;
			};
               
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Blend_Texture;
			float4 _Blend_Texture_ST;
			sampler2D _Blend_Texture01;
			float4 _Blend_Texture01_ST;
			float4 _Color01;
			float4 _Color02;
			float _Speedx;
			float _Speed01x;
			float _Speed02x;

			float _Speedy;
			float _Speed01y;
			float _Speed02y;

			float _Fresnel_Value;
			float _Lighten;
                
			vsOut vert(vsIn In)
			{
				vsOut o;
				o.pos = UnityObjectToClipPos(In.vertex);
				o.uv1 = TRANSFORM_TEX(In.texcoord, _MainTex);
				o.uv0 = TRANSFORM_TEX(In.texcoord, _Blend_Texture);
				o.uv2 = TRANSFORM_TEX(In.texcoord, _Blend_Texture01);
				o.normal = In.normal;
				o.viewdir = ObjSpaceViewDir(In.vertex);
				return o;
			}
                
			fixed4 frag(vsOut In) : COLOR
			{
				half4 Fresnel0 = (1.0 - dot(normalize(In.viewdir), normalize(In.normal))).xxxx;
				half4 Pow0 = pow(Fresnel0, _Fresnel_Value.xxxx);

				half2 UV_Pan1 = half2(In.uv1.x + _Time.x * _Speedy, In.uv1.y + _Time.x * _Speedx);
				half2 UV_Pan0 = half2(In.uv0.x + _Time.x * _Speed01y, In.uv0.y + _Time.x * _Speed01x);
				half2 UV_Pan2 = half2(In.uv2.x + _Time.x * _Speed02y, In.uv2.y + _Time.x * _Speed02x);

				fixed4 Tex2D0 = tex2D(_MainTex, UV_Pan1);
				fixed4 Tex2D1 = tex2D(_Blend_Texture, UV_Pan0);
				float4 Tex2D2 = tex2D(_Blend_Texture01, UV_Pan2);

				half4 Multiply5 = _Color01 * Tex2D0;
				half4 Multiply6 = _Color02 * Tex2D1;
				half4 Add0 = Multiply5 + Multiply6;
				half4 Multiply0 = Tex2D0 * Tex2D1;
				half4 Multiply7 = Add0 * Multiply0;
				half4 Multiply3 = Pow0 * Multiply7;

				half4 Multiply8 = Multiply3 * Tex2D2;
				half4 Multiply9 = Multiply8 * _Lighten.xxxx;



				return Multiply9;
				return 0;
			}

            ENDCG
        }
    }

	FallBack "Diffuse"
}

