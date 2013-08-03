Shader "Ninja/UnlitOpaque" 
{
	Properties 
	{
		_MainTex( "Base (RGBA)", 2D ) = "white" {}
		_Color( "Colour", Color ) = ( 255, 255, 255, 255 )
		_Emissive( "Emissive Colour", Color ) = ( 0, 0, 0, 0 )
	}
	SubShader 
	{
		LOD 200 
		
		Blend SrcAlpha OneMinusSrcAlpha 
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
			o.Albedo = c.rgb*_Color.rgb*IN.color.rgb + _Emissive.rgb;
	
			o.Alpha = c.a*IN.color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
