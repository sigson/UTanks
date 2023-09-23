// Upgrade NOTE: removed variant '__' where variant LOD_FADE_PERCENTAGE is used.

Shader "GlobalSnow/Snow" {

/////////// OPAQUE OBJECTS //////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="Opaque" }	
	CGPROGRAM
	#pragma surface surf BlinnPhong vertex:vert keepalpha addshadow exclude_path:prepass nometa exclude_path:deferred 
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile __ GLOBALSNOW_FLAT_SHADING GLOBALSNOW_RELIEF GLOBALSNOW_OCCLUSION
	#pragma multi_compile __ GLOBALSNOW_OPAQUE_CUTOUT
	#pragma multi_compile __ GLOBALSNOW_FOOTPRINTS
	#pragma multi_compile __ GLOBALSNOW_TERRAINMARKS
	#define GLOBALSNOW_ENABLE_SLOPE_CONTROL 1
	#define GLOBALSNOW_IS_TERRAIN 1
	#include "GlobalSnow.cginc"

	void vert (inout appdata_full v, out Input o) {
        UNITY_INITIALIZE_OUTPUT(Input,o);
		if (_GS_ReliefFlag>0) {	// Provide tangents for Terrain
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		}

    }

    void surf (Input IN, inout SurfaceOutput o) {
		// Check alpha
		#if GLOBALSNOW_OPAQUE_CUTOUT
		fixed4 textureColor = tex2D(_MainTex, IN.uv_MainTex);
		clip(textureColor.a + textureColor.g - 0.01);
		#endif
		SetSnowCoverage(IN, o);
    }
    
    ENDCG
}


/////////// TERRAIN ////////////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="TerrainOpaque" }	
	CGPROGRAM
	#pragma surface surf BlinnPhong keepalpha addshadow exclude_path:deferred exclude_path:prepass nometa
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile __ GLOBALSNOW_FLAT_SHADING GLOBALSNOW_RELIEF GLOBALSNOW_OCCLUSION
	#pragma multi_compile __ GLOBALSNOW_FOOTPRINTS
	#pragma multi_compile __ GLOBALSNOW_TERRAINMARKS
 	#define GLOBALSNOW_ENABLE_SLOPE_CONTROL 1
 	#define GLOBALSNOW_IS_TERRAIN 1
	#include "GlobalSnow.cginc"
             
    void surf (Input IN, inout SurfaceOutput o) {
		SetSnowCoverage(IN, o);
    }
    
    ENDCG
}

/////////// OPAQUE OBJECTS //////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="TransparentCutout" }	
	CGPROGRAM
	#pragma surface surf BlinnPhong keepalpha addshadow exclude_path:deferred exclude_path:prepass nometa noforwardadd
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile __ GLOBALSNOW_FLAT_SHADING
	#include "GlobalSnow.cginc"
             
    void surf (Input IN, inout SurfaceOutput o) {
		// Check alpha
		fixed4 textureColor = tex2D(_MainTex, IN.uv_MainTex);
		clip(textureColor.a - 0.01);
		SetSnowCoverage(IN, o);
    }
    
    ENDCG
}


/////////// SPECIAL EXCLUDED OBJECTS //////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="AntiSnow" }	
	
	Pass {
	
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"
	
	struct v2f {
	    float4 pos : SV_POSITION;
	};
	
	v2f vert (appdata_base v) {
	    v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
    	return o;
	}
	
   	fixed4 frag(v2f i) : SV_Target {
		return fixed4(0,0,0,0);
    }
    ENDCG

    }
}


SubShader {
	Tags { "RenderType"="ParticleCutOut" }	

	Pass {
	Cull Off
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	fixed _Cutoff;

	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv  : TEXCOORD0;
	};
	
	v2f vert (appdata_base v) {
	    v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
    	return o;
	}
	
   	fixed4 frag(v2f i) : SV_Target {
   		fixed4 co = tex2D(_MainTex, i.uv);
   		clip (co.a - _Cutoff);
		return fixed4(0,0,0,0);
    }
    ENDCG

    }
}


/////////// TreeOpaque ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="TreeOpaque" "DisableBatching"="True" }
	CGPROGRAM
	#pragma surface surf BlinnPhong vertex:vert keepalpha addshadow exclude_path:deferred exclude_path:prepass nometa noforwardadd
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma multi_compile __ GLOBALSNOW_FLAT_SHADING 
	#define GLOBALSNOW_MOVING_OBJECT
	#include "UnityCG.cginc"
	#include "TerrainEngine.cginc"
	#include "GlobalSnow.cginc"

    void vert (inout appdata_full v, out Input data) {
	    UNITY_INITIALIZE_OUTPUT(Input, data);
	    float3 worldPosOrig  = mul(unity_ObjectToWorld, v.vertex).xyz;
		TerrainAnimateTree(v.vertex, v.color.w);
	    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	    data.wposOffset = worldPos - worldPosOrig;	   
    }
    
    void surf (Input IN, inout SurfaceOutput o) {
		SetSnowCoverage(IN, o);
    }
	ENDCG
}
 
 /////////// SpeedTree ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="SnowedSpeedTree" "DisableBatching"="LODFading" }
	LOD 400
	Cull [_Cull]


	CGPROGRAM
	#pragma surface surf Lambert vertex:SpeedTreeVert nolightmap keepalpha exclude_path:deferred exclude_path:prepass nometa noforwardadd noshadowmask // dithercrossfade // bug in Unity dithercrossfade raised shader warning
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
//    #pragma multi_compile_vertex __ LOD_FADE_PERCENTAGE
    #pragma instancing_options assumeuniformscaling lodfade maxcount:50
	#pragma multi_compile GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
	#pragma multi_compile __ GLOBALSNOW_DISCARD_LEAVES
	#define ENABLE_WIND

    #include "GlobalSnowForwardOptions.cginc"
	#include "SpeedTree/SnowedSpeedTreeCommon.cginc"
	#include "SpeedTree/SnowedSpeedTree.cginc"
		
	void surf(Input IN, inout SurfaceOutput OUT) {
		#if GEOM_TYPE_LEAF && GLOBALSNOW_DISCARD_LEAVES
		discard;
		#else
		SpeedTreeFragOut o;
		SpeedTreeFrag(IN, o);
		SPEEDTREE_COPY_FRAG(OUT, o)
		SetTreeCoverage(IN, OUT);
		#endif
	}
	ENDCG
	
Pass
		{
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma target 3.0
                #pragma multi_compile_vertex  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
                #pragma multi_compile_fragment __ LOD_FADE_CROSSFADE
                #pragma multi_compile_instancing
				#pragma multi_compile GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
				#pragma multi_compile __ GLOBALSNOW_DISCARD_LEAVES
				#pragma multi_compile_shadowcaster
				#define ENABLE_WIND
				#include "SpeedTree/SnowedSpeedTreeCommon.cginc"

				struct v2f 
				{
					V2F_SHADOW_CASTER;
					#ifdef SPEEDTREE_ALPHATEST
						half2 uv : TEXCOORD1;
					#endif
                    UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				v2f vert(SpeedTreeVB v)
				{
					v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
					#ifdef SPEEDTREE_ALPHATEST
						o.uv = v.texcoord.xy;
					#endif
					OffsetSpeedTreeVertex(v, unity_LODFade.x);
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					#if GEOM_TYPE_LEAF && GLOBALSNOW_DISCARD_LEAVES
					discard;
					return 0;
					#else
                    UNITY_SETUP_INSTANCE_ID(i);
					#ifdef SPEEDTREE_ALPHATEST
						clip(tex2D(_MainTex, i.uv).a * _Color.a - _Cutoff);
					#endif
					UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
					SHADOW_CASTER_FRAGMENT(i)
					#endif
				}
			ENDCG
		}	
}

/////////// SpeedTree Billboard ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="SnowedSpeedTreeBillboard" "DisableBatching"="LODFading" }
		LOD 400
		Cull Off

		CGPROGRAM
			#pragma surface surf Lambert vertex:SpeedTreeBillboardVert nolightmap keepalpha exclude_path:deferred exclude_path:prepass nometa noforwardadd addshadow noinstancing
			#pragma target 3.0
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile __ LOD_FADE_CROSSFADE
			#pragma multi_compile __ BILLBOARD_FACE_CAMERA_POS
			#define ENABLE_WIND
			#include "SpeedTree/SnowedSpeedTreeBillboardCommon.cginc"
			#include "SpeedTree/SnowedSpeedTreeBillboard.cginc"
			
			void surf(Input IN, inout SurfaceOutput OUT)
			{
				SpeedTreeFragOut o;
				SpeedTreeFrag(IN, o);
				SPEEDTREE_COPY_FRAG(OUT, o)
				SetTreeBillboardCoverage(IN, OUT);
			}
		ENDCG
}

/////////// Grass ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "Queue" = "Geometry+200"	"IgnoreProjector"="True" "RenderType"="Grass" "DisableBatching"="True" }
	Cull Off
	LOD 200
		
CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassVert keepalpha exclude_path:deferred exclude_path:prepass nometa
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "GlobalSnowForwardOptions.cginc"
#include "TerrainEngine.cginc"
#include "Nature/SnowedGrass.cginc"

sampler2D _MainTex;
fixed _Cutoff;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex); // * IN.color;
	clip (c.a - _Cutoff);
	SetGrassCoverage(IN, o);
}
ENDCG

Pass {
	Tags{ "LightMode" = "ShadowCaster" }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#pragma target 3.0
	#pragma multi_compile_shadowcaster
	#include "UnityCG.cginc"
	#include "TerrainEngine.cginc"

	struct v2f {
		V2F_SHADOW_CASTER;
		half2 uv : TEXCOORD1;
	};

	sampler2D _MainTex;
	fixed _Cutoff;


	v2f vert(appdata_full v) {
		float waveAmount = v.color.a * _WaveAndDistance.z;
		v.color = TerrainWaveGrass(v.vertex, waveAmount, v.color);

		v2f o;
		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
		o.uv = v.texcoord.xy;
		return o;
	}

	float4 frag(v2f i) : SV_Target {
		clip(tex2D(_MainTex, i.uv).a - _Cutoff);
		SHADOW_CASTER_FRAGMENT(i)
	}
	ENDCG
}
}

/////////// Grass Billboard ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "Queue" = "Geometry+200" "IgnoreProjector"="True" "RenderType"="GrassBillboard" "DisableBatching"="True" }
		Cull Off
		LOD 200
				
CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassBillboardVert nolightmap keepalpha exclude_path:deferred exclude_path:prepass nometa noforwardadd
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "GlobalSnowForwardOptions.cginc"
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
#include "Nature/SnowedGrass.cginc"
    	
sampler2D _MainTex;
fixed _Cutoff;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex); // * IN.color;
	clip (c.a - _Cutoff);
	SetGrassCoverage(IN, o);
}

ENDCG

	Pass{
	Tags{ "LightMode" = "ShadowCaster" }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 3.0
	#pragma multi_compile_shadowcaster
	#include "UnityCG.cginc"
	#include "TerrainEngine.cginc"

	struct v2f {
		V2F_SHADOW_CASTER;
		half2 uv : TEXCOORD1;
	};

	sampler2D _MainTex;
	fixed _Cutoff;


	v2f vert(appdata_full v) {
		TerrainBillboardGrass(v.vertex, v.tangent.xy);
		// wave amount defined by the grass height
		float waveAmount = v.tangent.y;
		v.color = TerrainWaveGrass(v.vertex, waveAmount, v.color);

		v2f o;
		o.uv = v.texcoord.xy;
		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
		return o;
	}

	float4 frag(v2f i) : SV_Target{
		clip(tex2D(_MainTex, i.uv).a - _Cutoff);
		SHADOW_CASTER_FRAGMENT(i)
	}
	ENDCG
	}
}


/////////// Tree Bark ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="TreeBark" }	

CGPROGRAM
#pragma surface surf BlinnPhong vertex:TreeVertBark keepalpha addshadow nolightmap exclude_path:deferred exclude_path:prepass nometa noforwardadd
#pragma multi_compile __ GLOBALSNOW_FLAT_SHADING
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityBuiltin3xTreeLibrary.cginc"
#include "GlobalSnow.cginc"

    void surf (Input IN, inout SurfaceOutput o) {
		SetSnowCoverage(IN, o);
    }

ENDCG
}


///////////// Tree Leaf ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="TreeLeaf" }	

	CGPROGRAM
#pragma surface surf BlinnPhong vertex:TreeVertLeaf keepalpha addshadow nolightmap exclude_path:deferred exclude_path:prepass nometa noforwardadd
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityBuiltin3xTreeLibrary.cginc"
#include "BasicCoverage.cginc"

fixed _Cutoff;

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	clip (c.a - _Cutoff);
	o.Alpha = c.a;
	SetBasicCoverage(IN, o);
}
ENDCG
}


///////////// Tree Transparent Cutout ///////////////////////////////////////////////////////////////////

SubShader {
	Tags { "RenderType"="TreeTransparentCutout" }	

	CGPROGRAM
	#pragma surface surf BlinnPhong vertex:vert keepalpha addshadow nolightmap exclude_path:deferred exclude_path:prepass nometa noforwardadd
	#pragma target 3.0
	#pragma fragmentoption ARB_precision_hint_fastest
	#define GLOBALSNOW_FLAT_SHADING 1
	#define GLOBALSNOW_MOVING_OBJECT
	#define GLOBALSNOW_IGNORE_SURFACE_NORMAL 1
	#include "UnityCG.cginc"
	#include "TerrainEngine.cginc"
	#include "GlobalSnow.cginc"

	fixed _Cutoff;

    void vert (inout appdata_full v, out Input data) {
	    UNITY_INITIALIZE_OUTPUT(Input, data);
	    float3 worldPosOrig  = mul(unity_ObjectToWorld, v.vertex).xyz;
		TerrainAnimateTree(v.vertex, v.color.w);
	    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	    data.wposOffset = worldPos - worldPosOrig;	   
    }
    
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		clip (c.a - _Cutoff);
		SetSnowCoverage(IN, o);
	}
	ENDCG
}


///////// Tree Billboard (uncomment this section and comment out [ImageEffectOpaque] in GlobalSnow.cs script to support them ///////////////////////////////////////////////////////

// SubShader {
// 	Tags { "Queue" = "Transparent-100" "RenderType"="TreeBillboard" }
// 	
// 	Pass {
// 	ZWrite Off Cull Off
// 	CGPROGRAM
// 	#pragma vertex vert
// 	#pragma fragment frag
//       #include "GlobalSnowForwardOptions.cginc"
// 	#include "UnityCG.cginc"
// 	#include "TerrainEngine.cginc"
// 	#include "Nature/SnowedTreeBillboard.cginc"
//     	
// 			v2f vert (appdata_tree_billboard v) {
// 				v2f o;
// 				UNITY_SETUP_INSTANCE_ID(v);
// 				TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);	
// 				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
// 				o.pos = UnityObjectToClipPos(v.vertex);
// 				o.uv.x = v.texcoord.x;
// 				o.uv.y = v.texcoord.y > 0;
// 				return o;
// 			}

// 			sampler2D _MainTex;
// 			fixed4 frag(v2f input) : SV_Target
// 			{
// 				fixed4 col = tex2D(_MainTex, input.uv);
// 				clip(col.a - 0.001);
// 				SetTreeBillboardCoverage(input, col);
// 				return col;
// 			}
// 			ENDCG		
//     }
// }


Fallback Off
}
