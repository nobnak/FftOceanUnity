Shader "Custom/Water" {
	Properties {
		_SkyBox("SkyBox", CUBE) = "" {}
		_Fresnel0 ("Fresnel 0", Float) = 0.02
		_SeaColor ("Sea Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		samplerCUBE _SkyBox;
		float _Fresnel0;
		float4 _SeaColor;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;			
			float3 worldRefl;
		};
				
		float Fresnel(float3 V, float3 N) {
			float c = 1.0 - abs(dot(V, N));
			return _Fresnel0 + (1.0 - _Fresnel0) * c * c * c * c * c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float3 V = normalize(_WorldSpaceCameraPos-IN.worldPos);
			float3 N = IN.worldNormal;
		
			float fresnel = Fresnel(V, N);
			
			float3 skyColor = texCUBE(_SkyBox, WorldReflectionVector(IN, N)).rgb;
		
			//o.Albedo = (1.0 - fresnel) * _SeaColor;
			//o.Emission = skyColor * fresnel;
			//o.Albedo = lerp(_SeaColor, skyColor, fresnel);
			o.Emission = lerp(_SeaColor, skyColor, fresnel);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
}
