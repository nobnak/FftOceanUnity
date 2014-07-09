Shader "Custom/DecodeVector" {
	Properties {
		_XTex ("X", 2D) = "white" {}
		_YTex ("Y", 2D) = "white" {}
		_ZTex ("Z", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _XTex;
			sampler2D _YTex;
			sampler2D _ZTex;
			float _L;
			float _ScaleX = 1.0;
			float _ScaleY = 1.0;
			float _ScaleZ = 1.0;
			
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
				float x = DecodeFloatRGBA(tex2D(_XTex, IN.texcoord));
				float y = DecodeFloatRGBA(tex2D(_YTex, IN.texcoord));
				float z = DecodeFloatRGBA(tex2D(_ZTex, IN.texcoord));
				return float4(x, y, z, 1.0);
			}			
			ENDCG
		}
	} 
}
