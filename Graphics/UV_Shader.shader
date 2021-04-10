// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Unlit/Geometrie"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_uvThickness("float", Float) = 0.5
	}
		SubShader
		{
			Tags { "Queue" = "Geometry" }
			ZTest Off
			ZWrite Off
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma geometry geo

				// make fog work
				
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : Normal;
					float2 uv : TEXCOORD0;
				};

				struct v2g
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct g2f
				{
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
					//UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};
		
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _uvThickness;
				float4 _Color;
		
				v2g vert(appdata v)
				{
					v2g o;
					o.vertex = v.vertex;
					o.normal = v.normal;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv = v.uv;
					//UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				void CreateTris(v2g a, v2g b, float3 center , inout TriangleStream <g2f> triStream)
				{
					g2f Out;

					float3 a_pushed = lerp(a.vertex, center, _uvThickness);

					float3 b_pushed = lerp(b.vertex, center, _uvThickness);

					Out.vertex = UnityObjectToClipPos(a.vertex);
					Out.normal = a.normal;
					Out.uv = a.uv;

					triStream.Append(Out);


					Out.vertex = UnityObjectToClipPos(b.vertex);
					Out.normal = b.normal;
					Out.uv = b.uv;

					triStream.Append(Out);

					Out.vertex = UnityObjectToClipPos(a_pushed);
					Out.normal = a.normal;
					Out.uv = a.uv;

					triStream.Append(Out);

					triStream.RestartStrip();


					Out.vertex = UnityObjectToClipPos(b.vertex);
					Out.normal = b.normal;
					Out.uv = b.uv;

					triStream.Append(Out);


					Out.vertex = UnityObjectToClipPos(b_pushed);
					Out.normal = b.normal;
					Out.uv = b.uv;

					triStream.Append(Out);


					Out.vertex = UnityObjectToClipPos(a_pushed);
					Out.normal = a.normal;
					Out.uv = a.uv;

					triStream.Append(Out);

					triStream.RestartStrip();
				}

				[maxvertexcount(18)]
				void geo(triangle v2g IN[3], inout TriangleStream <g2f> triStream) 
				{
					g2f Out;

					float3 v1 = IN[0].vertex.xyz;
					float3 v2 = IN[1].vertex.xyz;
					float3 v3 = IN[2].vertex.xyz;

					float3 center = (v1 + v2 + v3) / 3.0;
					float3 normal = normalize(cross(v2 - v1, v3 - v1));

					/*
						v1 .---------.v2
							\		/
							 \	   /
							  \	  /
							 ` \ /
								. v3
					*/

					CreateTris(IN[0], IN[1], center, triStream);
					CreateTris(IN[1], IN[2], center, triStream);
					CreateTris(IN[2], IN[0], center, triStream);
				}

				fixed4 frag(g2f i) : SV_Target
				{
					// sample the texture
					fixed4 col = 1;
					// apply fog
					return _Color;
				}
				ENDCG
		}

}
	FallBack "Diffuse"
}

