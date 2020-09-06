Shader "XSJ/Map/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",Range(0,4))=0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag           
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            struct a2v{
                float4 vertex:POSITION;
                float4 texcoord:TEXCOORD;
            };
            struct v2f{
                float4 pos:POSITION;
                float2 uv:texcoord;
            };
            v2f vert(a2v v){
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
            fixed4 frag(v2f i):SV_Target{
                //平移
                return tex2D(_MainTex,i.uv - _Time.x*fixed2(2,0)* _Speed);
            }
            ENDCG
        }
    }
}