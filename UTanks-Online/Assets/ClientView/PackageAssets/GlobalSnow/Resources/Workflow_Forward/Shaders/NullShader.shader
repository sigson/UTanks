Shader "GlobalSnow/NullShader" {
Properties {
}

SubShader {
	Pass {
	ZWrite Off ZTest Always

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	
	struct v2f {
    	float4 pos : SV_POSITION;
	};

	v2f vert( appdata_base v ) {
	    v2f o;
	    o.pos = 0;
	    return o;
	}
	
	fixed4 frag(v2f i) : SV_Target {
		discard;
		return fixed4(0,0,0,0);
	}
	ENDCG
	}
	
}

Fallback Off
}
