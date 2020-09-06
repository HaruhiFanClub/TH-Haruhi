// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:35268,y:32105,varname:node_4795,prsc:2|emission-6545-OUT,voffset-6493-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31993,y:32342,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4600-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:33754,y:32188,varname:node_2393,prsc:2|A-9248-OUT,B-2053-RGB,C-797-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:33371,y:32576,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:33124,y:32543,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:33137,y:32398,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:9781,x:32445,y:33205,ptovrint:False,ptlb:DissolveTex,ptin:_DissolveTex,varname:node_9781,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Step,id:2325,x:32759,y:33493,varname:node_2325,prsc:2|A-9781-R,B-8373-U;n:type:ShaderForge.SFN_TexCoord,id:8373,x:32333,y:33531,varname:node_8373,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Tex2d,id:5130,x:34057,y:32465,ptovrint:False,ptlb:MaskTex,ptin:_MaskTex,varname:node_5130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6354-OUT;n:type:ShaderForge.SFN_Multiply,id:7110,x:30996,y:32335,varname:node_7110,prsc:2|A-7573-OUT,B-8986-T;n:type:ShaderForge.SFN_ValueProperty,id:7573,x:30718,y:32301,ptovrint:False,ptlb:u_speed,ptin:_u_speed,varname:node_7573,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3754,x:30718,y:32571,ptovrint:False,ptlb:V_speed,ptin:_V_speed,varname:node_3754,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4524,x:30996,y:32489,varname:node_4524,prsc:2|A-8986-T,B-3754-OUT;n:type:ShaderForge.SFN_Time,id:8986,x:30718,y:32392,varname:node_8986,prsc:2;n:type:ShaderForge.SFN_Append,id:9597,x:31200,y:32451,varname:node_9597,prsc:2|A-7110-OUT,B-4524-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:4520,x:33682,y:32878,ptovrint:False,ptlb:dissolve_on,ptin:_dissolve_on,varname:node_4520,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3616-OUT,B-2325-OUT;n:type:ShaderForge.SFN_Vector1,id:3616,x:32958,y:32637,varname:node_3616,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:8222,x:33046,y:33722,varname:node_8222,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Tex2d,id:6126,x:32015,y:32678,ptovrint:False,ptlb:noisetex2,ptin:_noisetex2,varname:node_6126,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6469-OUT;n:type:ShaderForge.SFN_Multiply,id:6545,x:34758,y:31939,varname:node_6545,prsc:2|A-2393-OUT,B-5130-R,C-769-RGB,D-2230-OUT,E-8084-OUT;n:type:ShaderForge.SFN_Multiply,id:4218,x:30972,y:32741,varname:node_4218,prsc:2|A-8244-OUT,B-4475-T;n:type:ShaderForge.SFN_ValueProperty,id:8244,x:30705,y:32760,ptovrint:False,ptlb:tex2u_speed,ptin:_tex2u_speed,varname:_u_speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:8518,x:30705,y:33021,ptovrint:False,ptlb:tex2V_speed,ptin:_tex2V_speed,varname:_V_speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1713,x:30972,y:32907,varname:node_1713,prsc:2|A-4475-T,B-8518-OUT;n:type:ShaderForge.SFN_Time,id:4475,x:30705,y:32840,varname:node_4475,prsc:2;n:type:ShaderForge.SFN_Append,id:151,x:31213,y:32820,varname:node_151,prsc:2|A-4218-OUT,B-1713-OUT;n:type:ShaderForge.SFN_TexCoord,id:3311,x:30767,y:31992,varname:node_3311,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4600,x:31650,y:32263,varname:node_4600,prsc:2|A-3311-UVOUT,B-9597-OUT;n:type:ShaderForge.SFN_Add,id:6469,x:31684,y:32711,varname:node_6469,prsc:2|A-3311-UVOUT,B-151-OUT;n:type:ShaderForge.SFN_Add,id:1721,x:31547,y:31925,varname:node_1721,prsc:2|A-3311-UVOUT,B-4430-OUT;n:type:ShaderForge.SFN_Vector1,id:4430,x:31352,y:32123,varname:node_4430,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:2274,x:32309,y:32184,varname:node_2274,prsc:2|A-1721-OUT,B-2574-OUT,C-5686-OUT,D-6074-R,E-6126-R;n:type:ShaderForge.SFN_ValueProperty,id:2574,x:31740,y:32113,ptovrint:False,ptlb:qiangdu,ptin:_qiangdu,varname:node_2574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Vector1,id:5686,x:31778,y:32197,varname:node_5686,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:6408,x:32614,y:32130,varname:node_6408,prsc:2|A-3311-UVOUT,B-2274-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4457,x:32322,y:32366,ptovrint:False,ptlb:masku,ptin:_masku,varname:node_4457,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:13,x:32322,y:32636,ptovrint:False,ptlb:maskv,ptin:_maskv,varname:node_13,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:5913,x:32774,y:32397,varname:node_5913,prsc:2|A-2900-OUT,B-2891-OUT;n:type:ShaderForge.SFN_Multiply,id:2891,x:32594,y:32581,varname:node_2891,prsc:2|A-6893-T,B-13-OUT;n:type:ShaderForge.SFN_Time,id:6893,x:32322,y:32460,varname:node_6893,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2900,x:32560,y:32351,varname:node_2900,prsc:2|A-4457-OUT,B-6893-T;n:type:ShaderForge.SFN_Add,id:6354,x:32937,y:32225,varname:node_6354,prsc:2|A-6408-OUT,B-5913-OUT;n:type:ShaderForge.SFN_Tex2d,id:769,x:33624,y:32475,ptovrint:False,ptlb:alpha,ptin:_alpha,varname:node_769,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:8084,x:34401,y:32533,varname:node_8084,prsc:2|A-2053-A,B-769-A,C-5130-A,D-4520-OUT;n:type:ShaderForge.SFN_Fresnel,id:8789,x:34383,y:32160,varname:node_8789,prsc:2|EXP-9513-OUT;n:type:ShaderForge.SFN_Multiply,id:2230,x:34722,y:32223,varname:node_2230,prsc:2|A-8789-OUT,B-9410-OUT;n:type:ShaderForge.SFN_Slider,id:9410,x:34305,y:32405,ptovrint:False,ptlb:f_,ptin:_f_,varname:node_9410,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5811971,max:1;n:type:ShaderForge.SFN_Slider,id:9513,x:34098,y:32299,ptovrint:False,ptlb:f_v,ptin:_f_v,varname:node_9513,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Multiply,id:6493,x:35299,y:32744,varname:node_6493,prsc:2|A-3053-R,B-3984-OUT,C-2852-OUT;n:type:ShaderForge.SFN_NormalVector,id:3984,x:34182,y:32977,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2852,x:34182,y:33163,ptovrint:False,ptlb:node_2852,ptin:_node_2852,varname:node_2852,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:3053,x:34649,y:32682,ptovrint:False,ptlb:vertexTEX,ptin:_vertexTEX,varname:node_3053,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2171-OUT;n:type:ShaderForge.SFN_Multiply,id:8127,x:34057,y:33532,varname:node_8127,prsc:2|A-3211-OUT,B-1264-T;n:type:ShaderForge.SFN_Multiply,id:6274,x:34057,y:33686,varname:node_6274,prsc:2|A-1264-T,B-9178-OUT;n:type:ShaderForge.SFN_Time,id:1264,x:33779,y:33589,varname:node_1264,prsc:2;n:type:ShaderForge.SFN_Append,id:3626,x:34261,y:33648,varname:node_3626,prsc:2|A-8127-OUT,B-6274-OUT;n:type:ShaderForge.SFN_TexCoord,id:1930,x:33779,y:33231,varname:node_1930,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2171,x:34781,y:33477,varname:node_2171,prsc:2|A-1930-UVOUT,B-3626-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3211,x:33779,y:33471,ptovrint:False,ptlb:vertex_u,ptin:_vertex_u,varname:node_3211,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:9178,x:33876,y:33845,ptovrint:False,ptlb:vertex_v,ptin:_vertex_v,varname:node_9178,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:6074-797-9781-5130-7573-3754-4520-6126-8244-8518-2574-4457-13-769-9410-9513-2852-3053-3211-9178;pass:END;sub:END;*/

Shader "Fps/Effects/noise_add_f" {
    Properties {
        _noiseTex ("noiseTex", 2D) = "white" {}
        [HDR]_TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _DissolveTex ("DissolveTex", 2D) = "white" {}
        _MaskTex ("MaskTex", 2D) = "white" {}
        _u_speed ("u_speed", Float ) = 0
        _V_speed ("V_speed", Float ) = 0
        [MaterialToggle] _dissolve_on ("dissolve_on", Float ) = 1
        _noisetex2 ("noisetex2", 2D) = "white" {}
        _tex2u_speed ("tex2u_speed", Float ) = 0
        _tex2V_speed ("tex2V_speed", Float ) = 0
        _qiangdu ("qiangdu", Float ) = 0
        _masku ("masku", Float ) = 0
        _maskv ("maskv", Float ) = 0
        _alpha ("alpha", 2D) = "white" {}
        _f_ ("f_", Range(0, 1)) = 0.5811971
        _f_v ("f_v", Range(0, 5)) = 1
        _node_2852 ("node_2852", Range(0, 1)) = 0
        _vertexTEX ("vertexTEX", 2D) = "white" {}
        _vertex_u ("vertex_u", Float ) = 0
        _vertex_v ("vertex_v", Float ) = 0
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
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _DissolveTex; uniform float4 _DissolveTex_ST;
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
            uniform float _u_speed;
            uniform float _V_speed;
            uniform fixed _dissolve_on;
            uniform sampler2D _noisetex2; uniform float4 _noisetex2_ST;
            uniform float _tex2u_speed;
            uniform float _tex2V_speed;
            uniform float _qiangdu;
            uniform float _masku;
            uniform float _maskv;
            uniform sampler2D _alpha; uniform float4 _alpha_ST;
            uniform float _f_;
            uniform float _f_v;
            uniform float _node_2852;
            uniform sampler2D _vertexTEX; uniform float4 _vertexTEX_ST;
            uniform float _vertex_u;
            uniform float _vertex_v;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_1264 = _Time;
                float2 node_2171 = (o.uv0+float2((_vertex_u*node_1264.g),(node_1264.g*_vertex_v)));
                float4 _vertexTEX_var = tex2Dlod(_vertexTEX,float4(TRANSFORM_TEX(node_2171, _vertexTEX),0.0,0));
                v.vertex.xyz += (_vertexTEX_var.r*v.normal*_node_2852);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_8986 = _Time;
                float2 node_4600 = (i.uv0+float2((_u_speed*node_8986.g),(node_8986.g*_V_speed)));
                float4 _noiseTex_var = tex2D(_noiseTex,TRANSFORM_TEX(node_4600, _noiseTex));
                float4 node_4475 = _Time;
                float2 node_6469 = (i.uv0+float2((_tex2u_speed*node_4475.g),(node_4475.g*_tex2V_speed)));
                float4 _noisetex2_var = tex2D(_noisetex2,TRANSFORM_TEX(node_6469, _noisetex2));
                float4 node_6893 = _Time;
                float2 node_6354 = ((i.uv0+((i.uv0+(-0.5))*_qiangdu*2.0*_noiseTex_var.r*_noisetex2_var.r))+float2((_masku*node_6893.g),(node_6893.g*_maskv)));
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(node_6354, _MaskTex));
                float4 _alpha_var = tex2D(_alpha,TRANSFORM_TEX(i.uv0, _alpha));
                float node_2230 = (pow(1.0-max(0,dot(normalDirection, viewDirection)),_f_v)*_f_);
                float4 _DissolveTex_var = tex2D(_DissolveTex,TRANSFORM_TEX(i.uv0, _DissolveTex));
                float3 node_6545 = ((2.0*i.vertexColor.rgb*_TintColor.rgb)*_MaskTex_var.r*_alpha_var.rgb*node_2230*(i.vertexColor.a*_alpha_var.a*_MaskTex_var.a*lerp( 1.0, step(_DissolveTex_var.r,i.uv1.r), _dissolve_on )));
                float3 emissive = node_6545;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
