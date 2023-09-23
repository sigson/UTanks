Shader "TankiOnline/Decal" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Color (RGB), Roughness (A)", 2D) = "white" {}
		_Roughness ("Roughness", Range(0, 1)) = 1
		[NoScaleOffset] _SurfaceMap ("Metallic(R), Occlusion(G), Height(B), Specularity(A)", 2D) = "black" {}
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_Specularity ("Specularity", Range(0, 1)) = 0.5
		_OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
		[Toggle(_PARALLAX)] _UseParallax ("Parallax", Float) = 0
		_Parallax ("Height Scale", Range(0.005, 0.08)) = 0.02
		_BumpScale ("Normal Map Scale", Float) = 1
		[NoScaleOffset] [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
		[HDR] _EmissionColor ("Emission Color", Vector) = (0,0,0,1)
		[NoScaleOffset] _EmissionMap ("Emission", 2D) = "white" {}
		_Glossiness ("Sync smoothness data", Range(0, 1)) = 0.5
	}
	
	//DummyShaderTextExporter - One Sided
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
	//CustomEditor "TankiOnlineShaderGUI"
}