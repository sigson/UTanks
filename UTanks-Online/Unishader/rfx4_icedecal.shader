Shader "KriptoFX/RFX4/Decal/Ice" {
	Properties {
		_Color ("Main Color", Vector) = (1,1,1,1)
		[HDR] _ReflectColor ("Reflection Color", Vector) = (1,1,1,0.5)
		_MainTex ("Base (RGB) Emission Tex (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_FPOW ("FPOW Fresnel", Float) = 5
		_R0 ("R0 Fresnel", Float) = 0.05
		_Cutoff ("Cutoff", Range(0, 1)) = 0.5
		_BumpAmt ("Distortion", Range(0, 1500)) = 10
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
}