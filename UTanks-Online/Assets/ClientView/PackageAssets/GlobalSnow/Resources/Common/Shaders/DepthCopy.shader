Shader "GlobalSnow/DepthCopy" {
Properties {
}

SubShader {

	CGINCLUDE
	#include "UnityCG.cginc"
	
	struct v2f {
    	float4 pos : SV_POSITION;
    	float depth : TEXCOORD0;
	};

	v2f vert( appdata_base v ) {
	    v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
	    o.depth = COMPUTE_DEPTH_01;
	    // cover all horizon
	    float4 scrPos = ComputeScreenPos(o.pos);
	    if (scrPos.y<=0.01 || scrPos.y>=0.99) o.depth = 1.0; 
	    if (scrPos.x<=0.01 || scrPos.x>=0.99) o.depth = 1.0;
	    return o;
	}
	
	float4 frag(v2f i) : SV_Target {
		return float4(i.depth, 0.0, 1.0, 1.0);
	}


	ENDCG

	Pass { // Depth snapshot
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
	}

}

Fallback Off
}
