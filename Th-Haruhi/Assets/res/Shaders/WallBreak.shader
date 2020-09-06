// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33520,y:32746,varname:node_4013,prsc:2|diff-3223-OUT;n:type:ShaderForge.SFN_Tex2d,id:9600,x:32353,y:32732,ptovrint:False,ptlb:maintex,ptin:_maintex,varname:node_9600,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:6924,x:32803,y:32872,varname:node_6924,prsc:2|A-9600-RGB,B-4923-OUT,T-4454-OUT;n:type:ShaderForge.SFN_Tex2d,id:1447,x:32412,y:33157,ptovrint:False,ptlb:masktex01,ptin:_masktex01,varname:node_1447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9742-UVOUT;n:type:ShaderForge.SFN_Lerp,id:3223,x:32926,y:33109,varname:node_3223,prsc:2|A-6924-OUT,B-9485-OUT,T-8500-OUT;n:type:ShaderForge.SFN_Tex2d,id:546,x:32438,y:33440,ptovrint:False,ptlb:mask02_tex,ptin:_mask02_tex,varname:_mask02,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9742-UVOUT;n:type:ShaderForge.SFN_Vector3,id:4923,x:32387,y:32915,varname:node_4923,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_ToggleProperty,id:2775,x:32426,y:33330,ptovrint:False,ptlb:mask01,ptin:_mask01,varname:node_2775,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Multiply,id:4454,x:32634,y:33174,varname:node_4454,prsc:2|A-1447-RGB,B-2775-OUT,C-1447-A;n:type:ShaderForge.SFN_Multiply,id:8500,x:32785,y:33476,varname:node_8500,prsc:2|A-546-RGB,B-3436-OUT,C-546-A;n:type:ShaderForge.SFN_ToggleProperty,id:3436,x:32438,y:33701,ptovrint:False,ptlb:mask02,ptin:_mask02,varname:node_3436,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_TexCoord,id:9742,x:32155,y:33043,varname:node_9742,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector3,id:9485,x:32479,y:33041,varname:node_9485,prsc:2,v1:0.1,v2:0.1,v3:0.1;proporder:9600-1447-546-2775-3436;pass:END;sub:END;*/

Shader "Fps/Effects/WallBreak" {
	Properties{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_masktex01("masktex01", 2D) = "white" {}
		_mask02_tex("mask02_tex", 2D) = "white" {}
		[MaterialToggle] _mask01("mask01", Float) = 0
		[MaterialToggle] _mask02("mask02", Float) = 0
	}

	SubShader{
	Tags{ "RenderType" = "Opaque" }
	LOD 250

	CGPROGRAM
	#pragma surface surf Lambert

	fixed4 _Color;
	sampler2D _MainTex;
	sampler2D _masktex01;
	sampler2D _mask02_tex;
	fixed _mask01;
	fixed _mask02;
	struct Input {
		float2 uv_MainTex;
		float2 uv_masktex01;
		float2 uv_mask02_tex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		half4 decal = tex2D(_masktex01, IN.uv_masktex01);
		half4 decal2 = tex2D(_mask02_tex, IN.uv_mask02_tex);
		if (_mask01 == 1)
			c.rgb = lerp(c.rgb, 0, decal.a);
		else if (_mask02 == 1)
			c.rgb = lerp(c.rgb, 0, decal2.a);

		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
	Fallback "Legacy Shaders/Diffuse"
}
