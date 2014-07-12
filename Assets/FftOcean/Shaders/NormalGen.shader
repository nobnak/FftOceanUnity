Shader "Custom/NormalGen" {
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
			
			struct appdat {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			vs2ps vert (appdata v) {
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = v.uv;
				return o;
	        }
	        
	        fixed4 frag(vs2ps IN) : COLOR {
	        	
	        }
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
