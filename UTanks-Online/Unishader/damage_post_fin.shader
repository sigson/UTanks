Shader "Shader Forge/damage_post_fin" {
	Properties {
		[HideInInspector] _MainTex ("MainTex", 2D) = "white" {}
		_distortion ("distortion", 2D) = "white" {}
		_TV_lines ("TV_lines", 2D) = "white" {}
		_mask ("mask", 2D) = "white" {}
		_dist_amount ("dist_amount", Range(0, 1)) = 0
		_damage_pixel_offset ("damage_pixel_offset", Range(0, 1)) = 0.15
		_ani_speed ("ani_speed", Range(0, 5)) = 0
		_damaged_pixel_mask ("damaged_pixel_mask", Range(0, 1)) = 1
		_pixel_mask_ani_speed ("pixel_mask_ani_speed", Range(0, 1)) = 0.3
		_fade_min ("fade_min", Range(0, 1)) = 0.18
		_fade_max ("fade_max", Range(0, 1)) = 0.72
		_fade_color ("fade_color", Vector) = (1,0.145098,0,1)
		_desaturate ("desaturate", Range(0, 1)) = 0.35
		_color_distort_size ("color_distort_size", Range(0, 0.1)) = 0.002
		_color_distort_speed ("color_distort_speed", Range(0, 20)) = 14.5
		_TV_lines_speed ("TV_lines_speed", Range(0, 1)) = 1
		_line_size ("line_size", Float) = 40
		_TV_vis ("TV_vis", Range(0, 1)) = 0
		_TV_speed_mask ("TV_speed_mask", Range(0, 1)) = 0.25
		_lines_blink_speed ("lines_blink_speed", Range(0, 1)) = 0.12
		_damage_lvl ("damage_lvl", Range(0, 100)) = 0
		_damage_lvl_start ("damage_lvl_start", Float) = 0
		_damage_lvl_1 ("damage_lvl_1", Float) = 25
		_damage_lvl_2 ("damage_lvl_2", Float) = 50
		_damage_lvl_3 ("damage_lvl_3", Float) = 75
		_damage_lvl_4 ("damage_lvl_4", Float) = 100
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
	//CustomEditor "ShaderForgeMaterialInspector"
}