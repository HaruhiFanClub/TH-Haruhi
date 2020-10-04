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
			 "DisableBatching" = "True"//�������ϲ�ģ�ͣ�������屾�����궪ʧ��������Ч��ʧЧ�����Ժ���������
			}
		Zwrite Off
		GrabPass{"_GrabTex"}//��ȡ��ǰ��Ļͼ�񣬲�����_GrabTex����
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

				//���Ŷ�ʵ��
				o.pos = UnityObjectToClipPos(float4(localPos,1));
				o.grabPos = ComputeGrabScreenPos(o.pos);//�ö���λ����Ļ�ĸ�λ�ã������ĸ�λ��ȥ����GrabPass�õ��õ���Ӧ���ֵĻ���

			

				//ͨ����������������������ܶȣ��ı��Ŷ��ܼ���
				_NoiseTex_ST.xy *= float2(_XDensity,_YDensity);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _NoiseTex);//�������ڲ��������������ű仯��uv
				o.uv.zw = v.texcoord;//�������ڲ���_Mask��ͼ������uv
				o.uv.xy -= _Time.y * _DistortVelocity;//����ʱ��������uv�ϲ��ϱ仯�Ӷ�������������ֵ���������Ŷ�������

				o.color = v.color * _Color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//�������������rg����ͨ���ϵ�ֵ��Ϊ���������ϵ�ƫ����
				float2 offset = tex2D(_NoiseTex,i.uv.xy).xy;
				//ԭȡ�õ�ֵ��0��1����ӳ�䵽-1��1�������Ŷ����������У�����_DistortStrength���Ĳ���ƫ�ƾ��룬�����Ŷ�ǿ��
				offset = (offset - 0.5) * 2 * _DistortStrength;
				//����ƫ�������ϲ������ֵ�ֵ����ֵΪ0��1�������ְ�ɫ���������Ŷ�����ɫ�������Ŷ����м��ɫ��Ϊ����
				i.grabPos.xy += tex2D(_Mask, i.uv.zw).x * offset;
				//ƫ�ƺ����Ļ����ȥ����ץȡ����Ļ����
				fixed4 color =  tex2D(_GrabTex, i.grabPos);
				fixed4 maskColor = tex2D(_Mask, i.uv.zw) * i.color;
				return color + maskColor;
			}
			ENDCG
		}
	}
}