// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShaderColormap/LiquidColormap"
{

    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #define PI 3.141592654
            float2 rot(float2 p,float a)
            {
                float c=sin(a*35.83);
                float s=cos(a*35.83);
                return mul(float2x2(s,c,c,-s), p);
            }

            struct v2f{
                float4 position : SV_POSITION;
            };

            v2f vert(float4 v:POSITION) : SV_POSITION {
                v2f o;
                o.position = UnityObjectToClipPos (v);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv= -1.0 + 2.0*i.position.xy/ _ScreenParams.xy;
                uv=float2(.125,.75)+(uv-float2(.125,.5))*.003;
                float T=_Time.y*.1;

                float4 c = clamp(1.-.7*float4(
                    length(uv-float2(1.1,1)),
                    length(uv-float2(1.1,1)),
                    length(uv-float2(1.1,1)), 0
                    ),0.,1.)*2.-1.;
                float4 c0=float4(0,0,0, 0);
                float w0=0.;
                const float N=5.;
                for(float i=0.;i<N;i++)
                {
                    float wt=(i*i/N/N-.2)*.3;
                    float wp=0.5+(i+1.)*(i+1.5)*0.000001;
                    float wb=.05+i/N*0.1;
                    c.zx=rot(c.zx,1.6+T*0.65*wt+(uv.x+.7)*23.*wp);
                    c.xy=rot(c.xy,c.z*c.x*wb+1.7+T*wt+(uv.y+1.1)*15.*wp);
                    c.yz=rot(c.yz,c.x*c.y*wb+2.4-T*0.79*wt+(uv.x+uv.y*(frac(i/2.)-0.25)*4.)*17.*wp);
                    c.zx=rot(c.zx,c.y*c.z*wb+1.6-T*0.65*wt+(uv.x+.7)*23.*wp);
                    c.xy=rot(c.xy,c.z*c.x*wb+1.7-T*wt+(uv.y+1.1)*15.*wp);
                    float w=(1.5-i/N);
                    c0+=c*w;
                    w0+=w;
                }
                c0=c0/w0*2.+.5;//*(1.-pow(uv.y-.5,2.)*2.)*2.+.5;
                c0*=.5+dot(c0,float4(1,1,1,0))/sqrt(3.)*.5;
                c0+=pow(length(sin(c0*PI*4.))/sqrt(3.)*1.0,20.)*(.3+.7*c0);
                //float4 o=float4(c0,1.0, 0, 0);
                float4 o=c0;
                return o;
                
            }
            ENDCG
            
        }
    }
}
