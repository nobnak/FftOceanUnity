Shader "Custom/HeightDisplacement" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _L;
			
			struct vs2ps {
				float4 vertex : POSITION;
			};

			vs2ps vert (appdata_full v) {
				vs2ps o;
				
				float4 worldPos = mul(_Object2World, v.vertex);
				
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
