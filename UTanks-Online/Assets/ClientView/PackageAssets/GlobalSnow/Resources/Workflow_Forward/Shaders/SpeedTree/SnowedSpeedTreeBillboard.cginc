	
	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData2;	// x = minimum altitude for vegetation
	uniform float4    _GS_SnowData4;    // w = billboard coverage

	
	// get snow coverage on tree billboards
	void SetTreeBillboardCoverage(Input IN, inout SurfaceOutput o) { 
		
		float minAltitude = saturate((IN.worldPos.y - _GS_SnowData2.x) * 0.1);
		float solid       = o.Albedo.g * 2.0 - _GS_SnowData4.w;
		float snowCover   = minAltitude * saturate(solid) * 0.95;

		// pass color data to output shader
		o.Albedo = (1.0 + o.Albedo.g) * (0.25 + _GS_SnowData1.w) * solid;
		o.Alpha  = snowCover;
	}	
