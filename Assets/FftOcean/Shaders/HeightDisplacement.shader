Shader "Custom/HeightDisplacement" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_Scale ("Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _L;
			float _Scale;
			
			struct vs2ps {
				float4 vertex : POSITION;
			};

			vs2ps vert (appdata_full v) {
				vs2ps o;
				
				float4 dx = _MainTex_TexelSize;
				float4 worldPos = mul(_Object2World, v.vertex);
				float2 uv = frac(worldPos.xz / _L);
				float2 st = frac(uv * dx.zw);
				
				float h00 = DecodeFloatRGBA( tex2Dlod(_MainTex, float4(uv, 0.0, 0.0)) );
				float h10 = DecodeFloatRGBA( tex2Dlod(_MainTex, float4(uv.x + dx.x, uv.y, 0.0, 0.0)) );
				float h01 = DecodeFloatRGBA( tex2Dlod(_MainTex, float4(uv.x, uv.y + dx.y, 0.0, 0.0)) );
				float h11 = DecodeFloatRGBA( tex2Dlod(_MainTex, float4(uv + dx.xy, 0.0, 0.0)) );
				
				float h = (1.0 - st.y) * ((1.0 - st.x) * h00 + (st.x * h10)) + st.y * ((1.0 - st.x) * h01 + st.x * h11);
				
				worldPos.y = _Scale * (h - 0.5);
				o.vertex = mul(UNITY_MATRIX_VP, worldPos);
				
				return o;
	        }
	        
	        fixed4 frag(vs2ps IN) : COLOR {
	        	return 1.0;
	        }
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
