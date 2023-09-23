Shader "ShaderColormap/LavaColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            #pragma target 3.0
			//https://www.shadertoy.com/view/lslXRS
            #pragma vertex vert
            #pragma fragment frag
            #include "noiseSimplex.cginc"

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

			#define time _Time.y*0.1

			float hash21(in float2 n){ return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453); }
			float2x2 makem2(in float theta){float c = cos(theta);float s = sin(theta);return float2x2(c,-s,s,c);}
			//float noise( in float2 x ){return texture(iChannel0, x*.01).x;}

			float2 gradn(float2 p)
			{
				float ep = .09;
				float gradx = snoise(float2(p.x+ep,p.y))-snoise(float2(p.x-ep,p.y));
				float grady = snoise(float2(p.x,p.y+ep))-snoise(float2(p.x,p.y-ep));
				return float2(gradx,grady);
			}

			float flow(in float2 p)
			{
				float z=2.;
				float rz = 0.;
				float2 bp = p;
				for (float i= 1.;i < 7.;i++ )
				{
					//primary flow speed
					p += time*.6;
		
					//secondary flow speed (speed of the perceived flow)
					bp += time*1.9;
		
					//displacement field (try changing time multiplier)
					float2 gr = gradn(i*p*.34+time*1.);
		
					//rotation of the displacement field
					gr = mul(gr, makem2(time*6.-(0.05*p.x+0.03*p.y)*40.));
		
					//displace the system
					p += gr*.5;
		
					//add noise octave
					rz+= (sin(snoise(p)*7.)*0.5+0.5)/z;
		
					//blend factor (blending displaced system with base system)
					//you could call this advection factor (.5 being low, .95 being high)
					p = lerp(bp,p,.77);
		
					//intensity scaling
					z = mul(z, 1.4);
					//octave scaling
					p = mul(p, 2.);
					bp = mul(bp, 1.9);
				}
				return rz;	
			}

			fixed4 frag(v2f fragCoord) : SV_Target 
			{
				float2 p = fragCoord.position.xy / _ScreenParams.xy-0.5;
				p.x = mul(p.x, _ScreenParams.x/_ScreenParams.y);
				p = mul(p, 3.);
				float rz = flow(p);
	
				float3 col = float3(.2,0.07,0.01)/rz;
				col=pow(col,float3(1.4,1.4,1.4));
				return float4(col,1.0);
			}

            ENDCG
            
        }
    }
}