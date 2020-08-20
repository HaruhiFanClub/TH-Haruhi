// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Enviro/Mobile/Clouds" {
    Properties {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _ShadingColor ("Shading Color", Color) = (0.5,0.5,0.5,1)
        _Ambient ("Light Color", Color) = (0,0,0,0)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _CloudCover ("Cloud Cover", Range(-2,2)) = 0.5
        _CloudSharpness ("Cloud Sharpness", Range(-0.1,0.1)) = 0
        _CloudDensity ("Density", Range(-2,2)) = 1
        _CloudSpeed ("Cloud Speed", Vector) = (0.001, 0, 0, 0)
        _CloudScale ("Scale", Range(0,2)) = 1
        _horizonBlend ("Horizon Blend", Range(0,25)) = 1
        _timeV ("Time", Range(0,.2)) = 1
    }
    SubShader {
    Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

    Pass 
    {
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Front
    ZWrite On

    CGPROGRAM
    #pragma target 2.0
    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_fog
    #include "UnityCG.cginc"
 
    sampler2D _MainTex;
    float4 _BaseColor;
    float4 _ShadingColor;
    float4 _Ambient;
    float4 _SunDir;
    float4 _CloudSpeed;
    float _CloudCover;
    float _CloudDensity;
    float _CloudSharpness;
    float _horizonBlend;
    float _CloudScale;
    float _timeV;

       struct VertexInput 
       {
         float4 vertex : POSITION;
       };



    struct v2f {
        float4 pos : SV_POSITION;
        float4 tex : TEXCOORD0;
        float4 WorldPosition : TEXCOORD2;
        UNITY_FOG_COORDS(1)
    };
 
 
    v2f vert (VertexInput v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos (v.vertex);
        float3 vertnorm = normalize(v.vertex.xyz);
	    float2 vertuv   = vertnorm.xz / pow(vertnorm.y + _CloudScale, 2);
	    float  vertfade = saturate(10 * vertnorm.y * vertnorm.y);
        o.tex = float4(vertuv.xy, 0, vertfade);
        o.WorldPosition = normalize(mul((float4x4)unity_ObjectToWorld, v.vertex)).xyzw;
        UNITY_TRANSFER_FOG(o,o.pos);
        return o;
    }
 
 
    float4 frag (v2f i) : COLOR
    {
        float2 offset = _Time.y * _CloudSpeed.xy;
        float2 offset2 = _Time.y * _CloudSpeed.xy * 0.8;

        fixed2 UV = i.tex.xy + offset;
        fixed2 UV2 = i.tex.xy + offset2;
        fixed4 cloudTexture = tex2D(_MainTex, UV);
        fixed4 cloudTexture2 = tex2D(_MainTex, UV2); 

        float3 viewDir = normalize(i.WorldPosition);
        fixed baseMorph = ((saturate(cloudTexture.a + _CloudCover)) - cloudTexture2.a) * (i.tex.w * _horizonBlend);

        baseMorph = (1.0 - pow(1-_CloudSharpness, baseMorph * 255));
        float Light = 1 / exp(baseMorph);

        float4 res = float4 (cloudTexture * lerp(_BaseColor.rgb,_ShadingColor.rgb,(_CloudDensity-Light)), baseMorph);
        res.xyz *= _Ambient.xyz - _timeV;

       fixed4 fogC = res; 
       UNITY_APPLY_FOG(i.fogCoord, fogC);
       fogC = lerp(fogC,res, saturate(dot(viewDir.y, float3(0,1,0))));
       return fogC;
    }
    ENDCG
    }
    }
}