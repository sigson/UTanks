Shader "GlobalSnow/Deferred Overlay" {
	Properties {
		[HideInInspector] _Color ("Color", Color) = (1,1,1,1)
		[HideInInspector] _MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off
	  	Fog { Mode Off }
// Uncomment to support excluding terrain (see documentation for additional steps)
/*		Stencil {
			Ref 8
			ReadMask 8
			Comp notEqual
			Pass keep
		}
*/
		Pass {	
			CGPROGRAM
      		#pragma vertex vert
      		#pragma fragment frag
      		#pragma target 3.0
      		#pragma exclude_renderers nomrt
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile _ GLOBALSNOW_RELIEF GLOBALSNOW_OCCLUSION
			#pragma multi_compile _ GLOBALSNOW_FOOTPRINTS
			#pragma multi_compile _ GLOBALSNOW_TERRAINMARKS
			#pragma multi_compile _ UNITY_HDR_ON
			#define GLOBALSNOW_ENABLE_SLOPE_CONTROL
      		#include "SnowOverlay.cginc"
      		ENDCG
      	}
		Pass {	
			CGPROGRAM
      		#pragma vertex vert
      		#pragma fragment frag
      		#pragma target 3.0
      		#pragma exclude_renderers nomrt
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile _ GLOBALSNOW_FOOTPRINTS
			#pragma multi_compile _ GLOBALSNOW_TERRAINMARKS
			#pragma multi_compile _ UNITY_HDR_ON
			#define GLOBALSNOW_ENABLE_SLOPE_CONTROL
			#define GLOBALSNOW_FLAT_SHADING 1
      		#include "SnowOverlay.cginc"
      		ENDCG
      	}
	}
	FallBack Off
}
