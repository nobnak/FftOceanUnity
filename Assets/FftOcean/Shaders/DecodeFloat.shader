Shader "Custom/DecodeFloat" {
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
			float _Scale = 1.0;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			vs2ps vert(appdata IN) {
				vs2ps OUT;
				
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				
				return OUT;				
			}
			
			fixed4 frag(vs2ps IN) : COLOR {
				float4 c = tex2D(_MainTex, IN.texcoord);
				float v = DecodeFloatRGBA(c);
				return v;
			}			
			ENDCG
		}
	} 
}
