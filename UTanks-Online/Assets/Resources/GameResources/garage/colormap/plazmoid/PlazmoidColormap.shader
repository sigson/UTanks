Shader "ShaderColormap/PlazmoidColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            #pragma target 3.0
			//https://www.shadertoy.com/view/4djXzz
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

            // various noise functions
            float Hash3d(float3 uv)
            {
                float f = uv.x + uv.y * 37.0 + uv.z * 521.0;
                return frac(cos(f*3.333)*100003.9);
            }
            float mixP(float f0, float f1, float a)
            {
                return lerp(f0, f1, a*a*(3.0-2.0*a));
            }
            const float2 zeroOne = float2(0.0, 1.0);
            float noise(float3 uv)
            {
                float3 fr = frac(uv.xyz);
                float3 fl = floor(uv.xyz);
                float h000 = Hash3d(fl);
                float h100 = Hash3d(fl + zeroOne.yxx);
                float h010 = Hash3d(fl + zeroOne.xyx);
                float h110 = Hash3d(fl + zeroOne.yyx);
                float h001 = Hash3d(fl + zeroOne.xxy);
                float h101 = Hash3d(fl + zeroOne.yxy);
                float h011 = Hash3d(fl + zeroOne.xyy);
                float h111 = Hash3d(fl + zeroOne.yyy);
                return mixP(
                    mixP(mixP(h000, h100, fr.x), mixP(h010, h110, fr.x), fr.y),
                    mixP(mixP(h001, h101, fr.x), mixP(h011, h111, fr.x), fr.y)
                    , fr.z);
            }

            float PI=3.14159265;
            #define saturate(a) clamp(a, 0.0, 1.0)
            // Weird for loop trick so compiler doesn't unroll loop
            // By making the zero a variable instead of a constant, the compiler can't unroll the loop and
            // that speeds up compile times by a lot.
            #define iTimeDelta unity_DeltaTime.x
            #define iFrame ((int)(_Time.y / iTimeDelta))
            #define ZERO_TRICK max(0, -iFrame)

            float Density(float3 p)
            {
                float final = snoise(p*0.16125) * 1.5;
                float other = snoise(p*0.16125 + 1234.567) * 1.5;
                other -= 0.5;
                final -= 0.5;
                final = 0.1/(abs(final*final*other));
                final += 0.5;
                return final*0.0002;
            }

            fixed4 frag(v2f fragCoord) : SV_Target
            {
	            // ---------------- First, set up the camera rays for ray marching ----------------
	            float2 uv = fragCoord.position.xy/_ScreenParams.xy * 2.0 - 1.0;

	            // Camera up vector.
	            float3 camUp=float3(0,1,0); // vuv

	            // Camera lookat.
	            float3 camLookat=float3(0,0.0,0);	// vrp

	            float mx=8.0/_ScreenParams.x*PI*2.0 + _Time.y * 0.01;
	            float my=-8.0/_ScreenParams.y*10.0 + sin(_Time.y * 0.03)*0.2+0.2;//*PI/2.01;
	            float3 camPos=float3(cos(my)*cos(mx),sin(my),cos(my)*sin(mx))*(200.2); 	// prp

	            // Camera setup.
	            float3 camVec=normalize(camLookat - camPos);//vpn
	            float3 sideNorm=normalize(cross(camUp, camVec));	// u
	            float3 upNorm=cross(camVec, sideNorm);//v
	            float3 worldFacing=(camPos + camVec);//vcv
	            float3 worldPix = worldFacing + uv.x * sideNorm * (_ScreenParams.x/_ScreenParams.y) + uv.y * upNorm;//scrCoord
	            float3 relVec = normalize(worldPix - camPos);//scp

	            // --------------------------------------------------------------------------------
	            float t = 0.0;
	            float inc = 0.02;
	            float maxDepth = 15.0;
	            float3 pos = float3(0,0,0);
                float density = 0.0;
	            // ray marching time
                for (int i = ZERO_TRICK; i < 37; i++)	// This is the count of how many times the ray actually marches.
                {
                    if ((t > maxDepth)) break;
                    pos = camPos + relVec * t;
                    float temp = Density(pos);

                    inc = 1.3 + temp*0.05;	// add temp because this makes it look extra crazy!
                    density += temp * inc;
                    t += inc;
                }

	            // --------------------------------------------------------------------------------
	            // Now that we have done our ray marching, let's put some color on this.
	            float3 finalColor = float3(0.01,0.1,1.0)* density*0.2;

	            // output the final color with sqrt for "gamma correction"
	            return float4(sqrt(clamp(finalColor, 0.0, 1.0)),1.0);
            }


            ENDCG
            
        }
    }
}