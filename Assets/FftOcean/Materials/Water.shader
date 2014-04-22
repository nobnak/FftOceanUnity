Shader "Custom/Water" {
	Properties {
		_SkyBox("SkyBox", CUBE) = "" {}
		_SeaColor ("Sea Color", Color) = (1, 1, 1, 1)
		_Fresnel0 ("Fresnel 0", Float) = 0.02
		_Roughness ("Roughnss", Range(0.01, 1.0)) = 0.01
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Sun

		sampler2D _MainTex;
		samplerCUBE _SkyBox;
		float4 _SeaColor;
		float _Fresnel0;
		float _Roughness;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldPos;			
			float3 worldRefl;
		};
		
		inline float F(float3 v, float3 n) {
			float c = 1.0 - max(0.0, dot(v, n));
			return _Fresnel0 + (1.0 - _Fresnel0) * c * c * c * c * c;
		}
		inline float D(float3 m, float3 n) {
			float TWO_PI = 6.28318;
			float nm = max(0.0, dot(n, m));
			float a = 2.0 / (_Roughness * _Roughness) - 2.0;
			return (a + 2.0) / TWO_PI * pow(nm, a);
		}

		inline float4 LightingSun(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
			float3 n = s.Normal;
			float3 l = lightDir;
			float3 v = viewDir;
			float3 h = normalize(l + v);
			float nl = max(0.0, dot(n, l));
			float spec = F(v, h) * D(h, n);
			
			return float4(spec * nl * _LightColor0.rgb, 1.0);
		}
				
		void surf (Input IN, inout SurfaceOutput o) {
			float3 V = normalize(_WorldSpaceCameraPos-IN.worldPos);
			float3 N = IN.worldNormal;
		
			float fresnel = F(V, N);
			
			float3 skyColor = texCUBE(_SkyBox, WorldReflectionVector(IN, N) * float3(1, 1, 1)).rgb;
			
			o.Emission = lerp(_SeaColor, skyColor, fresnel);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
}
