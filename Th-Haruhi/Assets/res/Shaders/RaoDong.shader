// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:34105,y:33015,varname:node_9361,prsc:2|emission-1560-OUT;n:type:ShaderForge.SFN_Fresnel,id:1469,x:33210,y:32699,varname:node_1469,prsc:2|EXP-9847-OUT;n:type:ShaderForge.SFN_Multiply,id:4050,x:33534,y:32860,varname:node_4050,prsc:2|A-7243-OUT,B-4487-OUT,C-5553-RGB,D-1469-OUT;n:type:ShaderForge.SFN_Slider,id:9847,x:32482,y:32776,ptovrint:False,ptlb:fresnel_v,ptin:_fresnel_v,varname:node_9847,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.076922,max:5;n:type:ShaderForge.SFN_Color,id:5553,x:32649,y:33113,ptovrint:False,ptlb:maincolor,ptin:_maincolor,varname:node_5553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:4487,x:33220,y:32805,ptovrint:False,ptlb:main_v,ptin:_main_v,varname:node_4487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:5249,x:33012,y:33544,ptovrint:False,ptlb:tex1,ptin:_tex1,varname:_maintex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9966-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:9213,x:31574,y:33122,varname:node_9213,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:1560,x:33861,y:33285,varname:node_1560,prsc:2|A-4050-OUT,B-6630-OUT,C-4580-OUT;n:type:ShaderForge.SFN_Multiply,id:6630,x:33486,y:33693,varname:node_6630,prsc:2|A-5249-RGB,B-3392-OUT,C-3756-RGB,D-7259-OUT,E-7243-OUT;n:type:ShaderForge.SFN_Tex2d,id:493,x:32676,y:33696,ptovrint:False,ptlb:tex2,ptin:_tex2,varname:node_493,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:3756,x:33186,y:33965,ptovrint:False,ptlb:color2,ptin:_color2,varname:node_3756,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7259,x:33224,y:34125,ptovrint:False,ptlb:color_v2,ptin:_color_v2,varname:node_7259,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:3392,x:33061,y:33777,varname:node_3392,prsc:2|A-493-RGB,B-7118-OUT,C-3877-OUT;n:type:ShaderForge.SFN_Fresnel,id:3877,x:32713,y:33910,varname:node_3877,prsc:2|EXP-8576-OUT;n:type:ShaderForge.SFN_Slider,id:8576,x:32298,y:33954,ptovrint:False,ptlb:f2,ptin:_f2,varname:node_8576,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.446844,max:5;n:type:ShaderForge.SFN_Multiply,id:4580,x:33314,y:33456,varname:node_4580,prsc:2|A-5249-RGB,B-3756-RGB,C-2020-OUT,D-7243-OUT;n:type:ShaderForge.SFN_Panner,id:9966,x:32651,y:33369,varname:node_9966,prsc:2,spu:1,spv:1|UVIN-9213-UVOUT,DIST-3719-OUT;n:type:ShaderForge.SFN_Multiply,id:3719,x:32005,y:34162,varname:node_3719,prsc:2|A-8159-TSL,B-2550-OUT;n:type:ShaderForge.SFN_Time,id:8159,x:31661,y:33789,varname:node_8159,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:2714,x:31515,y:34151,ptovrint:False,ptlb:u1,ptin:_u1,varname:_noise_u_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2822,x:31548,y:34310,ptovrint:False,ptlb:v1,ptin:_v1,varname:_noise_v_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Append,id:2550,x:31783,y:34242,varname:node_2550,prsc:2|A-2714-OUT,B-2822-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2020,x:32938,y:34135,ptovrint:False,ptlb:tex1_liangdu,ptin:_tex1_liangdu,varname:node_2020,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:7118,x:32656,y:34158,ptovrint:False,ptlb:tex2_v,ptin:_tex2_v,varname:node_7118,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_VertexColor,id:5118,x:32855,y:33201,varname:node_5118,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7243,x:33210,y:33139,varname:node_7243,prsc:2|A-5118-RGB,B-5118-A;proporder:9847-5553-4487-493-3756-7259-8576-2714-2822-2020-7118-5249;pass:END;sub:END;*/

Shader "X/Effects/RaoDong" {
    Properties {
        _fresnel_v ("fresnel_v", Range(0, 5)) = 3.076922
        _maincolor ("maincolor", Color) = (0.5,0.5,0.5,1)
        _main_v ("main_v", Float ) = 1
        _tex2 ("tex2", 2D) = "white" {}
        _color2 ("color2", Color) = (0.5,0.5,0.5,1)
        _color_v2 ("color_v2", Float ) = 2
        _f2 ("f2", Range(0, 5)) = 1.446844
        _u1 ("u1", Float ) = 1
        _v1 ("v1", Float ) = 1
        _tex1_liangdu ("tex1_liangdu", Float ) = 2
        _tex2_v ("tex2_v", Float ) = 2
        _tex1 ("tex1", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcColor
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float _fresnel_v;
            uniform float4 _maincolor;
            uniform float _main_v;
            uniform sampler2D _tex1; uniform float4 _tex1_ST;
            uniform sampler2D _tex2; uniform float4 _tex2_ST;
            uniform float4 _color2;
            uniform float _color_v2;
            uniform float _f2;
            uniform float _u1;
            uniform float _v1;
            uniform float _tex1_liangdu;
            uniform float _tex2_v;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 node_7243 = (i.vertexColor.rgb*i.vertexColor.a);
                float4 node_8159 = _Time;
                float2 node_9966 = (i.uv0+(node_8159.r*float2(_u1,_v1))*float2(1,1));
                float4 _tex1_var = tex2D(_tex1,TRANSFORM_TEX(node_9966, _tex1));
                float4 _tex2_var = tex2D(_tex2,TRANSFORM_TEX(i.uv0, _tex2));
                float3 emissive = ((node_7243*_main_v*_maincolor.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnel_v))+(_tex1_var.rgb*(_tex2_var.rgb*_tex2_v*pow(1.0-max(0,dot(normalDirection, viewDirection)),_f2))*_color2.rgb*_color_v2*node_7243)+(_tex1_var.rgb*_color2.rgb*_tex1_liangdu*node_7243));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
