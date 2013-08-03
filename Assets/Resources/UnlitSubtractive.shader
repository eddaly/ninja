Shader "Ninja/UnlitSubtractive" 
{
	Properties 
	{
		_MainTex( "Base (RGBA)", 2D ) = "white" {}
		_Color( "Colour", Color ) = ( 255, 255, 255, 255 )
		_Emissive( "Emissive Colour", Color ) = ( 0, 0, 0, 0 )
	}
	SubShader 
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType" = "Transparent" 
		}
		LOD 200
		
		ZWrite Off
		BlendOp RevSub
		Blend One One 
//		Blend SrcAlpha OneMinusSrcAlpha 
		Lighting Off
		Cull Off
		
		CGPROGRAM
		#pragma surface surf Flat

		half4 LightingFlat( SurfaceOutput s, half3 lightDir, half3 viewDir, half atten ) 
		{
			half4 c;
			c.rgb = s.Albedo.rgb;
			c.a = s.Alpha;
			
			return c;
		}

		struct Input 
		{
			float2 uv_MainTex;
			float4 color: Color;
		};
 
		sampler2D _MainTex;
		half4 _Color;
		half4 _Emissive;
 
		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D( _MainTex, IN.uv_MainTex ); 
//			o.Albedo = (c.rgb*_Color.rgb*IN.color.rgb + _Emissive.rgb)*c.a*_Color.a*IN.color.a;
			o.Albedo = c.rgb*_Color.rgb*IN.color.rgb;
	
			//	Alpha doesn't get used at all
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
