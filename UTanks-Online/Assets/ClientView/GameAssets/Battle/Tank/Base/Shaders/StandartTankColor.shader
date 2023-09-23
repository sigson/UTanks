// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7731,x:32894,y:32616,varname:node_7731,prsc:2|diff-5276-OUT;n:type:ShaderForge.SFN_Tex2d,id:1608,x:32013,y:32605,ptovrint:False,ptlb:Colormap,ptin:_Colormap,varname:node_1608,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9820,x:32015,y:32816,ptovrint:False,ptlb:Details,ptin:_Details,varname:node_9820,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7946,x:32015,y:33014,ptovrint:False,ptlb:Lightmap,ptin:_Lightmap,varname:node_7946,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Subtract,id:849,x:32268,y:33014,varname:node_849,prsc:2|A-7946-A,B-9820-A;n:type:ShaderForge.SFN_Subtract,id:619,x:32296,y:32780,varname:node_619,prsc:2|A-7946-RGB,B-9820-A;n:type:ShaderForge.SFN_Subtract,id:7079,x:32493,y:32983,varname:node_7079,prsc:2|A-9820-RGB,B-849-OUT;n:type:ShaderForge.SFN_Max,id:2194,x:32535,y:32736,varname:node_2194,prsc:2|A-8577-OUT,B-7079-OUT;n:type:ShaderForge.SFN_Blend,id:8577,x:32296,y:32576,varname:node_8577,prsc:2,blmd:10,clmp:False|SRC-1608-RGB,DST-619-OUT;n:type:ShaderForge.SFN_Slider,id:7534,x:32225,y:32302,ptovrint:False,ptlb:Temperature,ptin:_Temperature,varname:node_7534,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:9677,x:32034,y:32328,ptovrint:False,ptlb:positiveTempColor,ptin:_positiveTempColor,varname:node_9677,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.3151408,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:5782,x:31907,y:32444,ptovrint:False,ptlb:negativeTempColor,ptin:_negativeTempColor,varname:node_5782,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5553317,c3:1,c4:1;n:type:ShaderForge.SFN_If,id:2189,x:32343,y:32385,varname:node_2189,prsc:2|A-7534-OUT,B-5804-OUT,GT-9677-RGB,EQ-3314-RGB,LT-5782-RGB;n:type:ShaderForge.SFN_ValueProperty,id:5804,x:32121,y:32547,ptovrint:False,ptlb:const,ptin:_const,varname:node_5804,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Color,id:3314,x:32508,y:32500,ptovrint:False,ptlb:empty_color,ptin:_empty_color,varname:node_3314,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:5276,x:32770,y:32487,varname:node_5276,prsc:2|A-2194-OUT,B-2189-OUT,T-1430-OUT;n:type:ShaderForge.SFN_Abs,id:1430,x:32594,y:32306,varname:node_1430,prsc:2|IN-7534-OUT;proporder:9820-7946-1608-7534-9677-5782-5804-3314;pass:END;sub:END;*/

Shader "JT/DefaultTankShader" {
    Properties {
        _Details ("Details", 2D) = "white" {}
        _Lightmap ("Lightmap", 2D) = "white" {}
        _Colormap ("Colormap", 2D) = "white" {}
        _Temperature ("Temperature", Range(-1, 1)) = 0
        _positiveTempColor ("positiveTempColor", Color) = (1,0.3151408,0,1)
        _negativeTempColor ("negativeTempColor", Color) = (0,0.5553317,1,1)
        [HideInInspector]_const ("const", Float ) = 0
        [HideInInspector]_empty_color ("empty_color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _Colormap; uniform float4 _Colormap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - 0;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float4 _Colormap_var = tex2D(_Colormap,TRANSFORM_TEX(i.uv0, _Colormap));
                float4 _Lightmap_var = tex2D(_Lightmap,TRANSFORM_TEX(i.uv0, _Lightmap));
                float4 _Details_var = tex2D(_Details,TRANSFORM_TEX(i.uv0, _Details));
                float _Temperature_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Temperature );
                float _const_var = UNITY_ACCESS_INSTANCED_PROP( Props, _const );
                float node_2189_if_leA = step(_Temperature_var,_const_var);
                float node_2189_if_leB = step(_const_var,_Temperature_var);
                float4 _negativeTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _negativeTempColor );
                float4 _positiveTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _positiveTempColor );
                float4 _empty_color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _empty_color );
                float3 diffuseColor = lerp(max(( (_Lightmap_var.rgb-_Details_var.a) > 0.5 ? (1.0-(1.0-2.0*((_Lightmap_var.rgb-_Details_var.a)-0.5))*(1.0-_Colormap_var.rgb)) : (2.0*(_Lightmap_var.rgb-_Details_var.a)*_Colormap_var.rgb) ),(_Details_var.rgb-(_Lightmap_var.a-_Details_var.a))),lerp((node_2189_if_leA*_negativeTempColor_var.rgb)+(node_2189_if_leB*_positiveTempColor_var.rgb),_empty_color_var.rgb,node_2189_if_leA*node_2189_if_leB),abs(_Temperature_var));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _Colormap; uniform float4 _Colormap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Colormap_var = tex2D(_Colormap,TRANSFORM_TEX(i.uv0, _Colormap));
                float4 _Lightmap_var = tex2D(_Lightmap,TRANSFORM_TEX(i.uv0, _Lightmap));
                float4 _Details_var = tex2D(_Details,TRANSFORM_TEX(i.uv0, _Details));
                float _Temperature_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Temperature );
                float _const_var = UNITY_ACCESS_INSTANCED_PROP( Props, _const );
                float node_2189_if_leA = step(_Temperature_var,_const_var);
                float node_2189_if_leB = step(_const_var,_Temperature_var);
                float4 _negativeTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _negativeTempColor );
                float4 _positiveTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _positiveTempColor );
                float4 _empty_color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _empty_color );
                float3 diffuseColor = lerp(max(( (_Lightmap_var.rgb-_Details_var.a) > 0.5 ? (1.0-(1.0-2.0*((_Lightmap_var.rgb-_Details_var.a)-0.5))*(1.0-_Colormap_var.rgb)) : (2.0*(_Lightmap_var.rgb-_Details_var.a)*_Colormap_var.rgb) ),(_Details_var.rgb-(_Lightmap_var.a-_Details_var.a))),lerp((node_2189_if_leA*_negativeTempColor_var.rgb)+(node_2189_if_leB*_positiveTempColor_var.rgb),_empty_color_var.rgb,node_2189_if_leA*node_2189_if_leB),abs(_Temperature_var));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _Colormap; uniform float4 _Colormap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID( i );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _Colormap_var = tex2D(_Colormap,TRANSFORM_TEX(i.uv0, _Colormap));
                float4 _Lightmap_var = tex2D(_Lightmap,TRANSFORM_TEX(i.uv0, _Lightmap));
                float4 _Details_var = tex2D(_Details,TRANSFORM_TEX(i.uv0, _Details));
                float _Temperature_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Temperature );
                float _const_var = UNITY_ACCESS_INSTANCED_PROP( Props, _const );
                float node_2189_if_leA = step(_Temperature_var,_const_var);
                float node_2189_if_leB = step(_const_var,_Temperature_var);
                float4 _negativeTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _negativeTempColor );
                float4 _positiveTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _positiveTempColor );
                float4 _empty_color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _empty_color );
                float3 diffColor = lerp(max(( (_Lightmap_var.rgb-_Details_var.a) > 0.5 ? (1.0-(1.0-2.0*((_Lightmap_var.rgb-_Details_var.a)-0.5))*(1.0-_Colormap_var.rgb)) : (2.0*(_Lightmap_var.rgb-_Details_var.a)*_Colormap_var.rgb) ),(_Details_var.rgb-(_Lightmap_var.a-_Details_var.a))),lerp((node_2189_if_leA*_negativeTempColor_var.rgb)+(node_2189_if_leB*_positiveTempColor_var.rgb),_empty_color_var.rgb,node_2189_if_leA*node_2189_if_leB),abs(_Temperature_var));
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
