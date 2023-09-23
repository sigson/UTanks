Shader "ShaderColormap/ChunkerColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
			//https://www.shadertoy.com/view/fty3Dd
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

            /* 
            This Voronoi Shader is based on:

            1. An article by IQ: 
                https://iquilezles.org/articles/voronoilines
            2. tomkh's drawing helped it click: 
                https://www.shadertoy.com/view/llG3zy
            3.Shane's Rounded border shader::
                https://www.shadertoy.com/view/ll3GRM
            */


            #define pi acos(-1.)
            #define eps 8./_ScreenParams.y

            #define maxPoints 16.
            #define screenSize 4.

            #define bg (0.5+0.5*sin(float3(_Time.y,_Time.y*3.+403.,_Time.y*1.3+902.)))
            #define fg (0.5+0.5*sin(float3(-_Time.y*2.1,-_Time.y*0.9+403.,-_Time.y+902.)))

            float2 rnd2(float2 id){
                return float2(frac(sin(dot(id,float2(14.27,74.97)))*54329.34),
                       frac(sin(dot(id+912.35,float2(49.27,102.74)))*54329.34));
            }

            float2x2 rot(float a){
                float s = sin(a), c = cos(a);
                return float2x2(c,-s,s,c);
            }

            float smin2(float a, float b, float r)
            {
               float f = max(0., 1. - abs(b - a)/r);
               return min(a, b) - r*.25*f*f;
            }

            //float point(float2 uv, float r){
            //    return smoothstep(r+eps,r-eps,length(uv));
            //}

            float ring(float2 uv, float r){
                return smoothstep(eps+0.03, 0., abs(length(uv)-r+0.03));
            }

            //float line(float2 P, float2 A, float2 B, float r){
            //    float2 PA = P-A;
            //    float2 AB = B-A;
            //    //dot(AB,P-P3) = 0
            //    //dot(AB,P-AB*t)
            //    float t = clamp(dot(PA,AB)/dot(AB,AB),0.,1.);
            //    return smoothstep(r+eps,r-eps,length(PA-AB*t));
   
            //}

            //we are in "not world/object space" 
            //because we use length on vectors from float2(0,0.)
            //to get distances
            float4 voronoi(float2 uv){

                float2 st = frac(uv);
                float2 stFL = floor(uv);
                float2 d = float2(10., 10.);
                float2 A, B=float2(100., 100.);
    
                float2 mind;
    
                for(float i = -1.; i <= 1.; i++){
                    for(float j = -1.; j <= 1.; j++){
        
                    float2 id = float2(i,j);
                    float2 rndShift = rnd2(stFL+id);
                    float2 coords = id + 0.5+0.35*sin(pi*2.*(rndShift)+_Time.y) - st;
        
                    float c = length(coords.xy);//max(abs(coords.x),abs(coords.y));
        
                    if(c < d.x){
                        d.x = c;
                        d.y = rnd2(stFL+id).y;
                        A = coords;
                        }
                    }
                }
                mind = d;
    
                d = float2(10., 10.);
    
                for(float i = -1.; i <= 1.; i++){
                    for(float j = -1.; j <= 1.; j++){
        
                    float2 id = float2(i,j);
                    float2 rndShift = rnd2(stFL+id);
                    float2 coords = id + 0.5+0.35*sin(pi*2.*(rndShift)+_Time.y) - st;
        
                    float c = length(coords.xy);//max(abs(coords.x),abs(coords.y));
        
                    if(length(A-coords) > 0.00){
                        //d.x = c;
                        //d.y = rnd2(stFL+id).y;
                        B.x = smin2(B.x, dot( 0.5*(A+coords), 
                                   normalize(coords-A) ), 0.03 );
                        }
                    }
                }

                return float4(mind,B);
    
            }

            fixed4 frag(v2f fragCoord) : SV_Target 
            {
                // Normalized pixel coordinates (from 0 to 1)
                float2 uv = (fragCoord.position.xy*2.-_ScreenParams.xy)/_ScreenParams.y;

                float3 light = float3(1.,0.,2.);
                float3 ldir = normalize(float3(0., 0., 0.)-light);
    
                float3 light2 = float3(2.,1.,1.);
                float3 ldir2 = normalize(float3(0., 0., 0.)-light2);
    
                // Time varying pixel color
                //float3(0.1,1.4,2.)
                uv = uv * 4.;
                float4 voronoXY = voronoi(uv);
                //float edges = smoothstep(0.00,0.01,abs(voronoXY.x-voronoXY.z));
                float3 col = 0.5+0.5*cos(float3(0.4,1.2,1.6) + voronoXY.y*pi*2.);
                //col = lerp(col, float3(0.), smoothstep(0.06,0.05,voronoXY.x));
              //  col += sin(voronoXY.x*40.);
                //col += float3(frac(voronoXY.x*8.));
                //col = lerp(col, float3(0.), smoothstep(0.14,0.13,voronoXY.z));
               //col = lerp(col, float3(1.), smoothstep(0.05,0.,voronoXY.z));
                col -= sin(voronoXY.z*90.)/10.;
                col += pow(voronoXY.z,2.)*1.;
                //col = lerp(col, float3(0.), 1.-smoothstep(0.5,0.4,voronoXY.x*1.));
                col = lerp(col, float3(.9,0.6,0.0), smoothstep(0.3,.5,voronoXY.z)*0.3 );
                // Output to screen
    
                float3 n = float3(
                              voronoi(uv-float2(eps,0.)).x-voronoi(uv+float2(eps,0.)).x,
                              voronoi(uv-float2(eps,0.).yx).x-voronoi(uv+float2(eps,0.).yx).x,
                              voronoi(uv-float2(eps,0.)).z-voronoi(uv+float2(eps,0.)).z
                              );
                     n = normalize(n);//smoothstep(float3(-1.),float3(1.),;
         
                float diff = max(dot(ldir,n),0.);
    
                float spec = pow( max(
                             dot( reflect(-ldir,n),float3(0.,0.,1.)),0.),5.);
                col += diff*0.6+float3(0.9,0.5,0.1)*spec;
    
    
    
                float diff2 = max(dot(ldir,n),0.);
    
                float spec2 = pow( max(
                             dot( reflect(-ldir2,n),float3(0.,0.,1.)),0.),5.);
                col += diff2*0.8+float3(0.1,0.5,0.9)*spec2;
    
                col = lerp(col, float3(0., 0., 0.), smoothstep(0.1,0.095,voronoXY.z));
                col = lerp(col, float3(1.,0.6,0.)/2., smoothstep(0.02,0.01,voronoXY.z));
    
                col /= 1.6;
                col= pow(col, float3(1.4,1.4,1.4));

                return float4(col,1.0);
            }

            ENDCG
            
        }
    }
}