// Upgrade NOTE: upgraded instancing buffer 'PerDrawSprite' to new syntax.

//
//	SpriteStudio6 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
#if defined(UNITY_INSTANCING_ENABLED)
UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
// SpriteRenderer.Color while Non-Batched/Instanced.
fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
// this could be smaller but that's how bit each entry is regardless of type
float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

#define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
#define _Flip unity_SpriteFlipArray[unity_InstanceID]
#endif

CBUFFER_START(UnityPerDrawSprite)
#if !defined(UNITY_INSTANCING_ENABLED)
fixed4 _RendererColor;
float4 _Flip;
#endif
float _EnableExternalAlpha;
CBUFFER_END

struct InputVS
{
	float4 vertex : POSITION;
	float4 color : COLOR0;
	float4 texcoord : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct InputPS
{
#if defined(SV_POSITION)
	float4 Position : SV_POSITION;
#else
	float4 Position : POSITION;
#endif
	float4 ColorMain : COLOR0;
	float4 ColorOverlay : COLOR1;
	float4 Texture00UV : TEXCOORD0;
	float4 PositionDraw : TEXCOORD7;
#if defined(RESTRICT_SHADER_MODEL_3)
	float4	ParameterOverlay : TEXCOORD1;
// #else
#endif
	UNITY_VERTEX_OUTPUT_STEREO
};
