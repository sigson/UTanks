	
	uniform sampler2D _MainTex;
	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData2;	// x = minimum altitude, y = altitude scattering, z = coverage extension

	struct Input {
		float2 uv_MainTex;
    	float3 worldPos;
		float3 worldNormal;
	};

	// get snow coverage on simple forms
	void SetBasicCoverage(Input IN, inout SurfaceOutput o) { 

		// prevent snow on sides and below minimum altitude
		float3 wsNormal = IN.worldNormal; 
		float ny = wsNormal.y - 0.2;
		float flatSurface = saturate(ny * 10.0);
		float minAltitude = saturate( IN.worldPos.y - _GS_SnowData2.x);
		float snowCover   = minAltitude * flatSurface;
		
		// pass color data to output shader
		o.Albedo = 1.0; // _GS_SnowData1.www;
		o.Alpha *= snowCover;
	}	
