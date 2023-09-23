	
	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData2;	// x = minimum altitude, y = altitude scattering, z = coverage extension
    uniform float4    _GS_SnowCamPos;
    uniform half4     _GS_SnowTint;
	uniform sampler2D_float _GS_DepthTexture;
    
	// get snow coverage on trees
	void SetTreeCoverage(Input IN, inout SurfaceOutput o) { 

		// prevent snow on sides and below minimum altitude
		float3 wsNormal = IN.worldNormal; 
		float ny = wsNormal.y - 0.2;
		float flatSurface = saturate(ny * 10.0);
		float minAltitude = saturate( IN.worldPos.y - _GS_SnowData2.x);

          // mask support
        #if defined(GLOBALSNOW_MASK)
            float4 st = float4(IN.worldPos.xz - _GS_SnowCamPos.xz, 0, 0);
            st *= _GS_SnowData2.z;
            st += 0.5;
            float zmask = tex2Dlod(_GS_DepthTexture, st).g;
            float snowCover   = max(minAltitude * flatSurface - zmask, 0);
        #else
            float snowCover   = minAltitude * flatSurface;
        #endif
		
		
		// pass color data to output shader
		o.Albedo = _GS_SnowData1.www;
        o.Albedo.rgb *= _GS_SnowTint.rgb;
		o.Alpha *= snowCover;
	}	
