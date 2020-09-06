// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:34479,y:32282,varname:node_4795,prsc:2|emission-6545-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31696,y:32451,ptovrint:False,ptlb:noise01,ptin:_noise01,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5265ef67b9671c044b2b17aa22626ce4,ntxv:0,isnm:False|UVIN-4600-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:33708,y:32566,varname:node_2393,prsc:2|A-9248-OUT,B-2053-RGB,C-797-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:33389,y:32438,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:33035,y:32550,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:33137,y:32398,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_TexCoord,id:8373,x:32541,y:33645,varname:node_8373,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Tex2d,id:5130,x:33172,y:31978,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_5130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:375c0ac13646ada4980f177725760320,ntxv:0,isnm:False|UVIN-6354-OUT;n:type:ShaderForge.SFN_Multiply,id:6390,x:33925,y:32049,varname:node_6390,prsc:2|A-8617-OUT,B-5130-A,C-797-A,D-2053-A,E-4520-OUT;n:type:ShaderForge.SFN_Multiply,id:7110,x:30996,y:32335,varname:node_7110,prsc:2|A-7573-OUT,B-8986-T;n:type:ShaderForge.SFN_ValueProperty,id:7573,x:30718,y:32301,ptovrint:False,ptlb:noise01_u,ptin:_noise01_u,varname:node_7573,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3754,x:30718,y:32571,ptovrint:False,ptlb:noise01_v,ptin:_noise01_v,varname:node_3754,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4524,x:30996,y:32489,varname:node_4524,prsc:2|A-8986-T,B-3754-OUT;n:type:ShaderForge.SFN_Time,id:8986,x:30718,y:32392,varname:node_8986,prsc:2;n:type:ShaderForge.SFN_Append,id:9597,x:31194,y:32547,varname:node_9597,prsc:2|A-7110-OUT,B-4524-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:4520,x:33280,y:33117,ptovrint:False,ptlb:dissolve_on,ptin:_dissolve_on,varname:node_4520,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3242-OUT,B-5208-OUT;n:type:ShaderForge.SFN_Vector1,id:3616,x:33064,y:32867,varname:node_3616,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:6126,x:31752,y:32693,ptovrint:False,ptlb:noise02,ptin:_noise02,varname:node_6126,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7b72a298fabd63740821b5e5a0a60d74,ntxv:0,isnm:False|UVIN-6469-OUT;n:type:ShaderForge.SFN_Multiply,id:6545,x:34029,y:32392,varname:node_6545,prsc:2|A-2393-OUT,B-6390-OUT,C-769-RGB,D-769-A;n:type:ShaderForge.SFN_Multiply,id:4218,x:30972,y:32741,varname:node_4218,prsc:2|A-8244-OUT,B-4475-T;n:type:ShaderForge.SFN_ValueProperty,id:8244,x:30705,y:32760,ptovrint:False,ptlb:noise02_u,ptin:_noise02_u,varname:_u_speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8518,x:30705,y:33021,ptovrint:False,ptlb:noise02_v,ptin:_noise02_v,varname:_V_speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1713,x:30972,y:32907,varname:node_1713,prsc:2|A-4475-T,B-8518-OUT;n:type:ShaderForge.SFN_Time,id:4475,x:30705,y:32840,varname:node_4475,prsc:2;n:type:ShaderForge.SFN_Append,id:151,x:31213,y:32820,varname:node_151,prsc:2|A-4218-OUT,B-1713-OUT;n:type:ShaderForge.SFN_TexCoord,id:3311,x:30723,y:31994,varname:node_3311,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4600,x:31505,y:32400,varname:node_4600,prsc:2|A-3311-UVOUT,B-9597-OUT;n:type:ShaderForge.SFN_Add,id:6469,x:31470,y:32649,varname:node_6469,prsc:2|A-3311-UVOUT,B-151-OUT;n:type:ShaderForge.SFN_Add,id:6408,x:32645,y:32019,varname:node_6408,prsc:2|A-3311-UVOUT,B-7961-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4457,x:32371,y:32415,ptovrint:False,ptlb:main_u,ptin:_main_u,varname:node_4457,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:13,x:32346,y:32649,ptovrint:False,ptlb:main_v,ptin:_main_v,varname:node_13,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:5913,x:32801,y:32527,varname:node_5913,prsc:2|A-2900-OUT,B-2891-OUT;n:type:ShaderForge.SFN_Multiply,id:2891,x:32594,y:32581,varname:node_2891,prsc:2|A-6893-T,B-13-OUT;n:type:ShaderForge.SFN_Time,id:6893,x:32298,y:32504,varname:node_6893,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2900,x:32579,y:32434,varname:node_2900,prsc:2|A-4457-OUT,B-6893-T;n:type:ShaderForge.SFN_Add,id:6354,x:32919,y:32169,varname:node_6354,prsc:2|A-6408-OUT,B-7551-OUT;n:type:ShaderForge.SFN_Tex2d,id:769,x:34034,y:32975,ptovrint:False,ptlb:alpha,ptin:_alpha,varname:node_769,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6408-OUT;n:type:ShaderForge.SFN_Append,id:3176,x:32004,y:32326,varname:node_3176,prsc:2|A-6074-R,B-6126-R;n:type:ShaderForge.SFN_Multiply,id:7961,x:32347,y:32220,varname:node_7961,prsc:2|A-3176-OUT,B-1323-OUT,C-5837-OUT;n:type:ShaderForge.SFN_Slider,id:1323,x:31892,y:32767,ptovrint:False,ptlb:noise_qiangdu,ptin:_noise_qiangdu,varname:node_1323,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Add,id:4860,x:31483,y:32098,varname:node_4860,prsc:2|A-3311-UVOUT,B-5521-OUT;n:type:ShaderForge.SFN_Vector1,id:5521,x:31254,y:32192,varname:node_5521,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:5837,x:31807,y:32131,varname:node_5837,prsc:2|A-4860-OUT,B-7344-OUT;n:type:ShaderForge.SFN_Vector1,id:7344,x:31611,y:32293,varname:node_7344,prsc:2,v1:2;n:type:ShaderForge.SFN_Desaturate,id:8617,x:33813,y:31783,varname:node_8617,prsc:2|COL-5130-RGB,DES-9602-OUT;n:type:ShaderForge.SFN_Slider,id:9602,x:33469,y:31985,ptovrint:False,ptlb:Desaturate,ptin:_Desaturate,varname:node_9602,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Smoothstep,id:1766,x:33525,y:33492,varname:node_1766,prsc:2|A-2628-OUT,B-7812-OUT,V-8373-U;n:type:ShaderForge.SFN_Vector1,id:3242,x:33001,y:32977,varname:node_3242,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:915,x:32926,y:33469,varname:node_915,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:6280,x:33720,y:33492,varname:node_6280,prsc:2|IN-1766-OUT;n:type:ShaderForge.SFN_Clamp01,id:5208,x:33965,y:33525,varname:node_5208,prsc:2|IN-6280-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7812,x:32823,y:33385,ptovrint:False,ptlb:node_7812,ptin:_node_7812,varname:node_7812,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:7551,x:32968,y:32701,ptovrint:False,ptlb:uv_on,ptin:_uv_on,varname:node_7551,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5913-OUT,B-1623-OUT;n:type:ShaderForge.SFN_TexCoord,id:8386,x:32504,y:32798,varname:node_8386,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Append,id:1623,x:32731,y:32921,varname:node_1623,prsc:2|A-8386-Z,B-8386-W;n:type:ShaderForge.SFN_Tex2d,id:1228,x:33579,y:33095,ptovrint:False,ptlb:dissolveTEX,ptin:_dissolveTEX,varname:node_1228,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:2628,x:33949,y:33171,ptovrint:False,ptlb:node_2628,ptin:_node_2628,varname:node_2628,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5130-R,B-1228-R;proporder:6074-797-5130-7573-3754-4520-6126-8244-8518-4457-13-769-1323-9602-7812-7551-1228-2628;pass:END;sub:END;*/

Shader "Fps/Effects/noise_add02" {
    Properties {
        _noise01 ("noise01", 2D) = "white" {}
        [HDR]_TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _noise01_u ("noise01_u", Float ) = 0
        _noise01_v ("noise01_v", Float ) = 0
        [MaterialToggle] _dissolve_on ("dissolve_on", Float ) = 1
        _noise02 ("noise02", 2D) = "white" {}
        _noise02_u ("noise02_u", Float ) = 0
        _noise02_v ("noise02_v", Float ) = 0
        _main_u ("main_u", Float ) = 0
        _main_v ("main_v", Float ) = 0
        _alpha ("alpha", 2D) = "white" {}
        _noise_qiangdu ("noise_qiangdu", Range(0, 1)) = 0.1
        _Desaturate ("Desaturate", Range(0, 1)) = 0
        _node_7812 ("node_7812", Float ) = 1
        [MaterialToggle] _uv_on ("uv_on", Float ) = 0
        _dissolveTEX ("dissolveTEX", 2D) = "white" {}
        [MaterialToggle] _node_2628 ("node_2628", Float ) = 0
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
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _noise01; uniform float4 _noise01_ST;
            uniform float4 _TintColor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _noise01_u;
            uniform float _noise01_v;
            uniform fixed _dissolve_on;
            uniform sampler2D _noise02; uniform float4 _noise02_ST;
            uniform float _noise02_u;
            uniform float _noise02_v;
            uniform float _main_u;
            uniform float _main_v;
            uniform sampler2D _alpha; uniform float4 _alpha_ST;
            uniform float _noise_qiangdu;
            uniform float _Desaturate;
            uniform float _node_7812;
            uniform fixed _uv_on;
            uniform sampler2D _dissolveTEX; uniform float4 _dissolveTEX_ST;
            uniform fixed _node_2628;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_8986 = _Time;
                float2 node_4600 = (i.uv0+float2((_noise01_u*node_8986.g),(node_8986.g*_noise01_v)));
                float4 _noise01_var = tex2D(_noise01,TRANSFORM_TEX(node_4600, _noise01));
                float4 node_4475 = _Time;
                float2 node_6469 = (i.uv0+float2((_noise02_u*node_4475.g),(node_4475.g*_noise02_v)));
                float4 _noise02_var = tex2D(_noise02,TRANSFORM_TEX(node_6469, _noise02));
                float2 node_6408 = (i.uv0+(float2(_noise01_var.r,_noise02_var.r)*_noise_qiangdu*((i.uv0+(-0.5))*2.0)));
                float4 node_6893 = _Time;
                float2 node_6354 = (node_6408+lerp( float2((_main_u*node_6893.g),(node_6893.g*_main_v)), float2(i.uv1.b,i.uv1.a), _uv_on ));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_6354, _MainTex));
                float4 _dissolveTEX_var = tex2D(_dissolveTEX,TRANSFORM_TEX(i.uv0, _dissolveTEX));
                float4 _alpha_var = tex2D(_alpha,TRANSFORM_TEX(node_6408, _alpha));
                float3 emissive = ((2.0*i.vertexColor.rgb*_TintColor.rgb)*(lerp(_MainTex_var.rgb,dot(_MainTex_var.rgb,float3(0.3,0.59,0.11)),_Desaturate)*_MainTex_var.a*_TintColor.a*i.vertexColor.a*lerp( 1.0, saturate((1.0 - smoothstep( lerp( _MainTex_var.r, _dissolveTEX_var.r, _node_2628 ), _node_7812, i.uv1.r ))), _dissolve_on ))*_alpha_var.rgb*_alpha_var.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
