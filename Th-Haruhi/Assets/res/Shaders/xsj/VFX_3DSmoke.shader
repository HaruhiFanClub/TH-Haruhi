Shader "XSJ/VFX/3DSmoke" {
    Properties {
        [NoScaleOffset]_Tex ("SmokeTex", 2D) = "white" {}
        _N ("烟雾半径",Range(0,1)) = 0.9
        [HDR]_ColorA ("亮部颜色", Color) = (1,1,1,1)
        _ColorB ("暗部颜色", Color) = (0.2264151,0.2264151,0.2264151,1)
        _ColorC ("暗部反射", Color) = (0.4389808,0.08962262,1,1)
        _LightDir ("光源方向", Vector) = (-4.18,4.28,0)
        _Bright ("亮部范围",Range(0,0.8)) = 0
        _Black ("反射范围",Range(0,0.5)) = 0
        _Alpha ("Alpha",Range(0,10)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _Tex; 
            fixed4 _Tex_ST,_ColorA,_ColorB,_ColorC,_LightDir;
            fixed _Bright,_Black,_Alpha,_N;
            struct a2v {
                fixed4 vertex : POSITION;
                fixed3 normal : NORMAL;
                fixed4 texcoord0 : TEXCOORD0;
                fixed4 vColor : COLOR;
            };
            struct v2f {
                fixed4 pos : SV_POSITION;
                fixed4 uv0 : TEXCOORD0;
                fixed4 normalDir: TEXCOORD1;
                fixed4 vColor : COLOR;
            };
            v2f vert (a2v v) {
                v2f o = (v2f)0;
                o.uv0.xy = clamp((v.texcoord0 - 0.5)/_N + 0.5,0,1);
                o.vColor = v.vColor;
                o.normalDir.xyz = normalize(UnityObjectToWorldNormal(v.normal));
                o.pos = UnityObjectToClipPos( v.vertex );
                fixed3 lightDir = normalize(_LightDir.rgb);
                fixed ligh = dot(o.normalDir.xyz,lightDir);//光照计算，
                fixed a = max(0,ligh - _Bright) / (1 - _Bright);//亮部范围
                fixed b = max(0,-ligh - _Black) / (1 - _Black);//暗部反射颜色范围
                fixed3 c = lerp(_ColorB.rgb,(o.vColor.rgb*_ColorA.rgb),a) + b * _ColorC.rgb;
                o.uv0.zw = c.xy;
                o.normalDir.w = c.z;
                return o;
            }
            fixed4 frag(v2f i) : COLOR {
                
                fixed4 tex = tex2D(_Tex,TRANSFORM_TEX(i.uv0.xy, _Tex));
                fixed3 outCol = fixed3(i.uv0.zw,i.normalDir.w);
                fixed3 finalColor = outCol * tex.rgb;
                fixed alpha = min(1,tex.a * i.vColor.a * _Alpha);
                fixed4 finalRGBA = fixed4(finalColor,alpha);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
