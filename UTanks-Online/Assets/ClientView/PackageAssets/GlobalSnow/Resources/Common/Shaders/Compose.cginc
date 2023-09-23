	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	float4 _MainTex_ST;
	sampler2D _SnowedScene;
	sampler2D _SnowedScene2;
	sampler2D _DistantSnow;
	sampler2D _FrostTex;
	sampler2D _FrostNormals;
	float3 _FrostIntensity;
	fixed4 _FrostTintColor;

	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
		float3 uv2: TEXCOORD1;
	};
	
            
	v2f vert( appdata_base v ) {
	    v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		o.uv2 = float3(v.texcoord.xy, 1);

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0) {
			o.uv2 = float3(v.texcoord.x, 1.0 - v.texcoord.y, -1);
		}
		#endif
    	return o;
	}
	
	fixed4 frag(v2f i) : SV_Target {
		float4 uv = float4(i.uv, 0, 0);
		float2 uv2 = i.uv2.xy;
		float2 da = i.uv - UnityStereoScreenSpaceUVAdjust(0.5.xx, _MainTex_ST);
		float dd = dot(da, da) * 2.0;
		dd = saturate(pow(dd, _FrostIntensity.y)) * _FrostIntensity.x;
		fixed4 frost = fixed4(0,0,0,0);
		if (_FrostIntensity.x>0) {
			frost = tex2Dlod(_FrostTex, uv);
			#if UNITY_COLORSPACE_GAMMA
				frost.rgb = GammaToLinearSpace(frost.rgb);
			#endif
			fixed4 norm = tex2Dlod(_FrostNormals, uv);
			norm.rgb = UnpackNormal(norm);
			float2 disp = norm.xy * _FrostIntensity.z * dd;
			uv.xy += disp;
			uv2 += float2(disp.x, disp.y * i.uv2.z);
		}

		fixed4 pixel = tex2D(_MainTex, uv.xy);
		#if !NO_SNOW
		#if JUST_DISTANT_SNOW
			fixed4 snow = tex2D(_DistantSnow, uv.xy);
		#else
			#if FORCE_STEREO_RENDERING
				fixed4 snow;
				if (unity_StereoEyeIndex == 1) {
					snow = tex2D(_SnowedScene2, uv2.xy);
				} else {
					snow = tex2D(_SnowedScene, uv2.xy);
				}
			#else
				fixed4 snow = tex2D(_SnowedScene, uv2.xy);
			#endif
			#ifdef DISTANT_SNOW
				fixed4 distantSnow = tex2D(_DistantSnow, uv.xy);
				snow.rgb = lerp(snow.rgb, distantSnow.rgb, distantSnow.a);
				snow.a   = max(snow.a, distantSnow.a); 
			#endif
		#endif
		pixel.rgb = lerp(pixel.rgb, snow.rgb, snow.a);
		#endif
		#if !NO_FROST
        frost.rgb *= dd;
        pixel.rgb = frost.rgb * _FrostTintColor.rgb + pixel.rgb * (1.0 - frost.g);
		#endif
		return pixel; 
	}
	
	fixed4 fragDebug(v2f i) : SV_Target {
		#if JUST_DISTANT_SNOW
			fixed4 snow = tex2D(_DistantSnow, i.uv2.xy);
		#else
			#if FORCE_STEREO_RENDERING
				fixed4 snow;
				if (unity_StereoEyeIndex == 1) {
					snow = tex2D(_SnowedScene2, i.uv2.xy);
				} else {
					snow = tex2D(_SnowedScene, i.uv2.xy);
				}
			#else
				fixed4 snow = tex2D(_SnowedScene, i.uv2.xy);
			#endif	
			#ifdef DISTANT_SNOW
				fixed4 distantSnow = tex2D(_DistantSnow, i.uv2.xy);
				snow.rgb = lerp(snow.rgb, distantSnow.rgb, distantSnow.a);
				snow.a   = max(snow.a, distantSnow.a); 
			#endif
		#endif
		return snow * snow.a;
	}
