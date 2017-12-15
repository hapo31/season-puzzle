// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradation" {
	Properties
	{
		_MainTex("2D Texture", 2D) = "white" {}
		_TexColor("Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags{ "RenderType" = "Transparent" }
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// VS2015のグラフィックデバックON
				#pragma enable_d3d11_debug_symbols

				uniform fixed4 _TexColor;
				uniform sampler2D _MainTex;

				struct VertexInput {
					float4 pos:  POSITION;    // 3D空間座標
					float2 uv:   TEXCOORD0;   // テクスチャ座標
				};

				struct v2f {
					float4 v:    SV_POSITION; // 2D座標
					half2 uv:   TEXCOORD0;   // テクスチャ座標
					fixed4 color : COLOR;
				};

				// 頂点 shader
				v2f vert(VertexInput input)
				{
					v2f output;
					output.v = UnityObjectToClipPos(input.pos);
					output.uv = input.uv;

					return output;
				}

				// ピクセル shader
				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color = _TexColor * tex2D(_MainTex, IN.uv);
					float texy = IN.uv.y;
					color.rgb += fixed3(1.0 - texy, 1.0 - texy, 1.0 - texy);
					return color;
				}
				ENDCG
		}
	}
}
