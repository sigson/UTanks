Shader "Alternativa/TankStandardInvisibilityEffect" {
	Properties {
		_Alpha ("Alpha", Range(0, 1)) = 1
		_SurfaceMap ("Coloring mask (a), Occlusion (r), Height Map (g), Emission (b)", 2D) = "white" {}
		_Masks ("Burning Mask (a), Freezing emission Mask (r), White frost Mask (g)", 2D) = "white" {}
		_MainTex ("Albedo, Smoothness (a)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale ("Normal scale", Float) = 1
		_Metallic ("Metallic", Range(0, 1)) = 0
		_OcclusionStrength ("Occlusion strength", Range(0, 1)) = 1
		_Parallax ("Height scale", Range(0.005, 0.08)) = 0.02
		_SmoothnessStrength ("Smoothness strength", Range(0, 5)) = 1
		_EmissionColor ("Emission color", Vector) = (0,0,0,1)
		_EmissionIntensity ("EmissionIntensity", Float) = 1
		_ColoringMap ("Coloring emission mask or smoothness (a), Coloring (rgb)", 2D) = "white" {}
		_Coloring ("Coloring", Vector) = (1,1,1,1)
		_MetallicColoring ("Metallic", Range(0, 1)) = 0
		_ColoringSmoothness ("Smoothness", Range(0, 5)) = 0
		_ColoringBumpMap ("Normal Map", 2D) = "bump" {}
		_ColoringBumpScale ("Normal scale", Float) = 1
		_ColoringMaskThreshold ("Coloring mask threshold", Float) = -1
		_ColoringBumpMapDef ("", Float) = -1
		_ColoringMapAlphaDef ("", Float) = -1
		_WhiteFrostNormalMap ("White frost Normal Map", 2D) = "bump" {}
		_WhiteFrostNormalScale ("White frost Normal scale", Float) = 1
		_TotalWhiteFrostTemperature ("Total white frost temperature", Range(-1, 0)) = -0.5
		_FreezingMap ("Freezing Albedo (rgb), Mask (a)", 2D) = "white" {}
		_FreezingNormalMap ("Freezing normal map", 2D) = "bump" {}
		_FreezeNormalScale ("Freeze normal scale", Float) = 1
		_FreezeSmoothness ("Smoothness", Range(0, 1)) = 1
		_FreezeMetallic ("Metallic", Range(0, 1)) = 1
		_FreezeEmissionColor ("Freezing emission", Vector) = (1,1,1,1)
		_FreezeEmissionIntensity ("Freezing emission intensity", Float) = 1
		_AdditiveFreezeEmissionColor ("Additive freezing emission ", Vector) = (1,1,1,1)
		_AdditiveFreezeEmissionIntensity ("Additive freezing emission intensity", Float) = 1
		_FreezeTransColor ("Freeze transmission color", Vector) = (1,1,1,1)
		_MaxFreezeIntensity ("Freeze max intensity", Range(0, 1)) = 1
		_TotalBurntColoringTemperature ("Total burnt coloring temperature", Range(0, 1)) = 0.5
		_StrongBurningColor ("Strong burning", Vector) = (0.505,0.00727,0,1)
		_WeakBurningColor ("Weak burning", Vector) = (0.174,0.179,0.185,1)
		_BurningColoringMetallic ("Burning coloring metallic", Range(0, 1)) = 0.2
		_BurningMetallic ("Burning metallic", Range(0, 1)) = 0.5
		_BurningEmissionIntensity ("Strong burning emission intensity", Float) = 20
		_Temperature ("Temperature", Range(-1, 1)) = 0
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
		_DissolveMap ("Dissolve map", 2D) = "white" {}
		_DissolveCoeff ("Dissolve coeff", Range(0, 1.05)) = 0
		_DistortionCoeff ("Distortion coeff", Range(0, 1024)) = 0
		[HideInInspector] _Mode ("__mode", Float) = 0
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _ZWrite ("__zw", Float) = 1
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