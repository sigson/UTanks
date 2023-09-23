Shader "GlobalSnow/EraseShader" {
Properties {
}

SubShader {
	Tags { "RenderType"="Opaque" "DisableBatching"="True" }
    Cull [_EraseCullMode]
	Pass {

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	#include "GlobalSnowForwardOptions.cginc"
    
	struct v2f {
    	float4 pos : SV_POSITION;
	};

	v2f vert( appdata_base v ) {
	    v2f o;
        APPLY_VERTEX_MODIFIER(v)
		o.pos = UnityObjectToClipPos(v.vertex);
	    return o;
	}
	
	fixed4 frag(v2f i) : SV_Target {
		return fixed4(0,0,0,0);
	}
	ENDCG
	}
	
}

SubShader {
	Tags { "RenderType"="TransparentCutout" "DisableBatching"="True" }
	Cull [_EraseCullMode]
	Pass {

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	#include "GlobalSnowForwardOptions.cginc"
    
	struct v2f {
    	float4 pos : SV_POSITION;
	};

	v2f vert( appdata_base v ) {
	    v2f o;
        APPLY_VERTEX_MODIFIER(v)
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
