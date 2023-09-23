Shader "ShaderColormap/TVColormap"
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

            #define pi 3.14159

            float thc(float a, float b) {
                return tanh(a * cos(b)) / tanh(a);
            }

            float ths(float a, float b) {
                return tanh(a * sin(b)) / tanh(a);
            }

            float2 thc(float a, float2 b) {
                return tanh(a * cos(b)) / tanh(a);
            }

            float2 ths(float a, float2 b) {
                return tanh(a * sin(b)) / tanh(a);
            }

            float3 pal( in float t, in float3 a, in float3 b, in float3 c, in float3 d )
            {
                return a + b*cos( 6.28318*(c*t+d) );
            }

            float h21 (float2 a) {
                return frac(sin(dot(a.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float mlength(float2 uv) {
                return max(abs(uv.x), abs(uv.y));
            }

            fixed4 frag(v2f fragCoord) : SV_Target
            {
                float2 uv = (fragCoord.position.xy-0.5*_ScreenParams.xy)/_ScreenParams.y;
                uv -= 0.2 * float2(cos(0.2 * _Time.y), sin(0.2 * _Time.y));
    
                float a = atan2(uv.x, uv.y);
                float r = length(uv);

                float sc = 12. + 1. * cos(10. * uv.x + _Time.y);
                float2 ipos = floor(sc * uv) + 0.5;
                float2 fpos = frac(sc * uv) - 0.5;
    
                float v = h21(ipos);
                float t = 11. * v + _Time.y;
                float2 p = cos(t) * 0.2 * float2(cos(2. * v * t), sin(2. * (1.-v) * t));
                float d = mlength(fpos - p);
                float k = 0.5 + 0.4 * cos(t);
                float s = smoothstep(-k,k, 0.25 + 0.25 * thc(4., 20. * v + _Time.y) - d);
                s *= 2. * s;
                float3 col = step(d, 0.45) * pal(1. * mlength(uv) + 0.08 * frac(s + atan2(fpos.x, fpos.y)/pi + t) - 0.2 * t, float3(0.6, 0.6, 0.6), float3(0.6, 0.6, 0.6), float3(1., 1., 1.), 0.22 * (1. + cos(ceil(4. * v) * s + t)) * float3(0.,0.33,0.66));
    
                // Output to screen
                return float4(col,1.0);
            }
            ENDCG
            
        }
    }
}