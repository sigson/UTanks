Shader "ShaderSkybox/CybercityColormap"
{
    Properties {
        _LightIntensity ("LightIntensity", Float ) = 1
        [Toggle(DIRECTION_MOVE_FORWARD)]
        _DirectionMove ("Direction move forward", Float) = 0
        _LeftFogColor ("LeftFogColor", Color) = (0.1294,0.298,0.3725,1)
        _RightFogColor ("RightFogColor", Color) = (0.1294,0.298,0.3725,1)
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature DIRECTION_MOVE_FORWARD

            #define TAU 6.28318530718
            #define MAX_ITER 5

            //UNITY_INSTANCING_BUFFER_START( Props )
            //    UNITY_DEFINE_INSTANCED_PROP( float, _LightIntensity)
            //    UNITY_DEFINE_INSTANCED_PROP( float4, _LeftFogColor)
            //    UNITY_DEFINE_INSTANCED_PROP( float4, _RightFogColor)
            //UNITY_INSTANCING_BUFFER_END( Props )

            uniform float _LightIntensity;
            uniform float4 _LeftFogColor;
            uniform float4 _RightFogColor;

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

            struct hit{float dist;int mat;};

            float norminf3(float3 p){return max(abs(p.x),max(abs(p.y),abs(p.z)));}
            float norminf2(float2 p){return max(abs(p.x),abs(p.y));}
            float4x4 viewMatrix(float3 eye,float3 center,float3 up){
                // Based on gluLookAt man page
                float3 f=normalize(center-eye);
                float3 s=normalize(cross(f,up));
                float3 u=cross(s,f);
                return float4x4(
                    float4(s,0.),
                    float4(u,0.),
                    float4(-f,0.),
                    float4(0.,0.,0.,1)
                );
            }
            float hash21(float2 p){
                return frac(sin(dot(p,float2(41.558,36.55))+27.1));
            }
            float hash31(float3 p){
                return frac(sin(dot(p,float3(3.5225,8.142,7.356))+4.));
            }
            float2 edge(float2 p){
                if(abs(p.x)>abs(p.y))return float2((p.x<0.)?-1.:1.,0.);
                else return float2(0.,(p.y<0.)?-1.:1.);
            }

            // Ray tracing
            #define MAX_STEPS 250
            #define EPSILON.001
            #define MIN_DIST 0.
            #define MAX_DIST 1000.

            float towerSDF(float3 p,float width,float height){
                float2 p2=float2(norminf2(p.xz),p.y);
                p2.x-=width;
                p2.y-=height;
                return length(max(p2,0.))+min(0.,max(p2.x,p2.y));
            }
            hit scene(float3 p){
                //Scene's SDF
                hit res;
                p.x-=_Time.y;
                p.z-=.5;
    
                float3 fp;fp.xz=frac(p.xz)-.5;fp.y=p.y+2.8;
                float3 ip;ip=floor(p);
                float3 neighbor;neighbor.xz=fp.xz-edge(fp.xz);neighbor.y=fp.y;
    
                float width=.45;
                float max_height=2.;
                float me=towerSDF(fp,width,max_height*hash21(ip.xz));
                float next=towerSDF(neighbor,width,max_height);
    
                float3 fp2=fp;fp2.y=-fp.y+5.6;
                float3 neighbor2=neighbor;neighbor.y = mul(neighbor.y, -1.);
                float me2=towerSDF(fp2,width,max_height*hash21(ip.zx+float2(.1,.5)));
                float next2=towerSDF(neighbor,width,max_height);
    
                res.dist=me;
                res.dist=min(min(me,next),min(me2,next2));
    
                res.mat=1;
                return res;
            }

            hit trace(float3 cam,float3 dir,float start,float end){
                hit res;
    
                float depth=start;int mat=0;
                for(int i=0;i<MAX_STEPS;i++){
                    hit p=scene(cam+depth*dir);
                    if(p.dist<EPSILON){
                        res.dist=depth;res.mat=p.mat;
                        return res;
                    }
                    depth+=p.dist;
                    if(p.dist>=end){
                        res.dist=end;res.mat=0;
                        return res;
                    }
                }
                res.dist=end;res.mat=0;
                return res;
            }

            float3 estimateNormal(float3 p){
                return normalize(float3(scene(float3(p.x+EPSILON,p.y,p.z)).dist-scene(float3(p.x-EPSILON,p.y,p.z)).dist,scene(float3(p.x,p.y+EPSILON,p.z)).dist-scene(float3(p.x,p.y-EPSILON,p.z)).dist,scene(float3(p.x,p.y,p.z+EPSILON)).dist-scene(float3(p.x,p.y,p.z-EPSILON)).dist));
            }

            float3 backgroundColor=float3(.15, .15, .15);
            float3 displayColor(float3 cam,float3 dir,float2 st){
                float3 color;float3 p;int material_id;hit obj;
    
                obj=trace(cam,dir,MIN_DIST,MAX_DIST);
                p=cam+obj.dist*dir;
                material_id=obj.mat;
    
                //float3 bg=lerp(float3(.1294,.298,.3725),float3(.3882,.2392,.3608),(st.x+1.)/2.);//standartColor
                //float3 bg=lerp(float3(.3882,.2392,.3608),float3(.3882,.2392,.3608),(st.x+1.)/2.);
                float3 bg=lerp(float3(_LeftFogColor.x,_LeftFogColor.y,_LeftFogColor.z),float3(_RightFogColor.x,_RightFogColor.y,_RightFogColor.z),(st.x+1.)/2.);
                if(material_id==0){
                    color=bg;
                }else if(material_id==1){
                    float3 p2=p;
                    p2.x-=_Time.y;
                    p2.y/=2.5;
                    float3 color=(1.-abs(estimateNormal(p).y)>EPSILON)?lerp(float3(0., 0., 0.),float3(.9843,.9922,.3882),smoothstep(.9997,.9999,hash31(floor(p2*56.)))):float3(0., 0., 0.);
                    return lerp(bg,color,2.5*pow(1./(obj.dist*obj.dist),.6));
                }
    
                return color;
            }

            fixed4 frag(v2f fragCoord) : SV_Target 
            {
                // fragment
                float lighting = _LightIntensity;
                #ifdef DIRECTION_MOVE_FORWARD
                float direction = 1;
                #else
                float direction = -1;
                #endif
                float fov = 1.5;
                float2 st=(fragCoord.position.xy)/(_ScreenParams.xy);
                st=st*2.-1.;
                st.x = mul(st.x, (_ScreenParams.x * fov)/(_ScreenParams.y));
                float t=_Time.y;
                // ray tracing
                //float3 cam=float3(cos(t),0.,sin(t))*7.+float3(0.,2.,0.);
                float3 cam=float3(5.,0.,0.);
                float3 dir=normalize(float3(st,-1.1));
                float4x4 view=viewMatrix(cam,float3(0., 0., 0.),float3(0.,1.,0.));
                dir=(mul(view, float4(dir,0.))).xyz * direction;
    
                // shading
                float3 color=displayColor(cam,dir,st);
    
                return float4(color,1.) * lighting;
            }
            ENDCG
            
        }
    }
}