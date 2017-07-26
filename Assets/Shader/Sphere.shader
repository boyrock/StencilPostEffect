Shader "Custom/Sphere" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_Stencil("_Stencil", int) = 0
	}
	SubShader 
	{
		Stencil 
		{
			Ref [_Stencil]
	        Comp Always
	        Pass Replace
		}
		
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = _Color.rgb;// c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
