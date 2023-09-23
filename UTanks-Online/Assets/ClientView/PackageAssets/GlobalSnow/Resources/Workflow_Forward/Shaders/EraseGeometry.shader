Shader "GlobalSnow/EraseGeometry" {
Properties {
}

SubShader {
	Tags { "RenderType"="Opaque" "DisableBatching"="True" }
	ZWrite Off 
	ColorMask 0
	Pass {

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	
	struct v2f {
    	float4 pos : SV_POSITION;
	};

	v2f vert( appdata_base v ) {
	    v2f o;
	    o.pos = UnityObjectToClipPos(v.vertex);
	    return o;
	}
	
	fixed4 frag(v2f i) : SV_Target {
		return fixed4(0,0,0,0);
	}
	ENDCG
	}
	
}

Fallback Off
}
