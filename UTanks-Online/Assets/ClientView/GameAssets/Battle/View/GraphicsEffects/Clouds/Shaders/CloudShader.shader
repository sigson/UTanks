Shader "Custom/CloudShader"
{
    Properties
	{
		_Tex("Primary Noise", 2D) = "white" {}
		_Tex2("Secondary Noise", 2D) = "white" {}
		_Range("Primary Cutoff", Range(0,1)) = 0.1
		_Range2("Secondary Cutoff", Range(0,1)) = 0.5
		_DistortStrength("Distort Strength", Range(0,1)) = 0.1
		_Smoothness("Cutoff Smoothness", Range(0,1)) = 0.1
		_SecondLayerStrength("Secondary Layer Strength", Range(0,1)) = 0.5
		_Speed("Speed", Range(0,1)) = 0.2
		_Strength("Strength", Range(0,1)) = 0.2
		[Toggle(MOVE)] _MOVE("Back and forth?", Float) = 0
		[Toggle(INVERT)] _INVERT("Invert", Float) = 0
	}

		SubShader
		{
			Lighting Off
			Blend One Zero

			Pass
		{
			CGPROGRAM
	#include "UnityCustomRenderTexture.cginc"

	#pragma vertex CustomRenderTextureVertexShader
	#pragma fragment frag
	#pragma target 3.0
	#pragma shader_feature MOVE
	#pragma shader_feature INVERT

		sampler2D _Tex, _Tex2;
		float _Range, _Smoothness, _Speed, _Range2, _Strength, _DistortStrength, _SecondLayerStrength;

		float4 frag(v2f_customrendertexture IN) : COLOR
		{
			// scrolling movement controlled by _Speed variable
			float time = 0;
			// move back and forth with a sine wave
#if MOVE
			time = sin(_Time.x * 20)* _Speed;
#else
			time = _Time.x * _Speed;
#endif

			// primary noise texture, only need 1 channel
			float primNoise = tex2D(_Tex, (IN.globalTexcoord.xy * 2) + (time * 0.2)).r;
		
			// secondary noise texture, for more fluid motion, add in the first noise as distortion
			float secNoise = tex2D(_Tex2, (IN.globalTexcoord.xy * 4) + time + (primNoise * _DistortStrength));

			// detail texture to make edges a bit sharper
			float4 detailTex = tex2D(_Tex2, (IN.globalTexcoord.xy * 10)) + primNoise + secNoise;
			
			// first layer combined noise
			float clouds = (primNoise * secNoise)  * 0.5;

			// bigger second layer noise, moving slower 
			float4 layer2 = tex2D(_Tex2, (IN.globalTexcoord.xy) + (time * 0.1) + (primNoise * _DistortStrength));

			// smooth step through the noise
			clouds = smoothstep(_Range, _Range + _Smoothness, clouds * detailTex);

			// second layer clouds, at half strength
			float secondClouds = smoothstep(_Range2, _Range2 + _Smoothness, layer2.r * detailTex) * _SecondLayerStrength;
			// combine both
			clouds = (clouds + secondClouds);
			// clamp values
			clouds = saturate(clouds);
			// optional invert
#if INVERT
			clouds = 1 - clouds;
#endif
			// shadow strength
			clouds *= _Strength;
			return float4(1,1,1,clouds);

			}
			ENDCG
		}
		}
}
