Shader "ShaderColormap/MicroelectronicColormap"
{
    Properties {
        _StepSize ("StepSize", Range(0,1) ) = 0.1
    }
    SubShader {
   
        Pass {

            CGPROGRAM
            //#pragma target 2.0
			//https://www.shadertoy.com/view/7tcXRn
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

            #define PI 3.1415926
            #define INVERT true


            // from https://www.shadertoy.com/view/4djSRW
            float hash(float2 p)
            {
	            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float2 getRandomVector(float seed) {
                float2 outVec;
                if (seed < .25) {
                    outVec = float2(1., 1.);
                } else if (seed < .5) {
                    outVec = float2(-1., 1.);
                } else if (seed < .75) {
                    outVec = float2(1., -1.);
                } else {
                    outVec = float2(-1., -1.);
                }
    
                return outVec;
            }

            float biLerp(float f0, float f1, float f2, float f3, float lerpX, float lerpY) {
                float upper = lerp(f1, f2, lerpX);
                float lower = lerp(f0, f3, lerpX);
    
                return lerp(lower, upper, lerpY);
            }

            float2x2 createRotationMatrix(float rotation) {
                return float2x2(
                    cos(rotation), -sin(rotation),
                    sin(rotation), cos(rotation)
                );
            }

            float getModifiedDot(float2 uv, float2 p, float gridDimension, float pHash) {
                float rotation = sin(_Time.y * .15 + pHash) * 2. * PI;
                if (pHash < .5) {
                    rotation *= -1.;
                }
                float2x2 rotationMatrix = createRotationMatrix(rotation);
    
                return dot((uv - p) / gridDimension, mul(getRandomVector(pHash), rotationMatrix));
            }


            float getPerlinValue(float2 uv, float gridDimension, bool hideLines) {
                float xCoord = floor(uv.x / gridDimension) * gridDimension;
                float yCoord = floor(uv.y / gridDimension) * gridDimension;
    
                float xIndex = floor(uv.x / gridDimension);
                float yIndex = floor(uv.y / gridDimension);
    
                float p0Hash = hash(float2(xIndex, yIndex));
                float p1Hash = hash(float2(xIndex, yIndex + 1.));
                float p2Hash = hash(float2(xIndex + 1., yIndex + 1.));
                float p3Hash = hash(float2(xIndex + 1., yIndex));
    
                float2 p0 = float2(xCoord, yCoord);
                float2 p1 = float2(xCoord, yCoord + gridDimension);
                float2 p2 = float2(xCoord + gridDimension, yCoord + gridDimension);
                float2 p3 = float2(xCoord + gridDimension, yCoord);
    
                float rotation = sin(_Time.y * .15) * 2. * PI;
                float2x2 rotationMatrix = createRotationMatrix(rotation);
    
                float dot0 = getModifiedDot(uv, p0, gridDimension, p0Hash);
                float dot1 = getModifiedDot(uv, p1, gridDimension, p1Hash);
                float dot2 = getModifiedDot(uv, p2, gridDimension, p2Hash);
                float dot3 = getModifiedDot(uv, p3, gridDimension, p3Hash);
    
                float xInterp = smoothstep(p0.x, p2.x, uv.x);
                float yInterp = smoothstep(p0.y, p2.y, uv.y);
    
                float val = biLerp(dot0, dot1, dot2, dot3, xInterp, yInterp);

                float xLerp = fmod(uv.x / 2., gridDimension);
                float revealMargin = gridDimension * .95;

                if (hideLines || xLerp < revealMargin) {
                    return abs(val);
                } else {
                    float marginLerp = (xLerp - revealMargin) / (1. - revealMargin);
                    float distFromCenterMargin = abs(.5 - marginLerp);
                    float marginSmooth = smoothstep(0.499, .5, distFromCenterMargin);
                    return lerp(clamp(val, 0., 1.), abs(val), marginSmooth);
                }
            }

            fixed4 frag(v2f fragCoord) : SV_Target
            {
                float2 uv = fragCoord.position.xy/_ScreenParams.xy;
                float aspectRatio = _ScreenParams.x / _ScreenParams.y;
                uv.x *= aspectRatio;

                float timeScale = .08;
    
                uv += float2(sin(_Time.y * timeScale), _Time.y * timeScale);
    
                float3 background = float3(0.2, 0., 0.);
                float3 color1 = float3(1., 0.75, .25);
                float3 color2 = float3(1., 1., 1.);
                float3 color3 = float3(2.5, 2.5, 2.5);
    
                float gridSize = .15;
    
                float perlinVal1 = pow(getPerlinValue(uv, gridSize, false), .15);
                float perlinVal2 = log(getPerlinValue(uv, gridSize, false)) / 4.;
                float perlinVal3 = pow(getPerlinValue(uv, gridSize * 5., true), 2.);
    
                float3 color = lerp(lerp(lerp(background, color1, perlinVal1), color2, perlinVal2), color3, perlinVal3 * .7);
    
                if (INVERT) {
                    return float4(1. - clamp(color.r, 0., 1.), 1. - clamp(color.g, 0., 1.), 1. - clamp(color.b, 0., 1.), 1.);
                } else {
                    return float4(float3(color), 1.);
                }
    
                
            }

            ENDCG
            
        }
    }
}