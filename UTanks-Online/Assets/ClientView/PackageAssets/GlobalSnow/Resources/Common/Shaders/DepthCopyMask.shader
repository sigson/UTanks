Shader "GlobalSnow/DepthCopyMask" {
Properties {
	//_GS_DepthMask ("Depth mask", 2D) = "white" {}
	//_GS_DepthMaskWorldSize ("Depth mask size", Vector) = (2000,0,2000,0)
}

SubShader {

	CGINCLUDE
	#include "UnityCG.cginc"

	struct v2f {
    	float4 pos : SV_POSITION;
    	float depth : TEXCOORD0;
    	float3 worldPos: TEXCOORD1;
        float2 uv : TEXCOORD2;
	};

	sampler2D _GS_DepthMask;
	float4 _GS_DepthMaskWorldSize;

	v2f vert( appdata_base v ) {
	    v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
	    o.depth = COMPUTE_DEPTH_01;
	    o.worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
	    // cover all horizon
	    float4 scrPos = ComputeScreenPos(o.pos);
        if (scrPos.y<=0.01 || scrPos.y>=0.99) o.depth = 1.0; 
        if (scrPos.x<=0.01 || scrPos.x>=0.99) o.depth = 1.0;
        o.uv = scrPos.xy;
	    return o;
	}
	
	float4 frag (v2f i) : SV_Target {
        float2 maskUV = (i.worldPos.xz - _GS_DepthMaskWorldSize.yw) / _GS_DepthMaskWorldSize.xz + 0.5.xx;
		float a = tex2D(_GS_DepthMask, maskUV).a;
         if (maskUV.x<=0.01 || maskUV.x>=0.99 || maskUV.y<=0.01 || maskUV.y>=0.99) a = 1.0;
		return float4(i.depth, 1.0 - a, 0, 0);
	}


	ENDCG

	Pass { // Depth snapshot with mask
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
	}

}

Fallback Off
}
