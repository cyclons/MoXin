Shader "ChineseInk"
{
	Properties
	{
		_MainTex("Main", 2D) = "white" {}
		_Saturation("Saturation",Range(0.01,1)) = 0.5
		_Brightness("Brightness",Range(0.01,1)) = 0.7
		_Contrast("Contrast",Range(0.01,1)) = 0.5

		_Thred("Edge Thred" , Range(0.01,1)) = 0.25
		//_Range("Edge Range" , Range(1,10)) = 1			
		_EdgeTex("Edge Texture", 2D) = "white" {}
		_BrushTex("Brush Texture", 2D) = "white" {}

		[Enum(Opacity,1,Darken,2,Lighten,3,Multiply,4,Screen,5,Overlay,6,SoftLight,7)]
		_BlendType("Blend Type", Int) = 1



	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Thred;
			float _Range;
			sampler2D _EdgeTex;
			float4 _EdgeTex_ST;
			sampler2D _BrushTex;
			float4 _BrushTex_ST;
			float _BlendType;
			float _Saturation;
			float _Brightness;
			float _Contrast;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float vdotn : TEXCOORD3;

			};


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _BrushTex);
				o.worldPos = UnityObjectToClipPos(v.vertex);
				float3 viewDir = normalize(mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz - v.vertex);
				o.vdotn = dot(normalize(viewDir), v.normal);
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
			// sample the texture
			fixed4 mainTex = tex2D(_MainTex, i.uv.xy);
			fixed4 brushTex = tex2D(_BrushTex, i.uv.zw);
			mainTex = pow(mainTex, 1-_Brightness);
			mainTex *= 1 - cos(mainTex * 3.14)*_Contrast*2;

			fixed texGrey = (mainTex.r + mainTex.g + mainTex.b)*0.33;
			mainTex = _Saturation* mainTex+(1-_Saturation)*texGrey;

			fixed brushGrey= (brushTex.r + brushTex.g + brushTex.b)*0.33;
			
			fixed4 blend;
			//[Enum(Opacity, 1, Darken, 2, Lighten, 3, Multiply, 4, Screen, 5, Overlay, 6, SoftLight, 7)]
			if (_BlendType == 1)
				blend = mainTex * 0.5 + brushGrey * 0.5;
			else if (_BlendType == 2)
				blend = mainTex < brushGrey ? mainTex : brushGrey;
			else if (_BlendType == 3)
				blend = mainTex > brushGrey ? mainTex : brushGrey;
			else if (_BlendType == 4)
				blend = mainTex * brushGrey;
			else if (_BlendType == 5)
				blend = 1-(1- mainTex)*(1- brushGrey);
			else if (_BlendType == 6)
				blend = brushGrey >0.5 ? 1 - 2*(1 - mainTex)*(1 - brushGrey) : 2* mainTex * brushGrey;
			else if (_BlendType == 7)
				blend = mainTex >0.5 ? (2* mainTex -1)*(brushGrey- brushGrey* brushGrey)+ brushGrey : (2 * mainTex - 1)*(sqrt(brushGrey) - brushGrey) + brushGrey;
			fixed4 col = fixed4(blend.rgb, 1);



			//边缘
			fixed edge = pow(i.vdotn, 1)/4;
			fixed4 edgeColor = tex2D(_EdgeTex, float2(edge/_Thred, (i.uv.x+ i.uv.x)*0.5));
			edgeColor.a = edge > _Thred ? 1 : edge;

			col =edgeColor*(1-edgeColor.a)+ col *(edgeColor.a);

			return col;
			}

			ENDCG
		}
	}
}
