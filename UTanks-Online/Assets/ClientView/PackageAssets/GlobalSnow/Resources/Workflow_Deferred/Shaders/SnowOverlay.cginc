	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "UnityStandardUtils.cginc"
    #include "GlobalSnowDeferredOptions.cginc"

	#define RELIEF_SAMPLE_COUNT 8
	#define RELIEF_BINARY_SAMPLE_COUNT 8
	#define RELIEF_MAX_RAY_LENGTH 0.5
//	#define USE_BLEND_NORMALS_FUNCTION

	uniform sampler2D_float	_CameraDepthTexture;
	uniform sampler2D_float _GS_DeferredExclusionBuffer;
    uniform float4x4 		_ClipToWorld;
    uniform sampler2D _GS_GBuffer0Copy;
    uniform sampler2D _GS_GBuffer1Copy;
    #if GLOBALSNOW_FLAT_SHADING
		#define _GS_GBuffer2Copy _CameraGBufferTexture2
    #endif
    uniform sampler2D _GS_GBuffer2Copy;

	uniform sampler2D _GS_SnowTex;
	uniform float4	  _MainTex_TexelSize;
	uniform float4	  _MainTex_ST;
    uniform sampler2D _GS_SnowNormalsTex;
    uniform sampler2D _GS_NoiseTex;
    uniform sampler2D_float _GS_DepthTexture;
    uniform sampler2D_float _GS_FootprintTex;
    uniform sampler2D _GS_DetailTex;
    uniform sampler2D_float _GS_DecalTex;
    uniform float4    _GS_DecalTex_TexelSize;
	uniform float4 	  _GS_SnowCamPos;
    uniform float4    _GS_SunDir;		// w = terrain mark distance
	uniform float4    _GS_SnowData1;	// x = relief, y = occlusion, z = glitter, w = brightness
	uniform float4    _GS_SnowData2;	// x = minimum altitude for vegetation and trees, y = altitude scattering, z = coverage extension, w = billboard coverage
	uniform float4    _GS_SnowData3;    // x = Sun occlusion, y = sun atten, z = ground coverage, w = grass coverage
	uniform float4    _GS_SnowData4;    // x = footprint scale, y = footprint obscurance, z = snow normals strength, w = minimum altitude for terrain including scattering
	uniform float4    _GS_SnowData5;    // x = slope threshold, y = slope sharpness, z = slope noise, w = noise scale
	uniform float4    _GS_SnowData6;    // x = _Alpha, y = _Smoothness, z = altitude blending, w = snow thickness
	uniform half4    _GS_SnowTint;

	#define SNOW_TEXTURE_SCALE _GS_SnowData5.w
	#define SNOW_NORMALS_STRENGTH _GS_SnowData4.z

    #if !defined(USE_ZENITHAL_DEPTH)
		uniform sampler2D _GS_DepthMask;
		uniform float4 _GS_DepthMaskWorldSize;
    #endif

	uniform float3 _FlickerFreeCamPos;

    struct v2f {
    	float4 pos: SV_POSITION;
    	float2 uv : TEXCOORD0;
    	float3 cameraToFarPlane : TEXCOORD2;
    };


    v2f vert(appdata_img v) {
    	v2f o;
    	o.pos = float4(v.vertex.xy, 0, 1);
		#if UNITY_UV_STARTS_AT_TOP
			o.pos.y *= -1;
		#endif

    	o.uv = TransformStereoScreenSpaceTex((v.vertex + 1.0) * 0.5, 1.0);

		// Clip space X and Y coords
    	float2 clipXY = o.pos.xy / o.pos.w;
               
    	// Position of the far plane in clip space
    	float4 farPlaneClip = float4(clipXY, 1, 1);
               
    	// Homogeneous world position on the far plane
    	farPlaneClip.y *= _ProjectionParams.x;	
		_ClipToWorld = mul(_ClipToWorld, unity_CameraInvProjection);
    	float4 farPlaneWorld4 = mul(_ClipToWorld, farPlaneClip);
               
    	// World position on the far plane
    	float3 farPlaneWorld = farPlaneWorld4.xyz / farPlaneWorld4.w;
               
    	// Vector from the camera to the far plane
    	o.cameraToFarPlane = farPlaneWorld - _WorldSpaceCameraPos + _FlickerFreeCamPos;
    	return o;
    }

    inline float3 getWorldPos(v2f i, float depth01) {
		return i.cameraToFarPlane * depth01 + _WorldSpaceCameraPos;
    }

	// get snow coverage
#if GLOBALSNOW_FLAT_SHADING
	void frag(v2f i, out half4 outAlbedo: SV_Target0, out half4 outSpecular: SV_Target1, out half4 outAmbient : SV_Target2 ) {
#else
	void frag(v2f i, out half4 outAlbedo: SV_Target0, out half4 outSpecular: SV_Target1, out half4 outNormal: SV_Target2, out half4 outAmbient : SV_Target3 ) {
#endif
		
		float depth01 = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv)));
		float depthExclusion = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_GS_DeferredExclusionBuffer, i.uv))) * 0.9999;

		#if GLOBALSNOW_FLAT_SHADING
			float2 uv_flat = i.uv;
		#endif
		i.uv = TransformStereoScreenSpaceTex(i.uv, 1.0);
		
		outSpecular = 0;
		#if !GLOBALSNOW_FLAT_SHADING
			outNormal = 0;
		#endif
		outAmbient = 0;
		outAlbedo = 0;

        #if defined(EXCLUDE_NEAR_SNOW)
            depth01 = max(depth01, NEAR_DISTANCE_SNOW);
        #endif

		if (depthExclusion <= depth01) {
			discard;
			return;
		}

		float3 worldPos = getWorldPos(i, depth01);
		float3 snowPos  = worldPos;

        #if defined(USE_ZENITHAL_DEPTH)
		    float4 st = float4(worldPos.xz - _GS_SnowCamPos.xz, 0, 0);
		    st *= _GS_SnowData2.z;
		    st += 0.5;
            float2 zmask = tex2Dlod(_GS_DepthTexture, st).rg;
            _GS_SnowData6.x += zmask.g;
		    float y = max(_GS_SnowCamPos.y - worldPos.y, 0.001) / _GS_SnowCamPos.w;
		    float zd = min(zmask.r + _GS_SnowData3.z, y);
		    fixed snowCover = saturate( ((zd / y) - 0.9875) * 110.0);
        #else
            float2 maskUV = (worldPos.xz - _GS_DepthMaskWorldSize.yw) / _GS_DepthMaskWorldSize.xz + 0.5.xx;
            fixed snowCover = tex2D(_GS_DepthMask, maskUV).a;
            if (maskUV.x<=0.01 || maskUV.x>=0.99 || maskUV.y<=0.01 || maskUV.y>=0.99) snowCover = 1.0;
        #endif

		// diffuse
		#if GLOBALSNOW_FLAT_SHADING
			fixed4 diff = tex2D(_GS_SnowTex, snowPos.xz * 0.02);
		#else
			fixed4 diff = tex2D(_GS_SnowTex, snowPos.xz * 0.02).gggg;
		#endif

		// get world space normal
		#if GLOBALSNOW_FLAT_SHADING
			float3 wsNormal = tex2D(_GS_GBuffer2Copy, uv_flat).xyz * 2.0 - 1.0;
		#else
			float3 wsNormal = tex2D(_GS_GBuffer2Copy, i.uv).xyz * 2.0 - 1.0;
		#endif

		// prevent snow on walls and below minimum altitude
		float altG = tex2D(_GS_SnowTex, snowPos.xz * 0.5).g;
		float altNoise = diff.r * altG - 0.9;
		#if !SHADER_API_D3D9 && defined(GLOBALSNOW_ENABLE_SLOPE_CONTROL)
			float ny = wsNormal.y - _GS_SnowData5.x;
			float flatSurface = saturate( (ny + altNoise * _GS_SnowData5.z) * _GS_SnowData5.y);
		#else
			float ny = wsNormal.y - 0.7;
			float flatSurface = saturate(ny * 15.0);
		#endif
		float minAltitude = worldPos.y - _GS_SnowData2.w - altNoise * _GS_SnowData2.y;
		minAltitude = saturate(minAltitude / _GS_SnowData6.z);

		snowCover = snowCover * minAltitude * flatSurface - _GS_SnowData6.x;
		if (snowCover <= 0) {	
			discard;
			return;
		}
        
		const float snowHeight = _GS_SnowData1.x; // Range: 0.001 .. 0.3 or will distort
		float3 rayDir = normalize(_WorldSpaceCameraPos - snowPos);

		// relief
		#if GLOBALSNOW_RELIEF || GLOBALSNOW_OCCLUSION
			float height = 0;
			if (snowCover > 0.1) {
				// 3D surface mapping
				snowPos.y = 0;
				float3 snowPos0 = snowPos + float3(rayDir.x, max(0.3, rayDir.y), rayDir.z) * RELIEF_MAX_RAY_LENGTH;
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
				float height2 = tex2D(_GS_NoiseTex, (snowPos.xz + (_GS_SunDir.xz) * 0.03) * SNOW_TEXTURE_SCALE).g;
				diff.a = saturate(height-height2);
			#endif
		#endif // RELIEF
	
		// surface normal at point
		#if GLOBALSNOW_FLAT_SHADING
			float3 norm = float3(0,1,0);
		#else
			float4 texNorm1 = tex2D(_GS_SnowNormalsTex, snowPos.xz * SNOW_TEXTURE_SCALE);
			float3 norm1 = UnpackNormal(texNorm1);
			// perturb normal (optional)
			float4 texNorm2 = tex2D(_GS_SnowNormalsTex, snowPos.xz * SNOW_TEXTURE_SCALE * 64.0);
			float3 norm2 = UnpackNormal(texNorm2);
			#if defined(USE_BLEND_NORMALS_FUNCTION)
				float3 norm = BlendNormals(norm1, norm2); 
			#else
				float3 norm = (norm1 + norm2) * 0.5;
			#endif

			// rotate snow normal in world space
			float3 axis = normalize(float3(norm.y, 0, -norm.x));
			float angle = acos(norm.z);
			float s, c;
			sincos(angle, s, c);
    		float oc = 1.0 - c;
    		float3x3 rot = float3x3(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
                oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);

	        norm = lerp(mul(rot, wsNormal), norm.xzy, _GS_SnowData6.w * wsNormal.y);
			norm = normalize(lerp(wsNormal, norm, snowCover * SNOW_NORMALS_STRENGTH));

			// glitter (optional)
			float randomFacet = frac(norm.x * 173.13) - 0.99;
			float normalGloss = saturate(dot(-_GS_SunDir.xyz, norm));
			float glitter = saturate(randomFacet * 55.0) * normalGloss * _GS_SnowData1.z;
		#endif

		#if GLOBALSNOW_TERRAINMARKS	
			float drawDistSqr = dot(worldPos - _WorldSpaceCameraPos, worldPos - _WorldSpaceCameraPos);
			if (drawDistSqr< _GS_SunDir.w) {
				const float decalTexScale = 8.0 * _GS_DecalTex_TexelSize.x; // / 2048.0;
				const float texelScale = 0.1;
				const float2 offs  = float2(-0.5, 0.5);
				float2 uv00  = (snowPos.xz + offs.xx * texelScale) * decalTexScale; 
				float2 uv01  = (snowPos.xz + offs.xy * texelScale) * decalTexScale; 
				float2 uv10  = (snowPos.xz - offs.xy * texelScale) * decalTexScale; 
		        float4 decal = tex2D(_GS_DecalTex, uv00);
    		    float  h00   = decal.y;
				float  h01   = tex2D(_GS_DecalTex, uv01).y;
				float  h10   = tex2D(_GS_DecalTex, uv10).y;
				float  sh    = h00+h01+h10;
			    sh *= snowCover;
				if (sh>0) {
					#if !GLOBALSNOW_FLAT_SHADING
						float3 dduv00 = normalize(float3(offs.x, h00, offs.x));
					    float3 dduv01 = normalize(float3(offs.x, h01, offs.y));
					    float3 dduv10 = normalize(float3(offs.y, h10, offs.x));
					    float3 dduvA  = (dduv01 - dduv00);
					    float3 dduvB  = (dduv10 - dduv00);
					    float3 holeNorm = cross(dduvB, dduvA); 
					    holeNorm.y *= -0.5;
					    norm = lerp(norm, holeNorm, saturate( sh/1.75));
					    norm = normalize(norm) + 0.00001.xxx;
					#endif // !FLAT_SHADING

					diff.rgb -= sh / 38.0; // simulate self shadowing - the value of 38.0 can be decreased / increased to change obscurance
					#if GLOBALSNOW_OCCLUSION
					    diff.a = lerp(diff.a, diff.a * 0.5, sh/3.0);
					#endif
				}
			}
		#endif

		#if GLOBALSNOW_FOOTPRINTS		
			snowPos.xz = lerp(snowPos.xz, worldPos.xz, 0.9);	// make footprint less flat
			snowPos.xz *= _GS_SnowData4.xx;
			float4 dd = tex2D(_GS_FootprintTex, snowPos.xz / 2048.0);
	        dd.y *= snowCover;
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
					float3 holeNorm = float3(0.0,1.0,0.0);
					float holeDeep = 0.75; // (option)
					UNITY_UNROLL
					for (int k=0;k<holeSamples;k++) {
						dt = tex2Dlod(_GS_DetailTex, dduv);
						if (dt.a<=0) { // no hole
							float kh = (float)k / holeSamples;
							#if GLOBALSNOW_OCCLUSION
								diff.a += 0.075 * kh * dd.y;	// add occlusion around edges of hole
							#endif
							holeDeep *= kh;
							holeNorm = rayDir; // * 0.95;
							break;
						}
						dduv += holeStep;
					}
					diff.rgb *= 1.0 - dd.y * _GS_SnowData4.y;
					norm = normalize(lerp(norm, holeNorm, dd.y * holeDeep));
				}
 			}
 		#endif

		// output g-buffer values
		#if !GLOBALSNOW_FLAT_SHADING
			outNormal = half4(norm * 0.5 + 0.5, 1.0);
		#endif
		
		// Albedo
		fixed3 snowAlbedo;
		#if GLOBALSNOW_OCCLUSION
			snowAlbedo = (diff.rgb * _GS_SnowTint.rgb + glitter.xxx) * (_GS_SnowData1.w * saturate (1.0 - diff.a * _GS_SnowData1.y));
		#elif GLOBALSNOW_FLAT_SHADING
			snowAlbedo = diff.rgb * _GS_SnowTint.rgb * _GS_SnowData1.w;
		#else
			snowAlbedo = (diff.rgb * _GS_SnowTint.rgb + glitter.xxx) * _GS_SnowData1.w;
		#endif

		// Specular
	    float gloss = saturate(0.65 + _GS_SunDir.y);
		half4 newSpecular =  half4(snowAlbedo * gloss, _GS_SnowData6.y) * _GS_SnowData3.x;
		if (snowCover>=1.0) {
			outAlbedo = half4(snowAlbedo, 0.0);
			outSpecular = newSpecular;
		} else {
			outAlbedo = tex2Dlod(_GS_GBuffer0Copy, float4(i.uv, 0, 0));
			outAlbedo = lerp(outAlbedo, half4(snowAlbedo, 0), snowCover);
			outSpecular = tex2Dlod(_GS_GBuffer1Copy, float4(i.uv, 0, 0));
			outSpecular = lerp(outSpecular, newSpecular, snowCover);
		}

		half3 ambientColor = max(0, ShadeSH9(float4(norm, 1)));
		ambientColor = lerp(ambientColor, ambientColor.ggg, _GS_SnowTint.a) * outAlbedo.rgb;
		#if UNITY_HDR_ON
			outAmbient.rgb = ambientColor;
		#else
			outAmbient.rgb = exp2(-ambientColor);
		#endif

	}

