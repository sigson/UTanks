// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33352,y:32797,varname:node_4013,prsc:2|diff-4093-OUT;n:type:ShaderForge.SFN_Tex2d,id:5075,x:32467,y:32962,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_5075,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e9c652725a3f3ce47892fde6d9c0b5d2,ntxv:0,isnm:False|UVIN-9291-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7200,x:32453,y:33141,ptovrint:False,ptlb:Lightmap,ptin:_Lightmap,varname:node_7200,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:59442b06eb5111c4c9b13effeee025e2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7277,x:32434,y:32771,ptovrint:False,ptlb:Details,ptin:_Details,varname:node_7277,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f9dd3bcff07a2ec4b9560db62d54e3c1,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalBlend,id:9655,x:32810,y:32766,varname:node_9655,prsc:2|BSE-7200-RGB,DTL-5075-RGB;n:type:ShaderForge.SFN_UVTile,id:9291,x:32200,y:32875,varname:node_9291,prsc:2|WDT-803-OUT,HGT-803-OUT,TILE-325-OUT;n:type:ShaderForge.SFN_ValueProperty,id:325,x:32108,y:33103,ptovrint:False,ptlb:TileSize,ptin:_TileSize,varname:node_325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_ValueProperty,id:803,x:32075,y:32763,ptovrint:False,ptlb:Size,ptin:_Size,varname:node_803,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_VectorRejection,id:5313,x:33117,y:32634,varname:node_5313,prsc:2|A-7277-RGB,B-9655-OUT;n:type:ShaderForge.SFN_Lerp,id:5854,x:32880,y:33161,varname:node_5854,prsc:2|A-31-OUT,B-7277-A,T-7277-A;n:type:ShaderForge.SFN_Lerp,id:4093,x:33103,y:33240,varname:node_4093,prsc:2|A-5854-OUT,B-8362-OUT,T-8587-OUT;n:type:ShaderForge.SFN_Sign,id:8587,x:32892,y:33387,varname:node_8587,prsc:2|IN-7277-A;n:type:ShaderForge.SFN_Lerp,id:8362,x:32676,y:33341,varname:node_8362,prsc:2|A-7200-RGB,B-7277-RGB,T-1779-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1779,x:32488,y:33341,ptovrint:False,ptlb:LigtmapLerp,ptin:_LigtmapLerp,varname:node_1779,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.4;n:type:ShaderForge.SFN_Multiply,id:31,x:33134,y:32915,varname:node_31,prsc:2|A-7599-OUT,B-7200-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6762,x:32789,y:33029,ptovrint:False,ptlb:ColorLight,ptin:_ColorLight,varname:node_6762,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:7599,x:32855,y:32885,varname:node_7599,prsc:2|A-5075-RGB,B-5075-RGB,C-6762-OUT;proporder:5075-7200-325-803-7277-1779-6762;pass:END;sub:END;*/

Shader "Deprecated/TankColor" {
    Properties {
        _Color ("Color", 2D) = "white" {}
        _Lightmap ("Lightmap", 2D) = "white" {}
        _TileSize ("TileSize", Float ) = 10
        _Size ("Size", Float ) = 0.2
        _Details ("Details", 2D) = "black" {}
        _LigtmapLerp ("LigtmapLerp", Float ) = 0.4
        _ColorLight ("ColorLight", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Color; uniform float4 _Color_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _TileSize)
                UNITY_DEFINE_INSTANCED_PROP( float, _Size)
                UNITY_DEFINE_INSTANCED_PROP( float, _LigtmapLerp)
                UNITY_DEFINE_INSTANCED_PROP( float, _ColorLight)
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
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float _Size_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Size );
                float _TileSize_var = UNITY_ACCESS_INSTANCED_PROP( Props, _TileSize );
                float2 node_9291_tc_rcp = float2(1.0,1.0)/float2( _Size_var, _Size_var );
                float node_9291_ty = floor(_TileSize_var * node_9291_tc_rcp.x);
                float node_9291_tx = _TileSize_var - _Size_var * node_9291_ty;
                float2 node_9291 = (i.uv0 + float2(node_9291_tx, node_9291_ty)) * node_9291_tc_rcp;
                float4 _Color_var = tex2D(_Color,TRANSFORM_TEX(node_9291, _Color));
                float _ColorLight_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ColorLight );
                float4 _Lightmap_var = tex2D(_Lightmap,TRANSFORM_TEX(i.uv0, _Lightmap));
                float3 node_31 = ((_Color_var.rgb+_Color_var.rgb+_ColorLight_var)*_Lightmap_var.rgb);
                float4 _Details_var = tex2D(_Details,TRANSFORM_TEX(i.uv0, _Details));
                float _LigtmapLerp_var = UNITY_ACCESS_INSTANCED_PROP( Props, _LigtmapLerp );
                float3 diffuseColor = lerp(lerp(node_31,float3(_Details_var.a,_Details_var.a,_Details_var.a),_Details_var.a),lerp(_Lightmap_var.rgb,_Details_var.rgb,_LigtmapLerp_var),sign(_Details_var.a));
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
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Color; uniform float4 _Color_ST;
            uniform sampler2D _Lightmap; uniform float4 _Lightmap_ST;
            uniform sampler2D _Details; uniform float4 _Details_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _TileSize)
                UNITY_DEFINE_INSTANCED_PROP( float, _Size)
                UNITY_DEFINE_INSTANCED_PROP( float, _LigtmapLerp)
                UNITY_DEFINE_INSTANCED_PROP( float, _ColorLight)
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
                float _Size_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Size );
                float _TileSize_var = UNITY_ACCESS_INSTANCED_PROP( Props, _TileSize );
                float2 node_9291_tc_rcp = float2(1.0,1.0)/float2( _Size_var, _Size_var );
                float node_9291_ty = floor(_TileSize_var * node_9291_tc_rcp.x);
                float node_9291_tx = _TileSize_var - _Size_var * node_9291_ty;
                float2 node_9291 = (i.uv0 + float2(node_9291_tx, node_9291_ty)) * node_9291_tc_rcp;
                float4 _Color_var = tex2D(_Color,TRANSFORM_TEX(node_9291, _Color));
                float _ColorLight_var = UNITY_ACCESS_INSTANCED_PROP( Props, _ColorLight );
                float4 _Lightmap_var = tex2D(_Lightmap,TRANSFORM_TEX(i.uv0, _Lightmap));
                float3 node_31 = ((_Color_var.rgb+_Color_var.rgb+_ColorLight_var)*_Lightmap_var.rgb);
                float4 _Details_var = tex2D(_Details,TRANSFORM_TEX(i.uv0, _Details));
                float _LigtmapLerp_var = UNITY_ACCESS_INSTANCED_PROP( Props, _LigtmapLerp );
                float3 diffuseColor = lerp(lerp(node_31,float3(_Details_var.a,_Details_var.a,_Details_var.a),_Details_var.a),lerp(_Lightmap_var.rgb,_Details_var.rgb,_LigtmapLerp_var),sign(_Details_var.a));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
