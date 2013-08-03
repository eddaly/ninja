// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Ninja/UnlitAlphaOverlay" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

	Pass {
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
	
		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D( _MainTex, IN.uv_MainTex ); 
			o.Albedo = c.rgb*_Color.rgb*IN.color.rgb;
	
			o.Alpha = c.a*_Color.a*IN.color.a;
		}
}
}
