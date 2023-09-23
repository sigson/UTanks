Shader "Shader Forge/cyl_light" {
	Properties {
		_Color ("Color", Vector) = (0.07843138,0.3921569,0.7843137,1)
		_falloff ("falloff", Range(-1, 0)) = -0.5
		_speed ("speed", Range(0, 10)) = 3
		_noise ("noise", 2D) = "white" {}
		_center_falloff ("center_falloff", Range(-1, 0)) = -0.98
		_dust_size ("dust_size", Range(0, 1)) = 0.053
		_dust_speed ("dust_speed", Range(0, 10)) = 9
		_glow_vis ("glow_vis", Range(0, 1)) = 0.4
		_glow_falloff ("glow_falloff", Range(-1, 0)) = -0.8
		_glow_center_falloff ("glow_center_falloff", Range(-1, 0)) = -0.95
		_power ("power", Range(0, 3)) = 0
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
	Fallback "Diffuse"
	//CustomEditor "ShaderForgeMaterialInspector"
}