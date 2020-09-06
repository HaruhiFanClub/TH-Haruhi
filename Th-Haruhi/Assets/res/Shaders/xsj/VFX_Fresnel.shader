Shader "XSJ/VFX/FxFresnel"
{
    Properties
    {
        [HDR]_BaseColor ("基础颜色", Color) =  (1,1,1,1)
        _MainTex ("基础贴图", 2D) = "white" {}
        [Toggle]_UseUV2("使用UV2",Float) = 1
        _AlphaValue ("透明值",Range(0,1)) = 1
        [HDR]_RimColor ("边缘光颜色", Color) = (1,1,1,1)
        _RimPower ("边缘光强度", Range(0,10)) = 1
        _RimMask ("边缘光遮罩", 2D) = "white" {}

        [NoKeywordToggle] _Normal ("法线",Float)=0
        [Foldout] _NormalLayerShown ("", Float) = 1 
        _NormalTex ("法线贴图", 2D) = "white" {}


        [HideInInspector] _BlendMode ("__mode",Float) = 0.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _FaceMode ("__face", Float) = 0.0
        [HideInInspector] _CullMode ("__cull", Float) = 2.0
        [HideInInspector] _ZWriteMode ("__face", Float) = 0.0
        [HideInInspector] _ZWriteKey ("__zwrite", Float) = 1.0
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True" 
        }
        Pass{
            Zwrite [_ZWriteKey]
            ColorMask 0
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
            #pragma multi_compile _ _NORMAL_ON
            #pragma multi_compile _ _USEUV2_ON

            uniform fixed4 _BaseColor;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed _AlphaValue;
            uniform fixed4 _RimColor;
            uniform half _RimPower;
            uniform sampler2D _RimMask;
            #ifdef _NORMAL_ON
            uniform sampler2D _NormalTex;
            uniform half4 _NormalTex_ST;
            #endif

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                fixed4 color : COLOR;
                float2 uv0 : TEXCOORD2;
                #if _NORMAL_ON
                float2 uv : TEXCOORD3;
                float4 TW0:TEXCOORD4;
                float4 TW1:TEXCOORD5;
                float4 TW2:TEXCOORD6;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_full v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                #if _USEUV2_ON
                o.uv0 = TRANSFORM_TEX(v.texcoord1,_MainTex);
                #else
                o.uv0 = TRANSFORM_TEX(v.texcoord,_MainTex);
                #endif
                
                #if _NORMAL_ON
                o.uv = TRANSFORM_TEX(v.texcoord,_NormalTex);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                fixed3 worldBinormal = cross(o.worldNormal, worldTangent) * tangentSign;

                o.TW0 = float4(worldTangent.x, worldBinormal.x, o.worldNormal.x, o.worldPos.x);
                o.TW1 = float4(worldTangent.y, worldBinormal.y, o.worldNormal.y, o.worldPos.y);
                o.TW2 = float4(worldTangent.z, worldBinormal.z, o.worldNormal.z, o.worldPos.z);
                #endif
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 maincolor = tex2D(_MainTex, i.uv0);
                fixed4 maskvalue = tex2D(_RimMask, i.uv0);
                fixed4 col = _BaseColor * maincolor;
                float3 worldNormal = normalize(i.worldNormal);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                #if _NORMAL_ON
                worldNormal = UnpackNormal(tex2D(_NormalTex, i.uv));
                worldNormal = normalize(fixed3(dot(i.TW0.xyz, worldNormal), dot(i.TW1.xyz, worldNormal), dot(i.TW2.xyz, worldNormal)));
                #endif
                float NdotV = saturate(dot(worldNormal,viewDir));

                fixed3 rim = pow(1-saturate(NdotV),_RimPower)*_RimColor.rgb;
                rim.rgb = rim.rgb*maskvalue.rgb;
                col.rgb += rim;
                col.rgb *= i.color.rgb;
                col.a = _AlphaValue * i.color.a;
                return col;
            }
            ENDCG
        }
    }
    CustomEditor "FxFresnelGUI"
}
