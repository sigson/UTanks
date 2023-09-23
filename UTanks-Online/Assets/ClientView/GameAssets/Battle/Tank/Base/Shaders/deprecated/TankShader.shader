Shader "JT/PreviewDefaultTankShader" {
	Properties{
		_Colormap("Base (RGB)", 2D) = "white" {}
		_Lightmap("Lightmap (RGB)", 2D) = "lightmap" {}
		_Details("Details (RGB)", 2D) = "gray" {}
	}
		SubShader{
		// Ambient pass
		Pass {
				//Tags {"LightMode" = "PixelOrNone"}
				//Color [_PPLAmbient]
				SetTexture[_Colormap] {combine texture}
				SetTexture[_Lightmap] {combine texture * previous double, previous}
			}
			Pass {
				Blend SrcAlpha OneMinusSrcAlpha
				SetTexture[_Details] {combine texture}
			}

		// Vertex lights
		//Pass { 
		//	Tags {"LightMode" = "Vertex"}
		//	Material {
		//		Diffuse [_Color]
		//		Emission [_PPLAmbient]
		//	}
		//	Lighting On
			//SetTexture [_MainTex] {constantColor [_PPLAmbent] Combine texture * primary DOUBLE, texture * primary}
			//SetTexture [_Detail] { combine previous * texture DOUBLE, previous }
		//}
		//UsePass "Diffuse/PPL"
	}
		FallBack "VertexLit", 2
}