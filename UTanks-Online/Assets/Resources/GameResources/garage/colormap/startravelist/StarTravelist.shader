Shader "ShaderColormap/StarTravelist"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
			//https://www.shadertoy.com/view/XlfGRj
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

            //fixed4 frag(v2f i2) : SV_Target 
            //{
            //    float time = _Time.y * .5+23.0;
            //    // uv should be the 0-1 uv of texture...
	           // float2 uv = i2.position.xy / _ScreenParams.xy;
    
	           // return float4(colour, 1.0);
                
            //}
			#define iterations 17
			#define formuparam 0.53

			#define volsteps 20
			#define stepsize 0.1

			#define zoom   0.800
			#define tile   0.850
			#define speed  0.010 

			#define brightness 0.0015
			#define darkmatter 0.300
			#define distfading 0.730
			#define saturation 0.850


			fixed4 frag(v2f fragCoord) : SV_Target 
			{
				//get coords and direction
				float2 uv=fragCoord.position.xy/_ScreenParams.xy-.5;
				uv.y = mul(uv.y, _ScreenParams.y/_ScreenParams.x);
				float3 dir=float3(uv*zoom,1.);
				float time=_Time.y*speed+.25;

				//mouse rotation
				float a1=.5+1/_ScreenParams.x*2.;//iMouse.x
				float a2=.8+1/_ScreenParams.y*2.;//iMouse.y
				float2x2 rot1=float2x2(cos(a1),sin(a1),-sin(a1),cos(a1));
				float2x2 rot2=float2x2(cos(a2),sin(a2),-sin(a2),cos(a2));
				dir.xz = mul(dir.xz, rot1);
				dir.xy = mul(dir.xy, rot2);
				float3 from=float3(1.,.5,0.5);
				from+=float3(time*2.,time,-2.);
				from.xz = mul(from.xz, rot1);
				from.xy = mul(from.xy, rot2);
	
				//volumetric rendering
				float s=0.1,fade=1.;
				float3 v=float3(0., 0., 0.);
				for (int r=0; r<volsteps; r++) {
					float3 p=from+s*dir*.5;
					p = abs(float3(tile, tile, tile)-fmod(p,float3(tile*2.,tile*2.,tile*2.))); // tiling fold
					float pa,a=pa=0.;
					for (int i=0; i<iterations; i++) { 
						p=abs(p)/dot(p,p)-formuparam; // the magic formula
						a+=abs(length(p)-pa); // absolute sum of average change
						pa=length(p);
					}
					float dm=max(0.,darkmatter-a*a*.001); //dark matter
					a = mul(a, a*a); // add contrast
					if (r>6) fade*=1.-dm; // dark matter, don't render near
					//v+=float3(dm,dm*.5,0.);
					v+=fade;
					v+=float3(s,s*s,s*s*s*s)*a*brightness*fade; // coloring based on distance
					fade = mul(fade, distfading); // distance fading
					s+=_StepSize;
				}
				float lenv = length(v);
				v=lerp(float3(lenv,lenv,lenv),v,saturation); //color adjust
				return float4(v*.01,1.);	
	
			}

            ENDCG
            
        }
    }
}