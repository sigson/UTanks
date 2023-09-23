Shader "ShaderColormap/HexagonityColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
        _NoiseTex ("Noise", 2D ) = "white" {}
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            #pragma target 3.0
			//https://www.shadertoy.com/view/Xd2GR3
            #pragma vertex vert
            #pragma fragment frag
            #include "noiseSimplex.cginc"

            uniform float _StepSize;
			//uniform sampler2D _NoiseTex;
			uniform SamplerState _NoiseTex;

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
			

            #define AA 2

            // { 2d cell id, distance to border, distnace to center )
            float4 hexagon( float2 p ) 
            {
	            float2 q = float2( p.x*2.0*0.5773503, p.y + p.x*0.5773503 );
	
	            float2 pi = floor(q);
	            float2 pf = frac(q);

	            float v = fmod(pi.x + pi.y, 3.0);

	            float ca = step(1.0,v);
	            float cb = step(2.0,v);
	            float2  ma = step(pf.xy,pf.yx);
	
                // distance to borders
	            float e = dot( ma, 1.0-pf.yx + ca*(pf.x+pf.y-1.0) + cb*(pf.yx-2.0*pf.xy) );

	            // distance to center	
	            p = float2( q.x + floor(0.5+p.y/1.5), 4.0*p.y/3.0 )*0.5 + 0.5;
	            float f = length( (frac(p) - 0.5)*float2(1.0,0.85) );		
	
	            return float4( pi + ca - cb*ma, e, f );
            }

            float hash1( float2  p ) { float n = dot(p,float2(127.1,311.7) ); return frac(sin(n)*43758.5453); }

            float noise( in float3 x )
            {
                float3 p = floor(x);
                float3 f = frac(x);
	            f = f*f*(3.0-2.0*f);
	            float2 uv = (p.xy+float2(37.0,17.0)*p.z) + f.xy;
	            float2 rg = tex2Dlod( _NoiseTex, float4((uv+0.5)/256.0, 0.0, 0.0) ).xy;
	            return lerp( rg.x, rg.y, f.z );
            }

            fixed4 frag(v2f fragCoord) : SV_Target 
            {
                float3 tot = float3(0.0, 0.0, 0.0);
    
                #if AA>1
                for( int mm=0; mm<AA; mm++ )
                for( int nn=0; nn<AA; nn++ )
                {
                    float2 off = float2(mm,nn)/float(AA);
                    float2 uv = (fragCoord.position+off)/_ScreenParams.xy;
                    float2 pos = (-_ScreenParams.xy + 2.0*(fragCoord.position+off))/_ScreenParams.y;
                #else    
                {
                    float2 uv = fragCoord.position/_ScreenParams.xy;
                    float2 pos = (-_ScreenParams.xy + 2.0*fragCoord.position)/_ScreenParams.y;
                #endif

                    // distort
                    pos *= 1.2 + 0.15*length(pos);

                    // gray
                    float4 h = hexagon(8.0*pos + 0.5*_Time.y);
                    float n = snoise( float3(0.3*h.xy+_Time.y*0.1,_Time.y) ) * 6;
                    float3 col = 0.15 + 0.15*hash1(h.xy+1.2)*float3(1.0, 1.0, 1.0);
                    col *= smoothstep( 0.10, 0.11, h.z );
                    col *= smoothstep( 0.10, 0.11, h.w );
                    col *= 1.0 + 0.15*sin(40.0*h.z);
                    col *= 0.75 + 0.5*h.z*n;

                    // shadow
                    //h = hexagon(6.0*(pos+0.1*float2(-1.3,1.0)) + 0.6*_Time.y);
                    //col *= 1.0-0.8*smoothstep(0.45,0.451,snoise( float3(0.3*h.xy+_Time.y*0.1,0.5*_Time.y) * 4 ));

                    // red
                    h = hexagon(6.0*pos + 0.6*_Time.y);
                    n = snoise( float3(0.3*h.xy+_Time.y*0.1,0.5*_Time.y) ) * 6;
                    float3 colb = 0.9 + 0.8*sin( hash1(h.xy)*1.5 + 2.0 + float3(0.1,0.9,1.1) );
                    colb *= smoothstep( 0.10, 0.11, h.z );
                    colb *= 1.0 + 0.15*sin(40.0*h.z);

                    col = lerp( col, colb, smoothstep(0.45,0.451,n) );
        
                    col *= 2.5/(2.0+col);

                    col *= pow( 16.0*uv.x*(1.0-uv.x)*uv.y*(1.0-uv.y), 0.1 );

                    tot += col;
	            }	
 	            #if AA>1
                tot /= float(AA*AA);
                #endif
        
	            return float4( tot, 1.0 );
            }

            ENDCG
            
        }
    }
}