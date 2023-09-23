// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7731,x:32894,y:32616,varname:node_7731,prsc:2|diff-5276-OUT,alpha-7325-OUT;n:type:ShaderForge.SFN_Tex2d,id:1608,x:31297,y:32885,ptovrint:False,ptlb:ComparingTexShade1,ptin:_ComparingTexShade1,varname:node_1608,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9820,x:31736,y:33222,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9820,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:19bed23d58d65e247a9810bf6d1ac276,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7946,x:31181,y:33172,ptovrint:False,ptlb:CompareTex1,ptin:_CompareTex1,varname:node_7946,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fb1c6e6939e4e7e44ab92f65682f804f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Max,id:2194,x:32594,y:32852,varname:node_2194,prsc:2|A-6714-OUT,B-3552-OUT,C-9820-RGB;n:type:ShaderForge.SFN_Blend,id:8577,x:31731,y:32769,varname:node_8577,prsc:2,blmd:10,clmp:False|SRC-1608-RGB,DST-7946-RGB;n:type:ShaderForge.SFN_Slider,id:7534,x:32348,y:32156,ptovrint:False,ptlb:Temperature,ptin:_Temperature,varname:node_7534,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:9677,x:32039,y:32122,ptovrint:False,ptlb:positiveTempColor,ptin:_positiveTempColor,varname:node_9677,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.3151408,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:5782,x:31981,y:32303,ptovrint:False,ptlb:negativeTempColor,ptin:_negativeTempColor,varname:node_5782,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5553317,c3:1,c4:1;n:type:ShaderForge.SFN_If,id:2189,x:32389,y:32263,varname:node_2189,prsc:2|A-7534-OUT,B-5804-OUT,GT-9677-RGB,EQ-3314-RGB,LT-5782-RGB;n:type:ShaderForge.SFN_ValueProperty,id:5804,x:32201,y:32103,ptovrint:False,ptlb:const,ptin:_const,varname:node_5804,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Color,id:3314,x:31822,y:32180,ptovrint:False,ptlb:empty_color,ptin:_empty_color,varname:node_3314,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:5276,x:32640,y:32553,varname:node_5276,prsc:2|A-2194-OUT,B-2189-OUT,T-1430-OUT;n:type:ShaderForge.SFN_Abs,id:1430,x:32784,y:32140,varname:node_1430,prsc:2|IN-7534-OUT;n:type:ShaderForge.SFN_Slider,id:7325,x:32532,y:33256,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_7325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Color,id:8283,x:31092,y:32824,ptovrint:False,ptlb:ColorShade1,ptin:_ColorShade1,varname:node_8283,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:897,x:31953,y:32825,varname:node_897,prsc:2|A-8577-OUT,B-8283-RGB,T-5974-OUT;n:type:ShaderForge.SFN_Slider,id:5974,x:31058,y:33076,ptovrint:False,ptlb:ShadeLerp1,ptin:_ShadeLerp1,varname:node_5974,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.384763,max:1;n:type:ShaderForge.SFN_Subtract,id:9146,x:32181,y:32840,varname:node_9146,prsc:2|A-897-OUT,B-9820-A;n:type:ShaderForge.SFN_Tex2d,id:387,x:31039,y:32436,ptovrint:False,ptlb:CompareTex2,ptin:_CompareTex2,varname:node_387,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Slider,id:5205,x:31003,y:32663,ptovrint:False,ptlb:ShadeLerp2,ptin:_ShadeLerp2,varname:node_5205,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:414,x:31158,y:32514,ptovrint:False,ptlb:ColorShade2,ptin:_ColorShade2,varname:node_414,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4087,x:31229,y:32323,ptovrint:False,ptlb:ComparingTexShade2,ptin:_ComparingTexShade2,varname:node_4087,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Blend,id:5647,x:31478,y:32465,varname:node_5647,prsc:2,blmd:10,clmp:False|SRC-4087-RGB,DST-387-RGB;n:type:ShaderForge.SFN_Lerp,id:2370,x:31692,y:32500,varname:node_2370,prsc:2|A-5647-OUT,B-414-RGB,T-5205-OUT;n:type:ShaderForge.SFN_Subtract,id:2954,x:31968,y:32510,varname:node_2954,prsc:2|A-2370-OUT,B-9820-A;n:type:ShaderForge.SFN_SwitchProperty,id:3552,x:32185,y:32545,ptovrint:False,ptlb:EnableCompare2,ptin:_EnableCompare2,varname:node_3552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5804-OUT,B-2954-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6714,x:32206,y:32702,ptovrint:False,ptlb:EnableCompare1,ptin:_EnableCompare1,varname:node_6714,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5804-OUT,B-9146-OUT;proporder:9820-6714-7946-1608-8283-5974-3552-387-4087-414-5205-7325-7534-9677-5782-5804-3314;pass:END;sub:END;*/

Shader "JT/ComparingTexturesShader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [MaterialToggle] _EnableCompare1 ("EnableCompare1", Float ) = 0
        _CompareTex1 ("CompareTex1", 2D) = "white" {}
        _ComparingTexShade1 ("ComparingTexShade1", 2D) = "gray" {}
        _ColorShade1 ("ColorShade1", Color) = (1,0,0,1)
        _ShadeLerp1 ("ShadeLerp1", Range(0, 1)) = 0.384763
        [MaterialToggle] _EnableCompare2 ("EnableCompare2", Float ) = 0
        _CompareTex2 ("CompareTex2", 2D) = "gray" {}
        _ComparingTexShade2 ("ComparingTexShade2", 2D) = "gray" {}
        _ColorShade2 ("ColorShade2", Color) = (0.5,0.5,0.5,1)
        _ShadeLerp2 ("ShadeLerp2", Range(0, 1)) = 0
        _Opacity ("Opacity", Range(0, 1)) = 1
        _Temperature ("Temperature", Range(-1, 1)) = 0
        _positiveTempColor ("positiveTempColor", Color) = (1,0.3151408,0,1)
        _negativeTempColor ("negativeTempColor", Color) = (0,0.5553317,1,1)
        [HideInInspector]_const ("const", Float ) = 0
        [HideInInspector]_empty_color ("empty_color", Color) = (1,1,1,1)
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
            uniform sampler2D _ComparingTexShade1; uniform float4 _ComparingTexShade1_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _CompareTex1; uniform float4 _CompareTex1_ST;
            uniform sampler2D _CompareTex2; uniform float4 _CompareTex2_ST;
            uniform sampler2D _ComparingTexShade2; uniform float4 _ComparingTexShade2_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
                UNITY_DEFINE_INSTANCED_PROP( float4, _ColorShade1)
                UNITY_DEFINE_INSTANCED_PROP( float, _ShadeLerp1)
                UNITY_DEFINE_INSTANCED_PROP( float, _ShadeLerp2)
                UNITY_DEFINE_INSTANCED_PROP( float4, _ColorShade2)
                UNITY_DEFINE_INSTANCED_PROP( fixed, _EnableCompare2)
                UNITY_DEFINE_INSTANCED_PROP( fixed, _EnableCompare1)
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
                float _const_var = UNITY_ACCESS_INSTANCED_PROP( Props, _const );
                float4 _ComparingTexShade1_var = tex2D(_ComparingTexShade1,TRANSFORM_TEX(i.uv0, _ComparingTexShade1));
                float4 _CompareTex1_var = tex2D(_CompareTex1,TRANSFORM_TEX(i.uv0, _CompareTex1));
                float4 _ColorShade1_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ColorShade1 );
                float _ShadeLerp1_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ShadeLerp1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 _EnableCompare1_var = lerp( _const_var, (lerp(( _CompareTex1_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_CompareTex1_var.rgb-0.5))*(1.0-_ComparingTexShade1_var.rgb)) : (2.0*_CompareTex1_var.rgb*_ComparingTexShade1_var.rgb) ),_ColorShade1_var.rgb,_ShadeLerp1_var)-_MainTex_var.a), UNITY_ACCESS_INSTANCED_PROP( Props, _EnableCompare1 ) );
                float4 _ComparingTexShade2_var = tex2D(_ComparingTexShade2,TRANSFORM_TEX(i.uv0, _ComparingTexShade2));
                float4 _CompareTex2_var = tex2D(_CompareTex2,TRANSFORM_TEX(i.uv0, _CompareTex2));
                float4 _ColorShade2_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ColorShade2 );
                float _ShadeLerp2_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ShadeLerp2 );
                float3 _EnableCompare2_var = lerp( _const_var, (lerp(( _CompareTex2_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_CompareTex2_var.rgb-0.5))*(1.0-_ComparingTexShade2_var.rgb)) : (2.0*_CompareTex2_var.rgb*_ComparingTexShade2_var.rgb) ),_ColorShade2_var.rgb,_ShadeLerp2_var)-_MainTex_var.a), UNITY_ACCESS_INSTANCED_PROP( Props, _EnableCompare2 ) );
                float _Temperature_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Temperature );
                float node_2189_if_leA = step(_Temperature_var,_const_var);
                float node_2189_if_leB = step(_const_var,_Temperature_var);
                float4 _negativeTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _negativeTempColor );
                float4 _positiveTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _positiveTempColor );
                float4 _empty_color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _empty_color );
                float3 diffuseColor = lerp(max(max(_EnableCompare1_var,_EnableCompare2_var),_MainTex_var.rgb),lerp((node_2189_if_leA*_negativeTempColor_var.rgb)+(node_2189_if_leB*_positiveTempColor_var.rgb),_empty_color_var.rgb,node_2189_if_leA*node_2189_if_leB),abs(_Temperature_var));
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
            uniform sampler2D _ComparingTexShade1; uniform float4 _ComparingTexShade1_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _CompareTex1; uniform float4 _CompareTex1_ST;
            uniform sampler2D _CompareTex2; uniform float4 _CompareTex2_ST;
            uniform sampler2D _ComparingTexShade2; uniform float4 _ComparingTexShade2_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Temperature)
                UNITY_DEFINE_INSTANCED_PROP( float4, _positiveTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float4, _negativeTempColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _const)
                UNITY_DEFINE_INSTANCED_PROP( float4, _empty_color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
                UNITY_DEFINE_INSTANCED_PROP( float4, _ColorShade1)
                UNITY_DEFINE_INSTANCED_PROP( float, _ShadeLerp1)
                UNITY_DEFINE_INSTANCED_PROP( float, _ShadeLerp2)
                UNITY_DEFINE_INSTANCED_PROP( float4, _ColorShade2)
                UNITY_DEFINE_INSTANCED_PROP( fixed, _EnableCompare2)
                UNITY_DEFINE_INSTANCED_PROP( fixed, _EnableCompare1)
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
                float _const_var = UNITY_ACCESS_INSTANCED_PROP( Props, _const );
                float4 _ComparingTexShade1_var = tex2D(_ComparingTexShade1,TRANSFORM_TEX(i.uv0, _ComparingTexShade1));
                float4 _CompareTex1_var = tex2D(_CompareTex1,TRANSFORM_TEX(i.uv0, _CompareTex1));
                float4 _ColorShade1_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ColorShade1 );
                float _ShadeLerp1_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ShadeLerp1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 _EnableCompare1_var = lerp( _const_var, (lerp(( _CompareTex1_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_CompareTex1_var.rgb-0.5))*(1.0-_ComparingTexShade1_var.rgb)) : (2.0*_CompareTex1_var.rgb*_ComparingTexShade1_var.rgb) ),_ColorShade1_var.rgb,_ShadeLerp1_var)-_MainTex_var.a), UNITY_ACCESS_INSTANCED_PROP( Props, _EnableCompare1 ) );
                float4 _ComparingTexShade2_var = tex2D(_ComparingTexShade2,TRANSFORM_TEX(i.uv0, _ComparingTexShade2));
                float4 _CompareTex2_var = tex2D(_CompareTex2,TRANSFORM_TEX(i.uv0, _CompareTex2));
                float4 _ColorShade2_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ColorShade2 );
                float _ShadeLerp2_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ShadeLerp2 );
                float3 _EnableCompare2_var = lerp( _const_var, (lerp(( _CompareTex2_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_CompareTex2_var.rgb-0.5))*(1.0-_ComparingTexShade2_var.rgb)) : (2.0*_CompareTex2_var.rgb*_ComparingTexShade2_var.rgb) ),_ColorShade2_var.rgb,_ShadeLerp2_var)-_MainTex_var.a), UNITY_ACCESS_INSTANCED_PROP( Props, _EnableCompare2 ) );
                float _Temperature_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Temperature );
                float node_2189_if_leA = step(_Temperature_var,_const_var);
                float node_2189_if_leB = step(_const_var,_Temperature_var);
                float4 _negativeTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _negativeTempColor );
                float4 _positiveTempColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _positiveTempColor );
                float4 _empty_color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _empty_color );
                float3 diffuseColor = lerp(max(max(_EnableCompare1_var,_EnableCompare2_var),_MainTex_var.rgb),lerp((node_2189_if_leA*_negativeTempColor_var.rgb)+(node_2189_if_leB*_positiveTempColor_var.rgb),_empty_color_var.rgb,node_2189_if_leA*node_2189_if_leB),abs(_Temperature_var));
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
