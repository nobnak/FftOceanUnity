Shader "Custom/FresnelSchlick" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 1
		_Fresnel ("Fresnel", Float) = 0.02
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf FresnelSchlick

		sampler2D _MainTex;
		float4 _Color;
		float _Shininess;
		float _Fresnel;

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
		};
		
		float FresnelSchlick(float3 v, float3 h) {
			float c = 1.0 - dot(v, h);
			return _Fresnel + (1.0 - _Fresnel) * c * c * c * c * c;
		}
		
		inline float4 LightingFresnelSchlick(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten) {
			float3 h = normalize (lightDir + viewDir);
			float nl = saturate(dot(s.Normal, lightDir));
			float nh = saturate(dot(s.Normal, h));
			float fresnel = FresnelSchlick(lightDir, h);
			float3 diffuse = (1.0 - fresnel) * s.Albedo;
			float3 specular = fresnel * ((_Shininess + 2.0) / 8.0) * pow(nh, _Shininess);
			
			float4 c;
			c.rgb = (diffuse + specular) * nl * _LightColor0.rgb * (atten * 2);
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
