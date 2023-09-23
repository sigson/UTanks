Shader "GlobalSnow/Snow Particle Surface Shader" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	ColorMask RGB
	Cull Off ZWrite Off

	SubShader {
		CGPROGRAM
		#pragma surface surf Simple vertex:vert alpha
		#pragma target 3.0

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float _InvFade;

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
			o.Albedo = co.rgb;
			o.Alpha = co.a * IN.color.a;
		}
		ENDCG 
	}
}
}
