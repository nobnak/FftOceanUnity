Shader "Custom/WaterParticle" {
	Properties {
		_Fresnel0 ("Fresnel 0", Float) = 0.02
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		LOD 200 Cull Off
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		float _Fresnel0;
		float4 _Color;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;
			float3 worldRefl;
		};
		
		inline float F(float3 v, float3 n) {
			float c = 1.0 - abs(dot(v, n));
			return _Fresnel0 + (1.0 - _Fresnel0) * c * c * c * c * c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float3 V = normalize(_WorldSpaceCameraPos-IN.worldPos);
			float3 N = IN.worldNormal;
		
			float fresnel = F(V, N);
			
			o.Emission = fresnel * _Color.rgb;
			o.Alpha = fresnel * _Color.a;
		}
		ENDCG
	} 
}
