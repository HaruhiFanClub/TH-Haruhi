Shader "XSJ/VFX/Flash"
{
	Properties
	{
		//主纹理
		_MainTex("Main Texture", 2D) = "white" {}

		//流光纹理1
		_FlashTex("Flash Texture1",2D) = "white"{}
		//流光颜色1
		[HDR]_FlashColor("Flash Color",Color) = (1,1,1,1)
		//流光强度1
		_FlashIntensity("Flash Intensity", Range(0, 1)) = 0.6
		//水平流动速度1
		_FlashSpeedX("Flash Speed X", Range(-5, 5)) = 0
		//垂直流动速度1
		_FlashSpeedY("Flash Speed Y", Range(-5, 5)) = 0

		//流光纹理2
		_FlashTex1("Flash Texture2",2D) = "white"{}
		//流光颜色2
		[HDR]_FlashColor1("Flash Color",Color) = (1,1,1,1)
		//流光强度2
		_FlashIntensity1("Flash Intensity", Range(0, 1)) = 0.6
		//水平流动速度2
		_FlashSpeedX1("Flash Speed X", Range(-5, 5)) = 0
		//垂直流动速度2
		_FlashSpeedY1("Flash Speed Y", Range(-5, 5)) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 uv0 : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_ST;
			float4 _FlashTex_ST;
			float4 _FlashTex1_ST;
			sampler2D _MainTex;
			sampler2D _FlashTex;
			sampler2D _FlashTex1;
			fixed4 _FlashColor;
			fixed4 _FlashColor1;
			fixed _FlashIntensity;
			fixed _FlashIntensity1;
			fixed _FlashSpeedX;
			fixed _FlashSpeedX1;
			fixed _FlashSpeedY;					
			fixed _FlashSpeedY1;			

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv0.xy = TRANSFORM_TEX(v.texcoord1, _FlashTex);
				o.uv0.zw = TRANSFORM_TEX(v.texcoord1, _FlashTex1);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//=====================计算流光贴图1的uv=====================
				//获取流光UV
				float2 flashUV = i.uv0.xy;
				//不断改变uv的x轴，让他往x轴方向移动
				flashUV.x += -_Time.y*_FlashSpeedX;
				//不断改变uv的y轴，让他往y轴方向移动
				flashUV.y += -_Time.y*_FlashSpeedY;
				//取流光贴图的alpha值
				fixed flashAlpha = tex2D(_FlashTex, flashUV).a;
				//最终在主纹理上的可见值（flashAlpha为0则该位置不可见）
				fixed visible = flashAlpha*_FlashIntensity;

				//=====================计算流光贴图2的uv=====================
				//获取流光UV
				float2 flashUV1 = i.uv0.zw;
				//不断改变uv的x轴，让他往x轴方向移动
				flashUV1.x += -_Time.y*_FlashSpeedX1;
				//不断改变uv的y轴，让他往y轴方向移动
				flashUV1.y += -_Time.y*_FlashSpeedY1;
				//取流光贴图的alpha值
				fixed flashAlpha1 = tex2D(_FlashTex1, flashUV1).a;
				//最终在主纹理上的可见值（flashAlpha为0则该位置不可见）
				fixed visible1 = flashAlpha1*_FlashIntensity1;

				//=====================最终输出=====================
				//主纹理 + 可见的流光
				fixed4 col = tex2D(_MainTex, i.uv) + visible*_FlashColor + visible1*_FlashColor1;

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

