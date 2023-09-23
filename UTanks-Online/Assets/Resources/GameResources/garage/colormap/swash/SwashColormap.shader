// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShaderColormap/SwashColormap"
{

    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530718
            #define MAX_ITER 5

            struct v2f{
                float4 position : SV_POSITION;
            };
           
            v2f vert(float4 v:POSITION) : SV_POSITION {
                v2f o;
                o.position = UnityObjectToClipPos (v);
                return o;
            }

            fixed4 frag(v2f i2) : SV_Target 
            {
                float time = _Time.y * .5+23.0;
                // uv should be the 0-1 uv of texture...
	            float2 uv = i2.position.xy / _ScreenParams.xy;
    
            #ifdef SHOW_TILING
	            float2 p = fmod(uv*TAU*2.0, TAU)-250.0;
            #else
                float2 p = fmod(uv*TAU, TAU)-250.0;
            #endif
	            float2 i = float2(p);
	            float c = 1.0;
	            float inten = .005;

	            for (int n = 0; n < MAX_ITER; n++) 
	            {
		            float t = time * (1.0 - (3.5 / float(n+1)));
		            i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		            c += 1.0/length(float2(p.x / (sin(i.x+t)/inten),p.y / (cos(i.y+t)/inten)));
	            }
	            c /= float(MAX_ITER);
	            c = 1.17-pow(c, 1.4);
                float colourxx = pow(abs(c), 8.0);
	            float3 colour = float3(colourxx, colourxx, colourxx);
                colour = clamp(colour + float3(0.0, 0.35, 0.5), 0.0, 1.0);

	            #ifdef SHOW_TILING
	            // Flash tile borders...
	            float2 pixel = 2.0 / _ScreenParams.xy;
	            uv = mul(uv, 2.0);
	            float f = floor(fmod(_Time.y*.5, 2.0)); 	// Flash value.
	            float2 first = step(pixel, uv) * f;		   	// Rule out first screen pixels and flash.
	            uv  = step(fract(uv), pixel);				// Add one line of pixels per tile.
	            colour = lerp(colour, float3(1.0, 1.0, 0.0), (uv.x + uv.y) * first.x * first.y); // Yellow line
	            #endif
    
	            return float4(colour, 1.0);
                
            }
            ENDCG
            
        }
    }
}
