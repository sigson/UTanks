Shader "GlobalSnow/SnowForReflections" {

SubShader {
	CGPROGRAM
	#pragma surface surf BlinnPhong exclude_path:deferred exclude_path:prepass nometa
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile __ GLOBALSNOW_FLAT_SHADING
	#define GLOBALSNOW_ENABLE_SLOPE_CONTROL 1
	#define GLOBALSNOW_IS_TERRAIN 1
	#include "GlobalSnow.cginc"

    void surf (Input IN, inout SurfaceOutput o) {
		// Check alpha
		fixed4 textureColor = tex2D(_MainTex, IN.uv_MainTex);
		clip(textureColor.a + textureColor.g - 0.1);
		SetSnowCoverage(IN, o);
		o.Albedo = lerp(textureColor.rgb, o.Albedo, o.Alpha);
    }
    
    ENDCG
}


Fallback Off
}
