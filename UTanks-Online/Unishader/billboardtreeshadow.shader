Shader "TankiOnline/BillboardTreeShadow" {
	Properties {
		_MainTex ("Texture Image", 2D) = "white" {}
		_Cutoff ("Cutoff", Range(0, 1)) = 0.5
		_FrameInfo ("FrameInfo", Vector) = (1,1,0,1)
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