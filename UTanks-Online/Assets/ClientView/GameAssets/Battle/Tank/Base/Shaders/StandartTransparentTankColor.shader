// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7731,x:32894,y:32616,varname:node_7731,prsc:2|diff-5276-OUT,alpha-7325-OUT;n:type:ShaderForge.SFN_Tex2d,id:1608,x:32013,y:32605,ptovrint:False,ptlb:Colormap,ptin:_Colormap,varname:node_1608,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9c652725a3f3ce47892fde6d9c0b5d2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9820,x:32015,y:32816,ptovrint:False,ptlb:Details,ptin:_Details,varname:node_9820,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f0f90275fefa8fe4890a9d00dfa78f34,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7946,x:32015,y:33014,ptovrint:False,ptlb:Lightmap,ptin:_Lightmap,varname:node_7946,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a2624e3e6279ec04f8ea075b29a633c7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Subtract,id:849,x:32268,y:33014,varname:node_849,prsc:2|A-7946-A,B-9820-A;n:type:ShaderForge.SFN_Subtract,id:619,x:32296,y:32780,varname:node_619,prsc:2|A-7946-RGB,B-9820-A;n:type:ShaderForge.SFN_Subtract,id:7079,x:32493,y:32983,varname:node_7079,prsc:2|A-9820-RGB,B-849-OUT;n:type:ShaderForge.SFN_Max,id:2194,x:32535,y:32736,varname:node_2194,prsc:2|A-8577-OUT,B-7079-OUT;n:type:ShaderForge.SFN_Blend,id:8577,x:32296,y:32576,varname:node_8577,prsc:2,blmd:10,clmp:False|SRC-1608-RGB,DST-619-OUT;n:type:ShaderForge.SFN_Slider,id:7534,x:32225,y:32302,ptovrint:False,ptlb:Temperature,ptin:_Temperature,varname:node_7534,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:9677,x:32034,y:32328,ptovrint:False,ptlb:positiveTempColor,ptin:_positiveTempColor,varname:node_9677,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.3151408,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:5782,x:31907,y:32444,ptovrint:False,ptlb:negativeTempColor,ptin:_negativeTempColor,varname:node_5782,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5553317,c3:1,c4:1;n:type:ShaderForge.SFN_If,id:2189,x:32343,y:32385,varname:node_2189,prsc:2|A-7534-OUT,B-5804-OUT,GT-9677-RGB,EQ-3314-RGB,LT-5782-RGB;n:type:ShaderForge.SFN_ValueProperty,id:5804,x:32121,y:32547,ptovrint:False,ptlb:const,ptin:_const,varname:node_5804,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Color,id:3314,x:32508,y:32500,ptovrint:False,ptlb:empty_color,ptin:_empty_color,varname:node_3314,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:5276,x:32770,y:32487,varname:node_5276,prsc:2|A-2194-OUT,B-2189-OUT,T-1430-OUT;n:type:ShaderForge.SFN_Abs,id:1430,x:32594,y:32306,varname:node_1430,prsc:2|IN-7534-OUT;n:type:ShaderForge.SFN_Slider,id:7325,x:32549,y:32913,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_7325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;proporder:9820-7946-1608-7534-9677-5782-5804-3314-7325;pass:END;sub:END;*/

Shader "JT/DefaultTankTransparentShader" {
    Properties {
        _Details ("Details", 2D) = "white" {}
        _Lightmap ("Lightmap", 2D) = "white" {}
        _Colormap ("Colormap", 2D) = "white" {}
        _Temperature ("Temperature", Range(-1, 1)) = 0
        _positiveTempColor ("positiveTempColor", Color) = (1,0.3151408,0,1)
        _negativeTempColor ("negativeTempColor", Color) = (0,0.5553317,1,1)
        [HideInInspector]_const ("const", Float ) = 0
        [HideInInspector]_empty_color ("empty_color", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0, 1)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Colormap; uniform float4 _Colormap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
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
                float _Opacity_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Opacity );
                fixed4 finalRGBA = fixed4(finalColor,_Opacity_var);
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
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Colormap; uniform float4 _Colormap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
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
                float _Opacity_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Opacity );
                fixed4 finalRGBA = fixed4(finalColor * _Opacity_var,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
