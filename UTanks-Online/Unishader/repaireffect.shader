Shader "Alternativa/RepairEffect" {
	Properties {
		_RepairStubColor ("Repair Stub Color", Vector) = (1,1,1,1)
		_RepairFrontCoeff ("Repair Front Coeff", Range(0, 1)) = 0
		_RepairMovementDirection ("Repair Movement Direction", Range(0, 1)) = 1
		_RepairBackCoeff ("Repair back coeff", Range(0, 1)) = 0.78
		_RepairAdditionalLengthExtension ("Repair Length extension", Float) = 10
		_RepairVolume ("Repair volume", Vector) = (1.502808,0.778753,2.34775,0)
		_RepairCenter ("Repair Center", Vector) = (0,-0.02930036,0.60568,0)
		_RepairTex ("Repair Effect Texture", 2D) = "white" {}
		_RepairFadeAlphaRange ("Repair Fade alpha range", Range(0, 1)) = 0.92
		_RepairAlpha ("Repair alpha", Range(0, 1)) = 1
		_RepairRotationAngle ("Repair Rotation Angle", Range(-10, 10)) = 0
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
}