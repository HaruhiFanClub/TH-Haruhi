Shader "XSJ/VFX/Standard" { 
    Properties {
        [HDR]_Color ("Color", Color) = (1, 1, 1, 1)
        _ShadeColor ("ShadeColor",Color)=(0.9,0.28,0.23,1)
        _ShadeRange ("ShadeRange",Range(1,8))=1
		_MainTex ("MainTex", 2D) = "white" {}
		_MainTexAngle ("MainTexAngle", Range(0, 360)) = 0
		_MainTexSpeed_U ("MainTexSpeed_U", Float) = 0
		_MainTexSpeed_V ("MainTexSpeed_V", Float) = 0
		_MainTexStrength ("MainTexStrength", Range(0, 10)) = 1
        
		//Mask
		_MaskTex ("MaskTex", 2D) = "white" {}
		_MaskRGBA ("MaskRGBA", Vector) = (0, 0, 0, 1)
		_MaskTexAngle ("MaskTexAngle", Range(0, 360)) = 0
		//DisTex
		_DistortTex ("DistortTex", 2D) = "white" {}
		_DistRGBA ("DistRGBA", Vector) = (0, 0, 0, 1)
		_DistForceU ("DistStrength_U", range (-1,1)) = 1
		_DistForceV ("DistStrength_V", range (-1,1)) = 1
        _DistTime("DistSpeed", range (-1,1)) = 0.1
		
		[HideInInspector] _SrcBlend("SrcBlend", Int) = 5 // SrcAlpha
		[HideInInspector] _SrcBlend ("SrcBlend", Int) = 5 // SrcAlpha
		[HideInInspector] _DstBlend ("DstBlend", Int) = 10 // OneMinusSrcAlpha
		[HideInInspector] _CullMode ("_CullMode", Int) = 2 // Back
    }
    SubShader {
        Tags {"IgnoreProjector"="True""Queue"="Transparent""RenderType"="Transparent"}
        
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }

            Cull [_CullMode]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite Off
           
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" 
            #pragma target 3.0
            #pragma multi_compile __ _USE_DISTORT_ON
            #pragma multi_compile __ _USE_MASK_ON
            #pragma multi_compile __ _USE_SHADECOLOR_ON
            //Main贴图模块
            fixed4 _Color,_ShadeColor; 
            fixed _ShadeRange,_DistTime;
            sampler2D _MainTex; 
            fixed4 _MainTex_ST;
            fixed _MainTexAngle;
            fixed _MainTexSpeed_U;
            fixed _MainTexSpeed_V;
            fixed _MainTexStrength;

            //Mask贴图模块
            sampler2D _MaskTex;
            fixed4 _MaskTex_ST;
            fixed _MaskTexAngle;
            fixed4 _MaskRGBA;
            //扭曲纹理模块
            sampler2D _DistortTex;
            fixed _DistForceU;
            fixed _DistForceV;
            fixed4 _DistortTex_ST;
            fixed4 _DistRGBA; 
            struct a2v {
                fixed4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord0 : TEXCOORD0;
            };
            struct v2f {
                fixed4 pos : SV_POSITION;
                fixed4 uv : TEXCOORD0;
                fixed4 uv1 : TEXCOORD1;
                fixed4 uv2 : TEXCOORD2;
                fixed4 vColor : COLOR;
            };
            v2f vert (a2v v) {
                v2f o = (v2f)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.vColor = v.color;
                fixed oneDegree = (UNITY_PI / 180.0);
				o.uv.xy = v.texcoord0;//标准UV

            //DisUV
            #ifdef _USE_DISTORT_ON
				float2 disSpeedA = _Time.xz * _DistTime;
				fixed2 uv_DisA = (o.uv.xy * _DistortTex_ST.xy + _DistortTex_ST.zw); 
                o.uv.zw = uv_DisA + disSpeedA;
				float2 disSpeedB = _Time.yx * _DistTime;
                fixed2 uv_DisB = (o.uv.xy * _DistortTex_ST.xy + _DistortTex_ST.zw);
                o.uv2.zw = uv_DisB + disSpeedB; 
			#else
				o.uv.zw = o.uv.xy; 				
			#endif
            o.uv1.xy = o.uv.xy;
		
            //MaskUV
            #ifdef _USE_MASK_ON
            	fixed2 uv_MaskTex = (o.uv.xy * _MaskTex_ST.xy + _MaskTex_ST.zw); 
				fixed cos110 = cos(oneDegree * _MaskTexAngle); 
				fixed sin110 = sin(oneDegree * _MaskTexAngle); 
				fixed2 maskTexRotator = mul(uv_MaskTex - fixed2(0.5, 0.5), fixed2x2(cos110,  - sin110, sin110, cos110)) + fixed2(0.5, 0.5); 
                o.uv1.zw = maskTexRotator;                

			#else
                o.uv1.zw = o.uv.xy;
			#endif
            //UVMainTex
                fixed2 mainSpeed = _Time.y * (fixed2(_MainTexSpeed_U, _MainTexSpeed_V));
                fixed2 uv_Main = (o.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw); 
                fixed cosMain = cos(oneDegree * _MainTexAngle); 
                fixed sinMain = sin(oneDegree * _MainTexAngle); 
                fixed2 mainRota = mul(uv_Main - fixed2(0.5, 0.5), fixed2x2(cosMain,  - sinMain, sinMain, cosMain)) + fixed2(0.5, 0.5); 
                o.uv2.xy = mainRota + mainSpeed;
                return o;
            }
//Frag函数
            fixed4 frag(v2f i) : COLOR {
                
			//DisTex扭曲模块（只扭曲MainTex与DetailTex）
			#ifdef _USE_DISTORT_ON
				fixed4 disModA = tex2D(_DistortTex, i.uv.zw); 				
				fixed4 disModB = tex2D(_DistortTex, i.uv2.zw); 
                fixed4 disMod = (disModA + disModB)-1;
                fixed disMask = dot(disMod, _DistRGBA); 
				fixed2 disUV = fixed2(disMask * _DistForceU , disMask * _DistForceV); 
			#else
				fixed2 disUV = fixed2(0,0); 				
			#endif

            //MainTex模块
                fixed2 uv01 = i.uv2.xy + disUV;
                fixed4 mainTexModule = tex2D(_MainTex, uv01); 
                fixed3 colorOut1 = mainTexModule.rgb; 
                fixed alphaOut1 = mainTexModule.a * _MainTexStrength; 

				fixed3 colorOut2 = colorOut1; 
				fixed alphaOut2 = alphaOut1; 

            //MaskTex模块
            #ifdef _USE_MASK_ON
                //fixed2 uv03 = i.uv1.zw + disUV;
				fixed4 maskTexModule = tex2D(_MaskTex, i.uv1.zw); 
				fixed alphaMask = dot(maskTexModule, _MaskRGBA); 
				fixed alphaOut3 = alphaMask * alphaOut2; 
			#else
				fixed alphaOut3 = alphaOut2; 
			#endif

            fixed alphaOut = clamp ((i.vColor.a * _Color.a  * alphaOut3),0,1);            
            //LerpColor
            #ifdef _USE_SHADECOLOR_ON
                fixed lerpAlpha = pow(max(alphaOut,0.00000001),_ShadeRange);
                fixed3 lerpColor = lerp(_ShadeColor,_Color,lerpAlpha);
            #else
                fixed3 lerpColor = _Color;
            #endif
                         
                fixed3 colorOut = colorOut2 * i.vColor * lerpColor;			
                fixed3 finalColor = colorOut;
                return fixed4(finalColor,alphaOut);
            }
            ENDCG
        }
    }
	CustomEditor "FX_StandardInspector"
}


