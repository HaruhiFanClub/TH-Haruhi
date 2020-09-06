Shader "XSJ/Char/Diffuse HalfLambert"
{
	Properties
	{
		_MainTex("基础纹理", 2D) = "white" {}
		_Color ("基础颜色", Color)=(1,1,1,1)
		_ShadowStr("阴影强度", Range(0, 1)) = 0.3

		_BumpMap("法线纹理",2D) = "bump"{}
		_BumpScale("法线凹凸程度",Range(0,1)) = 1.0

		_SpecularMask("高光遮罩纹理", 2D) ="white" {}
		_Specular("高光颜色", Color) = (1,1,1,1)
		_Gloss("光滑度", Range(8.0,256)) = 256

		[Space(20)]
		[LightDir]_SubLightDir("辅光方向", Vector) = (0,0,-1,0)
		_SubLightColor("辅光颜色", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{"RenderType"="Opaque" "LightMode"="ForwardBase"}
		Pass
		{
			CGPROGRAM

			//#pragma
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase

			// cginc	
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityCG.cginc"

			//variables
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _ShadowStr;

			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;

			sampler2D _SpecularMask;
			fixed4 _Specular;
			float _Gloss;

			fixed4 _SubLightColor;
			half4 _SubLightDir;

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 lightDir : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				float4 uv : TEXCOORD2;
				UNITY_FOG_COORDS(3)
				SHADOW_COORDS(4)
				half3 subLightDir  : TEXCOORD5;
			};

			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);				
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _BumpMap);
				
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;

				o.subLightDir = mul(rotation, -_SubLightDir.xyz).xyz;
				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);

				fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
				fixed3 tangentNormal = UnpackNormal(packedNormal);
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));


				fixed3 albedo = tex2D(_MainTex,i.uv).rgb * _Color.rgb;  

			    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				//HalfLambert Diffuse
			    fixed halfLambert = dot(tangentNormal, tangentLightDir) * 0.5 + 0.5;
			    fixed3 diffuse = _LightColor0.rgb * albedo.rgb * halfLambert;

				//Blinn-Phong Specular
				fixed4 specularTex = tex2D(_SpecularMask, i.uv.xy);
				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(tangentNormal,halfDir)), _Gloss) * specularTex;

				fixed shadow = lerp(_ShadowStr, 1,  SHADOW_ATTENUATION(i)); 

				half3 subLightDir = normalize(i.subLightDir);
				half subDiff = saturate(dot(tangentNormal, subLightDir)) * _SubLightColor.rgb * tex2D(_MainTex,i.uv.xy);

			    fixed3 finalRGBA = ambient + diffuse * shadow  + specular + subDiff;

				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				return fixed4(finalRGBA,1.0);
			}
			ENDCG
		}
	}
	Fallback "Mobile/VertexLit"
}