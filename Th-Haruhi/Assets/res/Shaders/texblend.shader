// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:34073,y:32802,varname:node_9361,prsc:2|emission-9162-OUT;n:type:ShaderForge.SFN_Tex2d,id:6187,x:32806,y:32623,ptovrint:False,ptlb:maintex,ptin:_maintex,varname:node_6187,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1464-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4050,x:33243,y:32765,varname:node_4050,prsc:2|A-6187-RGB,B-6187-A,C-9529-RGB,D-9529-A;n:type:ShaderForge.SFN_Color,id:5553,x:33650,y:33370,ptovrint:False,ptlb:maincolor,ptin:_maincolor,varname:node_5553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:4487,x:33708,y:33491,ptovrint:False,ptlb:main_v,ptin:_main_v,varname:node_4487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:9316,x:32044,y:32645,varname:node_9316,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6396,x:32288,y:32828,varname:node_6396,prsc:2|A-9316-T,B-7861-OUT;n:type:ShaderForge.SFN_ValueProperty,id:974,x:31797,y:32827,ptovrint:False,ptlb:u,ptin:_u,varname:node_974,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:1380,x:31812,y:32541,varname:node_1380,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:1464,x:32582,y:32687,varname:node_1464,prsc:2,spu:1,spv:1|UVIN-1380-UVOUT,DIST-6396-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8689,x:31834,y:33020,ptovrint:False,ptlb:v,ptin:_v,varname:_node_974_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Append,id:7861,x:31989,y:32921,varname:node_7861,prsc:2|A-974-OUT,B-8689-OUT;n:type:ShaderForge.SFN_Tex2d,id:9529,x:32840,y:32972,ptovrint:False,ptlb:tex02,ptin:_tex02,varname:_maintex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9785-UVOUT;n:type:ShaderForge.SFN_Time,id:1562,x:32046,y:33254,varname:node_1562,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2924,x:32271,y:33388,varname:node_2924,prsc:2|A-1562-T,B-4563-OUT;n:type:ShaderForge.SFN_TexCoord,id:9585,x:31794,y:33152,varname:node_9585,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:9785,x:32462,y:33093,varname:node_9785,prsc:2,spu:1,spv:1|UVIN-9585-UVOUT,DIST-2924-OUT;n:type:ShaderForge.SFN_Append,id:4563,x:32061,y:33415,varname:node_4563,prsc:2|A-9020-OUT,B-3053-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9020,x:31806,y:33357,ptovrint:False,ptlb:u02,ptin:_u02,varname:node_9020,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3053,x:31806,y:33473,ptovrint:False,ptlb:v02,ptin:_v02,varname:node_3053,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:4399,x:32061,y:33806,varname:node_4399,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3038,x:32286,y:33940,varname:node_3038,prsc:2|A-4399-T,B-3328-OUT;n:type:ShaderForge.SFN_TexCoord,id:3616,x:31884,y:33675,varname:node_3616,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:5153,x:32477,y:33645,varname:node_5153,prsc:2,spu:1,spv:1|UVIN-3616-UVOUT,DIST-3038-OUT;n:type:ShaderForge.SFN_Append,id:3328,x:32076,y:33967,varname:node_3328,prsc:2|A-2872-OUT,B-1497-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2872,x:31821,y:33909,ptovrint:False,ptlb:u03,ptin:_u03,varname:_u03,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:1497,x:31821,y:34025,ptovrint:False,ptlb:v03,ptin:_v03,varname:_v03,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:8710,x:32855,y:33524,ptovrint:False,ptlb:tex03,ptin:_tex03,varname:_tex03,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5153-UVOUT;n:type:ShaderForge.SFN_Multiply,id:5669,x:33551,y:33012,varname:node_5669,prsc:2|A-4050-OUT,B-8710-RGB,C-8710-A,D-8264-RGB,E-8264-A;n:type:ShaderForge.SFN_VertexColor,id:6889,x:33565,y:33192,varname:node_6889,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:8264,x:33261,y:33388,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_8264,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9162,x:33792,y:33092,varname:node_9162,prsc:2|A-5669-OUT,B-6889-RGB,C-6889-A,D-5553-RGB,E-4487-OUT;proporder:5553-4487-6187-974-8689-9529-9020-3053-8710-2872-1497-8264;pass:END;sub:END;*/

Shader "Fps/Effects/texblend" {
    Properties {
        _maincolor ("maincolor", Color) = (0.5,0.5,0.5,1)
        _main_v ("main_v", Float ) = 1
        _maintex ("maintex", 2D) = "white" {}
        _u ("u", Float ) = 1
        _v ("v", Float ) = 1
        _tex02 ("tex02", 2D) = "white" {}
        _u02 ("u02", Float ) = 0
        _v02 ("v02", Float ) = 0
        _tex03 ("tex03", 2D) = "white" {}
        _u03 ("u03", Float ) = 0
        _v03 ("v03", Float ) = 0
        _mask ("mask", 2D) = "white" {}
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _maintex; uniform float4 _maintex_ST;
            uniform float4 _maincolor;
            uniform float _main_v;
            uniform float _u;
            uniform float _v;
            uniform sampler2D _tex02; uniform float4 _tex02_ST;
            uniform float _u02;
            uniform float _v02;
            uniform float _u03;
            uniform float _v03;
            uniform sampler2D _tex03; uniform float4 _tex03_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_9316 = _Time;
                float2 node_1464 = (i.uv0+(node_9316.g*float2(_u,_v))*float2(1,1));
                float4 _maintex_var = tex2D(_maintex,TRANSFORM_TEX(node_1464, _maintex));
                float4 node_1562 = _Time;
                float2 node_9785 = (i.uv0+(node_1562.g*float2(_u02,_v02))*float2(1,1));
                float4 _tex02_var = tex2D(_tex02,TRANSFORM_TEX(node_9785, _tex02));
                float3 node_4050 = (_maintex_var.rgb*_maintex_var.a*_tex02_var.rgb*_tex02_var.a);
                float4 node_4399 = _Time;
                float2 node_5153 = (i.uv0+(node_4399.g*float2(_u03,_v03))*float2(1,1));
                float4 _tex03_var = tex2D(_tex03,TRANSFORM_TEX(node_5153, _tex03));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 emissive = ((node_4050*_tex03_var.rgb*_tex03_var.a*_mask_var.rgb*_mask_var.a)*i.vertexColor.rgb*i.vertexColor.a*_maincolor.rgb*_main_v);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
