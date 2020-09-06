// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33496,y:32711,varname:node_9361,prsc:2|custl-1032-OUT;n:type:ShaderForge.SFN_Tex2d,id:9114,x:33057,y:32572,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4840-UVOUT;n:type:ShaderForge.SFN_Panner,id:4840,x:32675,y:32635,varname:node_4840,prsc:2,spu:0,spv:1|UVIN-8936-OUT,DIST-4846-OUT;n:type:ShaderForge.SFN_TexCoord,id:5490,x:31603,y:32545,varname:node_5490,prsc:2,uv:0,uaff:True;n:type:ShaderForge.SFN_Tex2d,id:8453,x:32271,y:32934,ptovrint:False,ptlb:noise,ptin:_noise,varname:node_8453,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1008-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6517,x:31465,y:32751,ptovrint:False,ptlb:noise_u,ptin:_noise_u,varname:node_6517,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7450,x:31465,y:33000,ptovrint:False,ptlb:noise_v,ptin:_noise_v,varname:node_7450,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Time,id:7147,x:31465,y:32834,varname:node_7147,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8233,x:31652,y:32787,varname:node_8233,prsc:2|A-6517-OUT,B-7147-T;n:type:ShaderForge.SFN_Multiply,id:6040,x:31652,y:32924,varname:node_6040,prsc:2|A-7147-T,B-7450-OUT;n:type:ShaderForge.SFN_Append,id:9231,x:31825,y:32924,varname:node_9231,prsc:2|A-8233-OUT,B-6040-OUT;n:type:ShaderForge.SFN_Add,id:8936,x:32441,y:32513,varname:node_8936,prsc:2|A-5490-UVOUT,B-3992-OUT;n:type:ShaderForge.SFN_VertexColor,id:9793,x:32802,y:32836,varname:node_9793,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1032,x:33244,y:32790,varname:node_1032,prsc:2|A-9114-RGB,B-9793-A,C-9793-RGB,D-5969-RGB,E-8090-OUT;n:type:ShaderForge.SFN_Tex2d,id:5969,x:32853,y:33030,ptovrint:False,ptlb:alpha,ptin:_alpha,varname:node_5969,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3992,x:32460,y:32934,varname:node_3992,prsc:2|A-8453-R,B-8797-OUT;n:type:ShaderForge.SFN_Slider,id:8797,x:31955,y:33183,ptovrint:False,ptlb:noise_qiangdu,ptin:_noise_qiangdu,varname:node_8797,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:1008,x:32047,y:32889,varname:node_1008,prsc:2|A-5490-UVOUT,B-9231-OUT;n:type:ShaderForge.SFN_Vector1,id:1158,x:32000,y:32824,varname:node_1158,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:5393,x:32062,y:32642,varname:node_5393,prsc:2|A-5490-Z,B-5490-W;n:type:ShaderForge.SFN_Multiply,id:4846,x:32230,y:32742,varname:node_4846,prsc:2|A-5393-OUT,B-1158-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8090,x:33163,y:33032,ptovrint:False,ptlb:tex_v,ptin:_tex_v,varname:node_8090,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:9114-8453-6517-7450-5969-8797-8090;pass:END;sub:END;*/

Shader "Fps/Effects/texnoise_customuv_add" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _noise ("noise", 2D) = "white" {}
        _noise_u ("noise_u", Float ) = 0
        _noise_v ("noise_v", Float ) = 0
        _alpha ("alpha", 2D) = "white" {}
        _noise_qiangdu ("noise_qiangdu", Range(0, 1)) = 0
        _tex_v ("tex_v", Float ) = 1
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform float _noise_u;
            uniform float _noise_v;
            uniform sampler2D _alpha; uniform float4 _alpha_ST;
            uniform float _noise_qiangdu;
            uniform float _tex_v;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
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
                float4 node_7147 = _Time;
                float2 node_1008 = (i.uv0+float2((_noise_u*node_7147.g),(node_7147.g*_noise_v)));
                float4 _noise_var = tex2D(_noise,TRANSFORM_TEX(node_1008, _noise));
                float2 node_4840 = ((i.uv0+(_noise_var.r*_noise_qiangdu))+(float2(i.uv0.b,i.uv0.a)*1.0)*float2(0,1));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4840, _MainTex));
                float4 _alpha_var = tex2D(_alpha,TRANSFORM_TEX(i.uv0, _alpha));
                float3 finalColor = (_MainTex_var.rgb*i.vertexColor.a*i.vertexColor.rgb*_alpha_var.rgb*_tex_v);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
       
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
