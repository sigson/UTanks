Shader "ShaderColormap/CubsatColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
			//https://www.shadertoy.com/view/XsBfRW
            #pragma vertex vert
            #pragma fragment frag

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

            fixed4 frag(v2f fragCoord) : SV_Target 
            {
                float aspect = _ScreenParams.y/_ScreenParams.x;
                float value;
	            float2 uv = fragCoord.position.xy / _ScreenParams.x;
                uv -= float2(0.5, 0.5*aspect);
                float rot = radians(45.0); // radians(45.0*sin(_Time.y));
                float2x2 m = float2x2(cos(rot), -sin(rot), sin(rot), cos(rot));
   	            uv  = mul(m, uv);
                uv += float2(0.5, 0.5*aspect);
                uv.y+=0.5*(1.0-aspect);
                float2 pos = 10.0*uv;
                float2 rep = frac(pos);
                float dist = 2.0*min(min(rep.x, 1.0-rep.x), min(rep.y, 1.0-rep.y));
                float squareDist = length((floor(pos)+float2(0.5, 0.5)) - float2(5.0, 5.0) );
    
                float edge = sin(_Time.y-squareDist*0.5)*0.5+0.5;
    
                edge = (_Time.y-squareDist*0.5)*0.5;
                edge = 2.0*frac(edge*0.5);
                //value = 2.0*abs(dist-0.5);
                //value = pow(dist, 2.0);
                value = frac (dist*2.0);
                value = lerp(value, 1.0-value, step(1.0, edge));
                //value *= 1.0-0.5*edge;
                edge = pow(abs(1.0-edge), 2.0);
    
                //edge = abs(1.0-edge);
                value = smoothstep( edge-0.05, edge, 0.95*value);
    
    
                value += squareDist*.1;
                //fragColor = float4(value);
                float4 fragColor = lerp(float4(1.0,1.0,1.0,1.0),float4(0.5,0.75,1.0,1.0), value);
                fragColor.a = 0.25*clamp(value, 0.0, 1.0);
                return fragColor;
            }

            ENDCG
            
        }
    }
}