// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Background"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_PatternTex("Pattern Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_LineWidth("Line Width", Float) = 1
		_PatternLength("Pattern Length", Float) = 300
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex SpriteVert
#pragma fragment SpriteFrag
#pragma target 2.0

#include "UnityCG.cginc"

	fixed4 _Color;
	sampler2D _MainTex;
	sampler2D _PatternTex;
	uniform float4 _MainTex_TexelSize;
	fixed4 _PatternTex_ST;
	float _LineWidth;
	float _PatternLength;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float2 texcoord : TEXCOORD0;
		float2 patterncoord : TEXCOORD1;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		float2 patterncoord : TEXCOORD1;
	};

	v2f SpriteVert(appdata_t IN)
	{
		v2f OUT;
		OUT.texcoord = IN.texcoord;
		OUT.patterncoord = TRANSFORM_TEX(IN.patterncoord, _PatternTex);
		OUT.vertex = UnityObjectToClipPos(IN.vertex);

		return OUT;
	}

	fixed4 SampleSpriteTexture(float2 uv1, float2 uv2)
	{
		if (tex2D(_MainTex, uv1).r > 0.8)
			return 0;
		fixed border = 0;
		for (fixed i = -1; i < 2; i++)
			for (fixed j = -1; j < 2; j++)
				border += tex2D(_MainTex, uv1 - fixed2(_MainTex_TexelSize.x * _LineWidth * i, _MainTex_TexelSize.y * _LineWidth * j)).r;
		fixed4 outline = fixed4(1, 1, 1, 1) * (border > 0 ? 1 : 0);
		border = 0;
		for (fixed i = -6; i < 7; i++)
			for (fixed j = -6; j < 7; j++)
				border += tex2D(_MainTex, uv1 - fixed2(_MainTex_TexelSize.x * _PatternLength * i, _MainTex_TexelSize.y * _PatternLength * j)).r * (1-(abs(i)+abs(j)) / 12);
		fixed len = min(1, max(0, border-8)*0.15 - 0.1);
		len = max(0, len);
		fixed4 pattern = tex2D(_PatternTex, uv2) * len;
		return max(outline, pattern);
	}

	fixed4 SpriteFrag(v2f IN) : SV_Target
	{
		fixed4 c = SampleSpriteTexture(IN.texcoord, IN.patterncoord) * _Color;
		c.rgb *= c.a;
		return c;
	}

		ENDCG
	}
	}
}
