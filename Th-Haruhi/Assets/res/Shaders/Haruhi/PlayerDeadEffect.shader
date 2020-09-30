Shader "Haruhi/PlayerDeadEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Radius("圆半径", Range(0,10)) = 0.1
		_SmallOffset("小圆偏移", Range(0,1)) = 0.1

		_CenterX("中心点 X", Range(0,1)) = 0.2
		_CenterY("中心点 Y", Range(0,1)) = 0.4

		_ScreenRatio("屏幕高宽比 ", Range(0,1)) = 0.75
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			//fixed _LuminosityAmount;
			fixed4 finalColor;
			float _Radius;
			float _CenterX;
			float _CenterY;
			float _ScreenRatio;
			float _SmallOffset;

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed4 renderTex = tex2D(_MainTex, i.uv);

				//中心大圆
				float2 distCenter = float2(_CenterX - i.uv.x, (_CenterY - i.uv.y) * _ScreenRatio);

				//上下左右的小圆
				float2 distLeft = float2(_CenterX - _SmallOffset * _ScreenRatio - i.uv.x, (_CenterY - i.uv.y) * _ScreenRatio);
				float2 distRight = float2(_CenterX + _SmallOffset * _ScreenRatio - i.uv.x, (_CenterY - i.uv.y) * _ScreenRatio);
				float2 distUp = float2(_CenterX - i.uv.x, (_CenterY - _SmallOffset - i.uv.y) * _ScreenRatio);
				float2 distDown = float2(_CenterX - i.uv.x, (_CenterY + _SmallOffset - i.uv.y) * _ScreenRatio);
				
				
				float smallRadius = 0;
				float tinyRadius = 0;
				float tinyRadius2 = 0;

				if (_Radius > 0.03)
				{
					smallRadius = _Radius - 0.03;
					if (_Radius > 0.09)
					{
						tinyRadius = _Radius - 0.09;

						if (_Radius > 0.3)
						{
							tinyRadius2 = _Radius - 0.3;
						}
					}
				}
				
				float lengthCenter = length(distCenter);
				float lengthLeft = length(distLeft);
				float lengthRight = length(distRight);
				float lengthUp = length(distUp);
				float lengthDown = length(distDown);

				//最后出现的小圆，正色
				if (lengthCenter < tinyRadius2)
				{
					finalColor = renderTex;
				}
				//倒数第二个小圆，反色
				else if (lengthCenter < tinyRadius)
				{
					finalColor = 1 - renderTex;
					
				}
				//相邻3个smallcirle交汇，正色
				else if(lengthLeft < smallRadius && lengthUp < smallRadius && lengthDown < smallRadius ||
					    lengthRight < smallRadius && lengthUp < smallRadius && lengthDown < smallRadius ||
					    lengthLeft < smallRadius && lengthUp < smallRadius && lengthRight < smallRadius ||
						lengthLeft < smallRadius && lengthDown < smallRadius && lengthRight < smallRadius)
				{
					finalColor = renderTex;
				}

				//相邻2个smallcircle交汇，反色
				else if (lengthLeft < smallRadius && lengthUp < smallRadius ||
						 lengthLeft < smallRadius && lengthDown < smallRadius ||
						 lengthRight < smallRadius && lengthUp < smallRadius ||
						 lengthRight < smallRadius && lengthDown < smallRadius)
				{
					finalColor = 1 - renderTex;
				}
				//中心大圆于4侧小圆交汇，正色
				else if (lengthCenter < _Radius && lengthLeft < smallRadius ||
						 lengthCenter < _Radius && lengthUp < smallRadius ||
						 lengthCenter < _Radius && lengthRight < smallRadius ||
					     lengthCenter < _Radius && lengthDown < smallRadius)
				{
					finalColor = renderTex;
				}
				else if (lengthCenter < _Radius ||
						 lengthLeft < smallRadius  ||
						 lengthRight < smallRadius ||
						 lengthUp < smallRadius ||
						 lengthDown < smallRadius)
				{
					finalColor = 1 - renderTex;
				}
				else
				{
					finalColor = renderTex;
				}
				
				return finalColor;
			}
				
			ENDCG
		}
	}
	FallBack "Diffuse"
}