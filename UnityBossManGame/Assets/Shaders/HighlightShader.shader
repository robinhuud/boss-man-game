// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/HighlightTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", Color) = (0,0,0,0)
		_HighlightColor("Highlight Color", Color) = (0,0,0,0)
		_HighlightWidth("Highlight Width", Range(0.0000, 0.03)) = 0.003
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct appdata {
		float4 vertex: POSITION;
		float3 normal: NORMAL;
	};

	struct v2f {
		float4 pos: POSITION;
		float4 color: COLOR;
	};

	uniform float _HighlightWidth;
	uniform float4 _HighlightColor;

	v2f vert(appdata v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
		o.pos.xy += offset * o.pos.z * _HighlightWidth;
		o.color = _HighlightColor;
		return o;
	}

	ENDCG

		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			//LOD 200


			Pass{
				Name "OUTLINE"
				Tags {"LightMode" = "Always" "Queue" = "Overlay" }
				Cull Front
				ZWrite Off
				ZTest Always
				ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha
				//Offset 5, 5

				CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				half4 frag(v2f i) : COLOR { return i.color; }
				ENDCG
			}
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

			sampler2D _MainTex;
			half _Glossiness;
			half _Metallic;
			fixed4 _Emission;
			fixed4 _Color;

			struct Input {
				float2 uv_MainTex;
			};



			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Emission = _Emission;
				o.Alpha = c.a;
			}
			ENDCG
		}

		SubShader{
			Tags {"Queue" = "Overlay"}
			CGPROGRAM
			#pragma surface surf Standard
			sampler2D _MainTex;
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG

			Pass{
				Name "OUTLINE"
				Tags {"LightMode" = "Always"}
				Cull Front
				ZWrite Off
				ZTest Always
				ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha

				SetTexture [_MainTex] { combine primary }
			}
		}
		
	FallBack "Diffuse"
}
