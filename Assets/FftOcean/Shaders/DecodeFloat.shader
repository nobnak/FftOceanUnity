Shader "Custom/DecodeFloat" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Gain ("Gain", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragmanet frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float _Gain;
			
			struct vs2ps {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			vs2ps vert(appdata_full IN) {
				vs2ps OUT;
				
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				
				return OUT;				
			}
			
			fixed4 frag(vs2ps IN) : COLOR {
				float4 c = tex2D(_MainTex, IN.texcoord);
				float v = DecodeFloatRGBA(c);
				return _Gain * v;
			}			
			ENDCG
		}
	} 
}
