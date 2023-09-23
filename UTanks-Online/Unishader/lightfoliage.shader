Shader "TankiOnline/LightFoliage" {
	Properties {
		_Color ("Main Color", Vector) = (1,1,1,1)
		_MainTex ("Base (RGB) Opaque (A)", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_BumpScale ("Normal Map Scale", Float) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 1
		_SmoothNormals ("Smooth Normals", Range(0, 1)) = 0
		[Space] _WindShakeTime ("Wind Shake Time", Range(0, 5)) = 1
		_WindShakeWindspeed ("Wind Shake Windspeed", Range(0, 5)) = 1
		_WindShakeBending ("Wind Shake Bending", Range(0, 5)) = 1
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
	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}