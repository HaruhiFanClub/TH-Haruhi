    Shader "XSJ/Map/Diffuse" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex", 2D) = "white" {}
    }
     
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 400
       
    CGPROGRAM
    #pragma surface surf Lambert
    #pragma target 3.0

    #include "Lighting.cginc"
	#include "AutoLight.cginc"
	#include "UnityCG.cginc"
    
    sampler2D _MainTex;
    fixed4 _Color;
     
    struct Input {
        float2 uv_MainTex;
    };
     
    void surf (Input IN, inout SurfaceOutput o) {
        float4 tex = tex2D(_MainTex, IN.uv_MainTex);
        float4 col;
        o.Albedo = tex.rgb * _Color.rgb;
        o.Alpha = tex.a * _Color.a;
    }
    ENDCG
     
    }
     
    FallBack "Mobile/VertexLit"
    }