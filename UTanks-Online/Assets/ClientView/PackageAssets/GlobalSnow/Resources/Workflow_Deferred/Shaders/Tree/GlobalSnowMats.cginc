	#include "UnityCG.cginc"

	#define SNOW_TEXTURE_SCALE 0.1
	#define RELIEF_SAMPLE_COUNT 8
	#define RELIEF_BINARY_SAMPLE_COUNT 8
	#define RELIEF_MAX_RAY_LENGTH 0.5
	#define SNOW_NORMAL_THRESHOLD 0.6

	uniform sampler2D _MainTex;
    uniform sampler2D _GS_SnowTex;
    uniform sampler2D _GS_SnowNormalsTex;
    uniform sampler2D _GS_NoiseTex;
    uniform sampler2D_float _GS_DepthTexture;
    uniform sampler2D_float _GS_FootprintTex;
    uniform sampler2D _GS_DetailTex;
    uniform sampler2D _GS_DecalTex;
    uniform float4    _GS_DecalTex_TexelSize;
	uniform float4 	  _GS_SnowCamPos;
    uniform float4    _GS_SunDir;		// w = terrain mark distance
	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData2;	// x = minimum altitude for vegetation and trees, y = altitude scattering, z = coverage extension, w = billboard coverage
	uniform float4    _GS_SnowData3;    // x = Sun occlusion, y = sun atten, z = ground coverage, w = grass coverage
	uniform float4    _GS_SnowData4;    // x = footprint scale, y = footprint obscurance, z = internally used, w = minimum altitude for terrain including scattering
	uniform float4    _GS_SnowData5;    // x = slope threshold, y = slope sharpness, z = slope noise

    #if !defined(USE_ZENITHAL_DEPTH)
    uniform sampler2D _GS_DepthMask;
    uniform float4 _GS_DepthMaskWorldSize;
    #endif

	#ifndef GLOBALSNOW_INPUT_STRUCTURE_INCLUDED
	#define GLOBALSNOW_INPUT_STRUCTURE_INCLUDED
	struct Input {
		float2 uv_MainTex;
    	float3 worldPos;
		#if GLOBALSNOW_FLAT_SHADING
			float3 worldNormal;
		#else
    		float3 worldNormal; INTERNAL_DATA
   		#endif
   		#ifdef GLOBALSNOW_MOVING_OBJECT
   			float3 wposOffset;
   		#endif
   	};
   	#endif

                
	// get snow coverage
	#if defined(GLOBALSNOW_IS_STANDARD_SHADER)
	void SetSnowCoverage(Input IN, inout SurfaceOutputStandard o)
	#elif defined(GLOBALSNOW_IS_NON_SURFACE_SHADER)
	void SetSnowCoverage(v2f IN, inout fixed4 finalColor)
	#else
	void SetSnowCoverage(Input IN, inout SurfaceOutput o)
	#endif
	{ 
        #if defined(USE_ZENITHAL_DEPTH)
            float4 st = float4(IN.worldPos.xz - _GS_SnowCamPos.xz, 0, 0);
            st *= _GS_SnowData2.z;
            st += 0.5;
            float2 zmask = tex2Dlod(_GS_DepthTexture, st).rg;
    	    #ifdef GLOBALSNOW_MOVING_OBJECT
    			fixed snowCover = any(IN.wposOffset.xz); // animated objects will always be snow-covered on top to prevent artifacts
	    		if (!snowCover) {
		    #else
    			fixed snowCover;
	    	#endif
            
			float y = max(_GS_SnowCamPos.y - IN.worldPos.y, 0.001) / _GS_SnowCamPos.w;
			float zd = min(zmask.r + _GS_SnowData3.z, y);
			snowCover = saturate( ((zd / y) - 0.9875) * 110.0);
            
    	    #ifdef GLOBALSNOW_MOVING_OBJECT
		        }
		        float3 snowPos = IN.worldPos - IN.wposOffset;
		    #else
		        float3 snowPos = IN.worldPos;
		    #endif
        #else
            fixed snowCover = 1.0;
            float2 maskUV = (IN.worldPos.xz - _GS_DepthMaskWorldSize.yw) / _GS_DepthMaskWorldSize.xz + 0.5.xx;
            float2 zmask = 1.0 - tex2D(_GS_DepthMask, maskUV).aa;
            if (maskUV.x<=0.01 || maskUV.x>=0.99 || maskUV.y<=0.01 || maskUV.y>=0.99) zmask.g = 0.0;
            #ifdef GLOBALSNOW_MOVING_OBJECT
                float3 snowPos = IN.worldPos - IN.wposOffset;
            #else
                float3 snowPos = IN.worldPos;
            #endif
        #endif
		
		// diffuse
		#if GLOBALSNOW_FLAT_SHADING
		fixed4 diff = tex2D(_GS_SnowTex, snowPos.xz * 0.02);
		#else
		fixed4 diff = tex2D(_GS_SnowTex, snowPos.xz * 0.02).gggg;
		#endif

		// get world space normal
		#if GLOBALSNOW_FLAT_SHADING || defined(GLOBALSNOW_IS_NON_SURFACE_SHADER)
		float3 wsNormal = IN.worldNormal;
		#else
		float3 wsNormal =  WorldNormalVector (IN, float3(0,0,1));
		#endif

		// prevent snow on walls and below minimum altitude
		float altG = tex2D(_GS_SnowTex, snowPos.xz * 0.5).g;
		float altNoise = diff.r * altG - 0.7;
		#if defined(GLOBALSNOW_IGNORE_SURFACE_NORMAL)
			float flatSurface = 1;
		#else
			#if !SHADER_API_D3D9 && defined(GLOBALSNOW_ENABLE_SLOPE_CONTROL)
			float ny = wsNormal.y - _GS_SnowData5.x;
			float flatSurface = saturate( (ny + (altNoise - 0.2) * _GS_SnowData5.z) * _GS_SnowData5.y);
			#else
			float ny = wsNormal.y - 0.7;
			float flatSurface = saturate(ny * 15.0);
			#endif
		#endif

		float minAltitude = saturate( IN.worldPos.y - _GS_SnowData2.x - altNoise * _GS_SnowData2.y);
		snowCover *= minAltitude * flatSurface - zmask.g;

		if (snowCover <= 0) {	
			#if !defined(GLOBALSNOW_IS_NON_SURFACE_SHADER) && !GLOBALSNOW_FLAT_SHADING
			o.Normal = float3(0,0,1);
			#endif
			return;
		}

		const float snowHeight = _GS_SnowData1.x; // Range: 0.001 .. 0.3 or will distort
		float3 rayDir = normalize(_WorldSpaceCameraPos - snowPos);

		// relief
		#if GLOBALSNOW_RELIEF || GLOBALSNOW_OCCLUSION
		float height = 0;
		if (snowCover > 0.1) {
			// 3D surface mapping
			rayDir.y = max(0.3, rayDir.y);
			snowPos.y = 0;
			float3 snowPos0 = snowPos + rayDir * RELIEF_MAX_RAY_LENGTH;
			float3 rayStep = (snowPos - snowPos0) / (float)RELIEF_SAMPLE_COUNT; 
			// locate hit point
			UNITY_UNROLL
			for (int k=0;k<RELIEF_SAMPLE_COUNT;k++) {
				float h1 = tex2Dlod(_GS_NoiseTex, float4(snowPos0.xz * SNOW_TEXTURE_SCALE, 0, 0)).g * snowHeight;	// use tex2Dlod to prevent artifacts due to quantization of step
//				float h1 = tex2D(_GS_NoiseTex, snowPos0.xz * SNOW_TEXTURE_SCALE).g * snowHeight;
				if (h1>snowPos0.y) {
					snowPos = snowPos0;
					snowPos0 -= rayStep;
					break;
				}
				snowPos0 += rayStep;
			}
			// binary search
			UNITY_UNROLL
			for (int j=0;j<RELIEF_BINARY_SAMPLE_COUNT;j++) {
				float3 occ = (snowPos0 + snowPos) * 0.5;
				height = tex2D(_GS_NoiseTex, occ.xz * SNOW_TEXTURE_SCALE).g;	// don't use tex2Dlod to prevent occlusion artifacts - we need an average here
				if (height * snowHeight>occ.y) {
					snowPos = occ;
				} else {
					snowPos0 = occ;
				}
			}

		}
		#if GLOBALSNOW_OCCLUSION
		// fake occlusion (optional)
		float height2 = tex2D(_GS_NoiseTex, (snowPos.xz + normalize(_GS_SunDir.xz) * 0.03) * SNOW_TEXTURE_SCALE).g;
		diff.a = saturate(height-height2);

		#endif
		#endif // RELIEF
	
		// surface normal at point
		#if GLOBALSNOW_FLAT_SHADING
		float3 norm = float3(0,0,1);
		float3 norm1 = norm;
		#else
		float4 texNorm1 = tex2D(_GS_SnowNormalsTex, snowPos.xz * SNOW_TEXTURE_SCALE);
		float3 norm1 = UnpackNormal(texNorm1);
		// perturb normal (optional)
		float4 texNorm2 = tex2D(_GS_SnowNormalsTex, snowPos.xz * SNOW_TEXTURE_SCALE * 64.0);
		float3 norm2 = UnpackNormal(texNorm2);
		float3 norm = normalize(norm1 + norm2);

		// glitter (optional)
		float randomFacet = frac(norm.x * 173.13) - 0.99;
		float normalGloss = saturate(dot(-_GS_SunDir.xyz, norm));
		float glitter = saturate(randomFacet * 55.0) * normalGloss * _GS_SnowData1.z;
		#endif

		#if GLOBALSNOW_TERRAINMARKS	
		float drawDistSqr = dot(IN.worldPos - _WorldSpaceCameraPos, IN.worldPos - _WorldSpaceCameraPos);
		if (drawDistSqr< _GS_SunDir.w) {
			const float decalTexScale = 8.0 * _GS_DecalTex_TexelSize.x; // / 2048.0;
			const float texelScale = 0.1;
			const float2 offs  = float2(-0.5, 0.5);
			float2 uv00  = (snowPos.xz + offs.xx * texelScale) * decalTexScale; 
			float2 uv01  = (snowPos.xz + offs.xy * texelScale) * decalTexScale; 
			float2 uv10  = (snowPos.xz - offs.xy * texelScale) * decalTexScale; 
//			float2 uv00  = (IN.worldPos.xz + offs.xx * texelScale) * decalTexScale; 	/// using IN.worldPos will produce flat marks
//			float2 uv01  = (IN.worldPos.xz + offs.xy * texelScale) * decalTexScale; 
//			float2 uv10  = (IN.worldPos.xz - offs.xy * texelScale) * decalTexScale; 
			float  h00   = tex2D(_GS_DecalTex, uv00).y;
			float  h01   = tex2D(_GS_DecalTex, uv01).y;
			float  h10   = tex2D(_GS_DecalTex, uv10).y;
			float  sh    = h00+h01+h10;
			if (sh>0) {
				#if !GLOBALSNOW_FLAT_SHADING
				float3 dduv00 = normalize(float3(offs.x, h00, offs.x));
				float3 dduv01 = normalize(float3(offs.x, h01, offs.y));
				float3 dduv10 = normalize(float3(offs.y, h10, offs.x));
				float3 dduvA  = (dduv01 - dduv00);
				float3 dduvB  = (dduv10 - dduv00);
				float3 holeNorm = cross(dduvB, dduvA); 
				holeNorm.y *= -1.0;
				norm = lerp(norm, holeNorm.xzy, saturate( sh/1.75));
				norm = normalize(norm);
				#endif // FLAT_SHADING

				diff.rgb -= sh / 38.0; // simulate self shadowing - the value of 38.0 can be decreased / increased to change obscurance
				#if GLOBALSNOW_OCCLUSION
				diff.a = lerp(diff.a, diff.a * 0.5, sh/3.0);
				#endif
			}
		}
		#endif

		#if GLOBALSNOW_FOOTPRINTS		
		snowPos.xz = lerp(snowPos.xz, IN.worldPos.xz, 0.9);	// make footprint less flat
		snowPos.xz *= _GS_SnowData4.xx;
		float4 dd = tex2D(_GS_FootprintTex, snowPos.xz / 2048.0);
		if (dd.y) {
			float2 dduv0 = frac(snowPos.xz) - 0.5.xx;
			float4 dduv  = float4(0,0,0,0);
			// rotate decal hit pos
			dduv.x = dot(dduv0, float2(dd.x, -dd.z));
			dduv.y = dot(dduv0, dd.zx);
			dduv.xy += 0.5.xx;
			float4 dt = tex2Dlod(_GS_DetailTex, dduv);
			if (dt.a) { // hole
				#if GLOBALSNOW_OCCLUSION
				diff.a *= 0.35; // soften occlusion inside plain hole
				#endif
				// 3D hole (optional)
				const int holeSamples = 5;
				const float deepness = 0.02; // (option)
				// rotate raydir				
				dduv0.x = dot(-rayDir.xz, float2(dd.x, -dd.z)); // dduv0.x * dd.x - dduv0.y * dd.z;
				dduv0.y = dot(-rayDir.xz, dd.zx); // dduv0.x * dd.z + dduv0.y * dd.x;
				float4 holeStep = float4(dduv0 * deepness, 0, 0);
				float3 holeNorm = float3(0.0,0.0,1.0); //float3( lerp(norm1, float3(0.0,0.0,1.0), 0.7);
				float holeDeep = 0.5; // (option)
				UNITY_UNROLL
				for (int k=0;k<holeSamples;k++) {
					dt = tex2Dlod(_GS_DetailTex, dduv);
					if (dt.a<=0) { // no hole
						float kh = (float)k / holeSamples;
						#if GLOBALSNOW_OCCLUSION
						diff.a += 0.075 * kh * dd.y;	// add occlusion around edges of hole
						#endif
						holeDeep *= kh;
						holeNorm = rayDir.xzy; // * 0.95;
						break;
					}
					dduv += holeStep;
				}
				diff.rgb *= 1.0 - dd.y * _GS_SnowData4.y;
				norm = lerp(norm, holeNorm, dd.y * holeDeep);
			}
 		}
 		#endif
 		
		// pass color data to output shader
		fixed3 snowAlbedo;
		#if GLOBALSNOW_OCCLUSION
		snowAlbedo = (diff.rgb + glitter.xxx) * _GS_SnowData1.w * saturate (1.0 - diff.a * _GS_SnowData1.y);
		#elif GLOBALSNOW_FLAT_SHADING
		snowAlbedo = diff.rgb * _GS_SnowData1.w;
		#else
		snowAlbedo = (diff.rgb + glitter.xxx) * _GS_SnowData1.w;
		#endif

		#if defined(GLOBALSNOW_IS_NON_SURFACE_SHADER) 
			finalColor.rgb = lerp(finalColor.rgb, snowAlbedo, snowCover);
		#else
			#if !defined(GLOBALSNOW_IS_STANDARD_SHADER)
				_SpecColor.rgb = diff.rgb * 0.025;
				o.Gloss = _GS_SnowData3.x * saturate(0.65 + _GS_SunDir.y);
			#endif
			#if defined(GLOBALSNOW_IS_OVERLAY)
			o.Albedo = snowAlbedo;
			o.Alpha = snowCover;
			#else
			o.Albedo = lerp(o.Albedo, snowAlbedo, snowCover);
			#endif
			#if !GLOBALSNOW_FLAT_SHADING
			o.Normal = norm;
			#endif
		#endif
	}

