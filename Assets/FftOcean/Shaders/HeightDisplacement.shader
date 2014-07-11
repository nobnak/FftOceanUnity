Shader "Custom/HeightDisplacement" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_Scale ("Scale", Vector) = (1, 1, 1, 1)
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
			float4 _Scale;
			
			struct vs2ps {
				float4 vertex : POSITION;
			};

			vs2ps vert (appdata_full v) {
				vs2ps o;
				
				float4 dx = _MainTex_TexelSize;
				float4 worldPos = mul(_Object2World, v.vertex);
				float2 uv = frac(worldPos.xz / _L);
				float2 st = frac(uv * dx.zw);
				float4 uvw = tex2Dlod(_MainTex, float4(uv + 0.5 * dx.xy, 0.0, 0.0));
				
				worldPos.y = _Scale.z * uvw.z;
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
