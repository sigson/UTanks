Shader "GlobalSnow/Moving Object Snow/Opaque" {
	Properties {
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
		 _Color ("Color", Color) = (1,1,1,1)
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
         _SnowTint ("Snow Tint Color", Color) = (1,1,1,1)
         _SnowCoverage ("Coverage", Range(0,10)) = 1
         _SnowScale ("Snow Scale", Float) = 1.0
         _Scatter ("Scatter", Range(0,10)) = 3
         _SlopeThreshold ("Slope Threshold", Range(0,1)) = 0.7
         _SlopeNoise ("Slope Noise", Range(0,5)) = 2.5
         _SlopeSharpness ("Slope Sharpness", Range(0,10)) = 5
	}
	SubShader {
    Tags { "Queue"="Geometry" "RenderType"="Opaque" }  
    CGPROGRAM
    #pragma surface surf Standard vertex:vert addshadow
    #pragma target 3.0
    #pragma fragmentoption ARB_precision_hint_fastest
    #pragma multi_compile __ GLOBALSNOW_FLAT_SHADING GLOBALSNOW_RELIEF GLOBALSNOW_OCCLUSION
//    #pragma multi_compile __ GLOBALSNOW_OPAQUE_CUTOUT
    #define GLOBALSNOW_ENABLE_SLOPE_CONTROL 1
    #define GLOBALSNOW_IS_STANDARD_SHADER 1
    #include "GlobalSnowMovingObject.cginc"

    half _Glossiness;
    half _Metallic;

    void vert (inout appdata_full v, out Input data) {
        UNITY_INITIALIZE_OUTPUT(Input, data);
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        data.wpos = float3(v.vertex.x, worldPos.y, v.vertex.z);     
    }

    void surf (Input IN, inout SurfaceOutputStandard o) {
/*        #if GLOBALSNOW_OPAQUE_CUTOUT
            fixed4 textureColor = tex2D(_MainTex, IN.uv_MainTex);
            clip(textureColor.a + textureColor.g - 0.01);
        #endif
*/
        SetSnowCoverage(IN, o);
        o.Albedo = lerp(tex2D(_MainTex, IN.uv_MainTex).rgb * _Color, o.Albedo, o.Alpha);
        o.Metallic = lerp(_Metallic, o.Metallic, o.Alpha);
        o.Smoothness = lerp(_Glossiness, 0.5, o.Alpha);
    }
    
    ENDCG
    }
}

