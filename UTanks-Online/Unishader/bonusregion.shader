Shader "TankiOnline/Bonus Region" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Main Color", Vector) = (1,1,1,1)
		_SideTint ("Side Tint", Vector) = (1,1,1,1)
		_FrontTint ("Front Tint", Vector) = (1,1,1,1)
		_EmissionIntensity ("Emission Intensity", Float) = 4
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
}