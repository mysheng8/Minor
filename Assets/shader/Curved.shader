Shader "Custom/Curved" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_QOffset ("Offset", Vector) = (0,0,0,0)
		_Brightness ("Brightness", Float) = 0.0
		_Dist ("Distance", Float) = 100.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass
        {
			Tags {"LightMode"="ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "UnityLightingCommon.cginc"

			sampler2D _MainTex;
			fixed4 _QOffset;
			half _Dist;
			half _Brightness;


			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR0;
			};


			v2f vert (appdata_base v)
			{
				v2f o;
				float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
				float yOff = vPos.x/_Dist;
				vPos += _QOffset*yOff*yOff;
				o.pos = mul (UNITY_MATRIX_P, vPos);
				o.uv = v.texcoord;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.color=nl * _LightColor0;
				return o;
			}

			fixed4 _Color;

			fixed4  frag (v2f i): SV_Target
			{
				fixed4 col = _Color;//tex2D(_MainTex, i.uv);
				col += UNITY_LIGHTMODEL_AMBIENT*_Brightness;
				return col;
			}
			ENDCG
		}
	}
}
