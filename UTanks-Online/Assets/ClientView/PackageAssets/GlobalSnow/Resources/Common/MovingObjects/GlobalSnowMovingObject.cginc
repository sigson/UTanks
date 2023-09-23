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
    uniform float4    _GS_SunDir;		// w = terrain mark distance
	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData3;    // x = Sun occlusion, y = sun atten, z = ground coverage, w = grass coverage

    uniform float _SnowCoverage, _Scatter, _SnowScale;
    uniform half3 _Color, _SnowTint;
    uniform float _SlopeThreshold, _SlopeNoise, _SlopeSharpness;

	#ifndef GLOBALSNOW_INPUT_STRUCTURE_INCLUDED
	#define GLOBALSNOW_INPUT_STRUCTURE_INCLUDED
	struct Input {
        float2 uv_MainTex;
        float3 wpos;
		#if GLOBALSNOW_FLAT_SHADING
			float3 worldNormal;
		#else
    		float3 worldNormal; INTERNAL_DATA
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
        fixed snowCover = _SnowCoverage;
        float3 snowPos = IN.wpos;
		snowPos.xz *= _SnowScale;
		
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
			    float ny = wsNormal.y - _SlopeThreshold;
			    float flatSurface = saturate( (ny + (altNoise - 0.2) * _SlopeNoise) * (5.0 + _SlopeSharpness));
			#else
			    float ny = wsNormal.y - 0.7;
			    float flatSurface = saturate(ny * 15.0);
			#endif
		#endif

		float minAltitude = saturate( 1.0 - altNoise * _Scatter);
		snowCover *= minAltitude * flatSurface;

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

		// pass color data to output shader
		fixed3 snowAlbedo;
		#if GLOBALSNOW_OCCLUSION
		    snowAlbedo = (diff.rgb + glitter.xxx) * _GS_SnowData1.w * saturate (1.0 - diff.a * _GS_SnowData1.y);
		#elif GLOBALSNOW_FLAT_SHADING
		    snowAlbedo = diff.rgb * _GS_SnowData1.w;
		#else
		    snowAlbedo = (diff.rgb + glitter.xxx) * _GS_SnowData1.w;
		#endif
        snowAlbedo *= _SnowTint;

		#if defined(GLOBALSNOW_IS_NON_SURFACE_SHADER) 
			finalColor.rgb = lerp(finalColor.rgb, snowAlbedo, snowCover);
		#else
			#if !defined(GLOBALSNOW_IS_STANDARD_SHADER)
				_SpecColor.rgb = diff.rgb * 0.025;
				o.Gloss = _GS_SnowData3.x * saturate(0.65 + _GS_SunDir.y);
			#endif
    	    o.Albedo = snowAlbedo;
		    o.Alpha = snowCover;
			#if !GLOBALSNOW_FLAT_SHADING
			    o.Normal = norm;
			#endif
		#endif
	}

