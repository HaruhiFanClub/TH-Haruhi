Shader "Haruhi/Distortion(GrabPass)"
{
	Properties
	{
		_DistortStrength("Intisity",Range(0,1)) = 0.5
		_DistortVelocity("Speed",Range(0,1)) = 0.5
		_XDensity("Horiz",float) = 1
		_YDensity("Vert",float) = 1
		_NoiseTex("MainTex",2D) = "white"{}
		_Mask("Mask",2D) = "Black"{}
		_Color("Main Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags{
			 "RenderType" = "Transparent"
			 "Queue" = "Transparent+1"
			 "DisableBatching" = "True"//批处理会合并模型，造成物体本地坐标丢失，公告牌效果失效，所以忽视批处理
			}
		Zwrite Off
		GrabPass{"_GrabTex"}//获取当前屏幕图像，并存入_GrabTex纹理
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				fixed4 color : COLOR;
				float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
				float4 uv : TEXCOORD1;
			};

			sampler2D _GrabTex;
			sampler2D _NoiseTex;
			sampler2D _Mask;
			float _XDensity;
			float _YDensity;
			float4 _NoiseTex_ST;
			float4 _Color;
			fixed _DistortStrength;
			fixed _DistortVelocity;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				float3 center = float3(0, 0, 0);
				float3 viewer = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
				float3 normalDir = normalize(viewer - center);
				float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
				float3 rightDir = normalize(cross(upDir, normalDir));
				upDir = normalize(cross(normalDir, rightDir));
				float3 centerOff = v.vertex.xyz - center;
				float3 localPos = center + rightDir * centerOff.x + center + upDir * centerOff.y + center + normalDir * centerOff.z;

				//热扰动实现
				o.pos = UnityObjectToClipPos(float4(localPos,1));
				o.grabPos = ComputeGrabScreenPos(o.pos);//该顶点位于屏幕哪个位置，就用哪个位置去采样GrabPass得到该点理应呈现的画面

			

				//通过缩放纹理控制噪声纹理密度，改变扰动密集感
				_NoiseTex_ST.xy *= float2(_XDensity,_YDensity);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _NoiseTex);//这是用于采样噪声的有缩放变化的uv
				o.uv.zw = v.texcoord;//这是用于采样_Mask贴图的正常uv
				o.uv.xy -= _Time.y * _DistortVelocity;//根据时间因素在uv上不断变化从而获得流动的噪点值，增加热扰动流动感

				o.color = v.color * _Color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//采样噪声，获得rg两个通道上的值作为两个方向上的偏移量
				float2 offset = tex2D(_NoiseTex,i.uv.xy).xy;
				//原取得的值在0到1，重映射到-1到1，增加扰动方向的随机感，并用_DistortStrength更改采样偏移距离，控制扰动强度
				offset = (offset - 0.5) * 2 * _DistortStrength;
				//采样偏移量乘上采样遮罩的值，该值为0到1，既遮罩白色部分正常扰动，黑色部分无扰动，中间灰色则为过度
				i.grabPos.xy += tex2D(_Mask, i.uv.zw).x * offset;
				//偏移后的屏幕坐标去采样抓取的屏幕纹理
				fixed4 color =  tex2D(_GrabTex, i.grabPos);
				fixed4 maskColor = tex2D(_Mask, i.uv.zw) * i.color;
				return color + maskColor;
			}
			ENDCG
		}
	}
}