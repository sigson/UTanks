	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData2;	// x = minimum altitude, y = altitude scattering, z = coverage extension
	uniform float4    _GS_SnowData3;	// x = Sun occlusion, y = sun atten, z = ground coverage, w = grass coverage
    uniform float4    _GS_SnowCamPos;
    uniform half4     _GS_SnowTint;
    uniform sampler2D_float _GS_DepthTexture;
    	
struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 worldPos;
};


	// get snow coverage on grass
	void SetGrassCoverage(Input IN, inout SurfaceOutput o) { 
		// prevent snow on sides and below minimum altitude
		float minAltitude = saturate( IN.worldPos.y - _GS_SnowData2.x);
		float snowCover   = minAltitude * saturate(IN.uv_MainTex.y + _GS_SnowData3.w);

        // mask support
        #if defined(GLOBALSNOW_MASK)
            float4 st = float4(IN.worldPos.xz - _GS_SnowCamPos.xz, 0, 0);
            st *= _GS_SnowData2.z;
            st += 0.5;
            float zmask = tex2Dlod(_GS_DepthTexture, st).g;
            snowCover   = max(snowCover - zmask, 0);
        #endif

		// pass color data to output shader
		o.Albedo = _GS_SnowData1.www;
        o.Albedo.rgb *= _GS_SnowTint.rgb;
		o.Alpha = snowCover * 0.96;
	}
	
	
	
