

//////////////////////////////////////////////////////////////////////////
//
//   FileName : AlphaBlendAlways.shader
//     Author : Chiyer
// CreateTime : 2015-08-12
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

Shader "Fps/Effects/AlphaBlendedAlways" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	Lighting Off 
	ZWrite Off 
	ZTest Off
	Fog { Mode Off }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
