Shader "XSJ/VFX/FxStandard"
{
    Properties
    {
        [HDR]_Color ("基础颜色", Color) = (1,1,1,1)
        _MainTex ("基础贴图", 2D) = "white" {}
        _AlphaValue ("Alpha值", Range(0,10)) = 1
        _USpeed ("Uspeed", Float ) = 0
        _VSpeed ("Vspeed", Float ) = 0
        [HDR]_LerpColor ("LerpColor", Color) = (1,1,1,1)
        _LerpValue ("LerpValue",Range(0,1)) = 1
        [Toggle]_DiffuseRotate("贴图旋转",Float)=0
        _DiffuseAngle ("旋转角度", Range(0,360)) = 0


        [NoKeywordToggle] _DiffuseMask ("基础遮罩",Float)=0
        [Foldout] _DiffuseMaskLayerShown ("", Float) = 1 
        _DiffuseMaskTex ("遮罩贴图", 2D) = "white" {}
        _Mask_USpeed ("Mask_USpeed", Float ) = 0
        _Mask_VSpeed ("Mask_VSpeed", Float ) = 0
        [Toggle]_MaskRotate("遮罩旋转",Float)=0
        _MaskAngle ("旋转角度", Range(0,360)) = 0

        [NoKeywordToggle] _Distortion ("扭曲效果",Float) = 0
        [Foldout] _DistortionLayerShown ("", Float) = 1
        _DistortionTex ("扭曲贴图", 2D) = "white" {}
        _DistortionIntensity ("扭曲强度", Float) = 1
        _Distortion_USpeed ("Distortion_USpeed", Float ) = 0
        _Distortion_VSpeed ("Distortion_VSpeed", Float ) = 0

        [NoKeywordToggle] _Dissolution ("溶解效果", Float) = 0
        [Foldout] _DissolutionLayerShown ("",Float) = 1
        _DissolutionTex ("溶解贴图", 2D) = "white" {}
        _Dissolvability ("溶解程度", Range(0,1)) = 0
        [HDR]_EdgeColor ("边缘颜色", Color) = (1,1,1,1)
        _EdgeWidth ("边缘宽度", Range(0,1)) = 0


        [HideInInspector] _BlendMode ("__mode",Float) = 0.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _FaceMode ("__face", Float) = 0.0
        [HideInInspector] _CullMode ("__cull", Float) = 2.0
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane" 
        }

        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha [_DstBlend]
            Cull [_CullMode]
            Lighting Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ _DIFFUSEROTATE_ON
            #pragma multi_compile _ _DIFFUSEMASK_ON
            #pragma multi_compile _ _MASKROTATE_ON
            #pragma multi_compile _ _DISTORTION_ON
            #pragma multi_compile _ _DISSOLUTION_ON

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform half _AlphaValue;
            uniform half _USpeed;
            uniform half _VSpeed;
            uniform float4 _LerpColor;
            uniform half _LerpValue;

            #if _DIFFUSEROTATE_ON
            uniform half _DiffuseAngle;
            #endif

            #if _DIFFUSEMASK_ON
            uniform sampler2D _DiffuseMaskTex;
            uniform half4 _DiffuseMaskTex_ST;
            uniform half _Mask_USpeed;
            uniform half _Mask_VSpeed;
            #endif

            #if _MASKROTATE_ON
            uniform half _MaskAngle;
            #endif

            #if _DISTORTION_ON
            uniform sampler2D _DistortionTex;
            uniform half4 _DistortionTex_ST;
            uniform half _DistortionIntensity;
            uniform half _Distortion_USpeed;
            uniform half _Distortion_VSpeed;
            #endif

            #if _DISSOLUTION_ON
            uniform sampler2D _DissolutionTex;
            uniform half4 _DissolutionTex_ST;
            uniform half _Dissolvability;
            uniform fixed4 _EdgeColor;
            uniform half _EdgeWidth;
            #endif

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                #if _DIFFUSEMASK_ON
                float2 uv1 :TEXCOORD1;
                #endif
                #if _DISTORTION_ON
                float2 uv2 :TEXCOORD2;
                #endif
                #if _DISSOLUTION_ON
                float2 uv3 :TEXCOORD3;
                #endif
                UNITY_FOG_COORDS(4)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                #if _DIFFUSEROTATE_ON
                half2 uv_Main = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw; 
                half Angle_sin;
                half Angle_cos;
                sincos(_DiffuseAngle*0.01745, Angle_sin, Angle_cos);// π/180
                half2x2 RotateMatrix = half2x2( Angle_cos, -Angle_sin, Angle_sin, Angle_cos);
                o.texcoord = mul(uv_Main-half2(0.5,0.5),RotateMatrix)+half2(0.5,0.5);
                #else
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                #endif
                o.texcoord += fixed2(_USpeed*_Time.y,_VSpeed*_Time.y);

                #if _DIFFUSEMASK_ON
                #if _MASKROTATE_ON
                half2 uv_Mask = v.texcoord * _DiffuseMaskTex_ST.xy + _DiffuseMaskTex_ST.zw;
                half MaskAngle_sin;
                half MaskAngle_cos;
                sincos(_MaskAngle*0.01745, MaskAngle_sin, MaskAngle_cos);// π/180
                half2x2 MaskRotateMatrix = half2x2(MaskAngle_cos, -MaskAngle_sin, MaskAngle_sin, MaskAngle_cos);
                o.uv1 = mul(uv_Mask-half2(0.5,0.5),MaskRotateMatrix)+half2(0.5,0.5),_MainTex;
                #else
                o.uv1 = TRANSFORM_TEX(v.texcoord,_DiffuseMaskTex);
                #endif         
                o.uv1 += fixed2(_Mask_USpeed*_Time.y,_Mask_VSpeed*_Time.y);
                #endif

                #if _DISTORTION_ON
                o.uv2 = TRANSFORM_TEX(v.texcoord,_DistortionTex);
                o.uv2 += fixed2(_Distortion_USpeed*_Time.y,_Distortion_VSpeed*_Time.y);
                #endif


                #if _DISSOLUTION_ON
                o.uv3 = TRANSFORM_TEX(v.texcoord,_DissolutionTex);
                #endif

                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                #if _DISTORTION_ON
                fixed4 Distortion = tex2D(_DistortionTex,i.uv2);
                i.texcoord += Distortion.r*_DistortionIntensity*Distortion.a;
                #endif

                fixed4 col = tex2D(_MainTex, i.texcoord);

                #if _DIFFUSEMASK_ON
                fixed4 DiffuseMask = tex2D(_DiffuseMaskTex,i.uv1);
                col.a = col.a * DiffuseMask.a * DiffuseMask.r ;
                #endif

                col.rgb = col.rgb * _Color.rgb;
                fixed value = saturate(col.a);
                float lerpdegree = saturate(1-value- _LerpValue);
                col.rgb = lerp(col.rgb,_LerpColor.rgb,lerpdegree);
    

                #if _DISSOLUTION_ON
                fixed4 Dissolution = tex2D(_DissolutionTex, i.uv3);
                fixed dissolve = (Dissolution.r + Dissolution.a - 2*_Dissolvability)*0.5;
                clip(dissolve);
                float degree = saturate(dissolve/_EdgeWidth);
                col.rgb = lerp(_EdgeColor.rgb,col.rgb,degree);
                #endif
                col.rgb = col.rgb * i.color;    
                col.a = clamp((col.a * i.color.a * _AlphaValue),0,1) ;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    CustomEditor "FxStandardGUI"
}
