Shader "Custom/Water" {
	Properties {
		_FresnelLookUp ("Fresnel", 2D) = "white" {}
		_SeaColor ("Sea Color", Color) = (1, 1, 1, 1)
		_SkyBox("SkyBox", CUBE) = "" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _FresnelLookUp;
		float4 _SeaColor;
		samplerCUBE _SkyBox;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;			
			float3 worldRefl;
		};
				
		float Fresnel(float3 V, float3 N) {
			float costhetai = abs(dot(V, N));
			return tex2D(_FresnelLookUp, float2(costhetai, 0.0)).a;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float3 V = normalize(_WorldSpaceCameraPos-IN.worldPos);
			float3 N = IN.worldNormal;
		
			float fresnel = Fresnel(V, N);
			
			float3 skyColor = texCUBE(_SkyBox, WorldReflectionVector(IN, N)*float3(1,1,1)).rgb;
		
			o.Albedo = lerp(_SeaColor, skyColor, fresnel);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
}
