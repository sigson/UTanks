Shader "GlobalSnow/Snow Particle Surface Shader Opaque" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Float) = 0.01
}

Subshader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="ParticleCutOut" }
	ColorMask RGB
	Cull Off ZWrite On

		CGPROGRAM
		#pragma surface surf Simple vertex:vert
		#pragma target 3.0
		#include "UnityCG.cginc"


		sampler2D _MainTex;
		fixed _Cutoff;

		struct Input {
			fixed4 color: COLOR;
			float2 uv_MainTex;
		};
			
		half4 LightingSimple(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = saturate(s.Albedo * _LightColor0.rgb * atten);
			c.a = s.Alpha;
			return c;
		}

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
		}
			
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 co = tex2D(_MainTex, IN.uv_MainTex);
			clip (co.a * IN.color.a - _Cutoff);
			o.Albedo = co.rgb;
		}
		ENDCG 
}
}
