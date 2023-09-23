Shader "GlobalSnow/DeferredMaskWrite" {
SubShader {

	Tags { "DisableBatching" = "True" }
	Cull [_EraseCullMode]

	CGINCLUDE
	#include "UnityCG.cginc"
    #include "GlobalSnowDeferredOptions.cginc"
    
    struct appdata {
    	float4 vertex : POSITION;
        float2 uv     : TEXCOORD0;
    };

	struct v2f {
	    float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};

	sampler2D _GS_MaskTexture;
	float4 _GS_MaskTexture_ST;
	fixed _GS_MaskCutOff;

	v2f vert(appdata v) {
    	v2f o;
        APPLY_VERTEX_MODIFIER(v)
    	o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _GS_MaskTexture);
		return o;
	}	
	
	fixed4 frag(v2f i): SV_Target {
		fixed4 color = tex2D(_GS_MaskTexture, i.uv);
		color.a = 1.0 - color.a;
		clip(color.a - _GS_MaskCutOff);
		return fixed4(0,0,0,0);
	}	
	
	
	ENDCG

	Pass { 
       	Fog { Mode Off }
       	ColorMask 0
		CGPROGRAM
		#pragma target 3.0
   		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
	}
	
}

Fallback Off
}
