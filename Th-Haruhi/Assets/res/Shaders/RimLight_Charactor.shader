Shader "Fps/RimLight"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		[HideInInspector] _Normals("Normals", 2D) = "bump" {}
		_RimColor("RimColor", Color) = (0,0,0,0)
		_RimPower("RimPower", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha exclude_path:deferred 
		struct Input
		{
			half2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform half _RimPower;
		uniform half4 _RimColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			float3 normalizeResult23 = normalize( i.viewDir );
			float dotResult21 = dot( UnpackNormal( tex2D( _Normals, uv_Normals ) ) , normalizeResult23 );
			o.Emission = ( pow( ( 1.0 - saturate( dotResult21 ) ) , _RimPower ) * _RimColor ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Mobile/VertexLit"
}