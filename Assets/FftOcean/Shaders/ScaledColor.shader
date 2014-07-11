Shader "Custom/ScaledColor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Scale ("Scale", Vector) = (1, 1, 1, 1)
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
			float4 _Scale;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			vs2ps vert(appdata i) {
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = i.uv;
				return o;
			}
			fixed4 frag(vs2ps i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				//return _Scale * c + 0.5;
				return c;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
