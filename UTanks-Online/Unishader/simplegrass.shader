Shader "TankiOnline/SimpleGrass" {
	Properties {
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _LightMap ("Texture", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		_WindShakeTime ("Wind Shake Time", Range(0, 5)) = 1
		_WindShakeWindspeed ("Wind Shake Windspeed", Range(0, 5)) = 1
		_WindShakeBending ("Wind Shake Bending", Range(0, 5)) = 1
		_GrassCullingDistance ("Culling distance", Float) = 50
		_GrassCullingRange ("Culling range", Float) = 1
		_Ambient ("ambient", Float) = 0.7
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