Shader "TankiOnline/Parachute" {
	Properties {
		_Color ("Main Color", Vector) = (1,1,1,1)
		_MainTex ("Base (RGB) Opaque (A)", 2D) = "white" {}
		_BumpScale ("Normal Map Scale", Float) = 1
		[NoScaleOffset] [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
		_TransColor ("Transmission Color", Vector) = (1,1,1,1)
		[NoScaleOffset] _TransMap ("Transmission Map", 2D) = "white" {}
		_TransWeight ("Transmission Weight", Range(0, 1)) = 1
		_TransFallof ("Transmission Falloff", Range(0.01, 10)) = 1
		_TransDistortion ("Transmission Distortion", Range(0, 1)) = 1
		[Space] _HidingCenter ("Hide Center  - XYZ", Vector) = (0,0,0,0)
		_MinHidingRadius ("Min Hiding Radius", Float) = 0
		_MaxHidingRadius ("Max Hiding Radius", Float) = 0
		_HidingSpeed ("HidingSpeed", Float) = 0
		_HidingStartTime ("Hiding Start Time", Float) = 0
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