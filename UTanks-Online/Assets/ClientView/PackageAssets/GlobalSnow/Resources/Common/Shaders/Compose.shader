Shader "GlobalSnow/Compose" {
Properties {
	_MainTex ("Main RGBA", 2D) = "white" {}
	_SnowedScene ("Snow RGBA", 2D) = "black" {}
	_SnowedScene2("Snow2 RGBA", 2D) = "black" {}
	_DistantSnow ("Distant Snow RGBA", 2D) = "black" {}
	_FrostTex ("Frost RGBA", 2D) = "white" {}
	_FrostNormals ("Frost Normals RGBA", 2D) = "bump" {}
	_FrostIntensity ("Frost Data", Vector) = (1,5,0)
}

SubShader {
   	ZTest Always Cull Off ZWrite Off
   	Fog { Mode Off }
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
		#pragma multi_compile __ FORCE_STEREO_RENDERING
		#pragma multi_compile __ DISTANT_SNOW JUST_DISTANT_SNOW
		#pragma multi_compile __ NO_FROST
		#pragma multi_compile __ NO_SNOW
		#pragma fragmentoption ARB_precision_hint_fastest	
		#pragma target 3.0	
		#include "Compose.cginc"
		ENDCG
	}
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment fragDebug
		#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
		#pragma multi_compile __ FORCE_STEREO_RENDERING
		#pragma multi_compile __ DISTANT_SNOW JUST_DISTANT_SNOW
		#pragma fragmentoption ARB_precision_hint_fastest	
		#pragma target 3.0	
		#include "Compose.cginc"
		ENDCG
	}

}
}
