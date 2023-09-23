Shader "ShaderColormap/ColorWarpColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 3.0
			//https://www.shadertoy.com/view/tdG3Rd
            #pragma vertex vert
            #pragma fragment frag
            //#include "noiseSimplex.cginc"

            uniform float _StepSize;

            struct v2f{
                float4 position : SV_POSITION;
            };
           
            v2f vert(float4 v:POSITION) : SV_POSITION {
                v2f o;
                o.position = UnityObjectToClipPos (v);
                return o;
            }

            //fixed4 frag(v2f fragCoord) : SV_Target 
            //{
            //    float time = _Time.y * .5+23.0;
            //    // uv should be the 0-1 uv of texture...
	           // float2 uv = i2.position.xy / _ScreenParams.xy;
    
	           // return float4(colour, 1.0);
                
            //}
			
            float colormap_red(float x) {
                if (x < 0.0) {
                    return 54.0 / 255.0;
                } else if (x < 20049.0 / 82979.0) {
                    return (829.79 * x + 54.51) / 255.0;
                } else {
                    return 1.0;
                }
            }

            float colormap_green(float x) {
                if (x < 20049.0 / 82979.0) {
                    return 0.0;
                } else if (x < 327013.0 / 810990.0) {
                    return (8546482679670.0 / 10875673217.0 * x - 2064961390770.0 / 10875673217.0) / 255.0;
                } else if (x <= 1.0) {
                    return (103806720.0 / 483977.0 * x + 19607415.0 / 483977.0) / 255.0;
                } else {
                    return 1.0;
                }
            }

            float colormap_blue(float x) {
                if (x < 0.0) {
                    return 54.0 / 255.0;
                } else if (x < 7249.0 / 82979.0) {
                    return (829.79 * x + 54.51) / 255.0;
                } else if (x < 20049.0 / 82979.0) {
                    return 127.0 / 255.0;
                } else if (x < 327013.0 / 810990.0) {
                    return (792.02249341361393720147485376583 * x - 64.364790735602331034989206222672) / 255.0;
                } else {
                    return 1.0;
                }
            }

            float4 colormap(float x) {
                return float4(colormap_red(x), colormap_green(x), colormap_blue(x), 1.0);
            }

            // https://iquilezles.org/articles/warp
            /*float noise( in float2 x )
            {
                float2 p = floor(x);
                float2 f = frac(x);
                f = f*f*(3.0-2.0*f);
                float a = textureLod(iChannel0,(p+float2(0.5,0.5))/256.0,0.0).x;
	            float b = textureLod(iChannel0,(p+float2(1.5,0.5))/256.0,0.0).x;
	            float c = textureLod(iChannel0,(p+float2(0.5,1.5))/256.0,0.0).x;
	            float d = textureLod(iChannel0,(p+float2(1.5,1.5))/256.0,0.0).x;
                return lerp(lerp( a, b,f.x), lerp( c, d,f.x),f.y);
            }*/


            float rand(float2 n) { 
                return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
            }

            float noise(float2 p){
                float2 ip = floor(p);
                float2 u = frac(p);
                u = u*u*(3.0-2.0*u);

                float res = lerp(
                    lerp(rand(ip),rand(ip+float2(1.0,0.0)),u.x),
                    lerp(rand(ip+float2(0.0,1.0)),rand(ip+float2(1.0,1.0)),u.x),u.y);
                return res*res;
            }

            const float2x2 mtx = float2x2( 0.80,  0.60, -0.60,  0.80 );

            float fbm( float2 p )
            {
                float f = 0.0;

                f += 0.500000*noise( p + _Time.y  ); p = mul(2.02, mul(p, mtx));
                f += 0.031250*noise( p ); p = mul(2.01, mul(p, mtx));
                f += 0.250000*noise( p ); p = mul(2.03, mul(p, mtx));
                f += 0.125000*noise( p ); p = mul(2.01, mul(p, mtx));
                f += 0.062500*noise( p ); p = mul(2.04, mul(p, mtx));
                f += 0.015625*noise( p + sin(_Time.y) );

                return f/0.96875;
            }

            float pattern( in float2 p )
            {
	            return fbm( p + fbm( p + fbm( p ) ) );
            }

            fixed4 frag(v2f fragCoord) : SV_Target 
            {
                float2 uv = fragCoord.position.xy/_ScreenParams.x;
	            float shade = pattern(uv);
                return float4(colormap(shade).rgb, shade);
            }

            /** SHADERDATA
            {
	            "title": "Base warp fBM",
	            "description": "Noise but Pink",
	            "model": "person"
            }
            */

            ENDCG
            
        }
    }
}