Shader "ShaderColormap/WaveletColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 3.0
			//https://www.shadertoy.com/view/3ttSzr
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
			
            fixed4 frag(v2f fragCoord) : SV_Target {
                float2 uv =  (2.0 * fragCoord.position - _ScreenParams.xy) / min(_ScreenParams.x, _ScreenParams.y);
   
                for(float i = 1.0; i < 8.0; i++){
                uv.y += i * 0.1 / i * 
                  sin(uv.x * i * i + _Time.y * 3.5) * sin(uv.y * i * i + _Time.y * 3.5);
              }
    
               float3 col;
               col.r  = uv.y - 0.1;
               col.g = uv.y + 0.3;
               col.b = uv.y + 0.95;
    
                return float4(col,1.0);
            }

            ENDCG
            
        }
    }
}