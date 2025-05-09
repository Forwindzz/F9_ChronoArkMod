//
//	SpriteStudio6 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
fixed4 _Color;	// Material Color.

float4 _BlendParam;	/* .x:Blend-Operation / .y:Opacity / .z:PartsColor.Opacity / .w:(no used) */
float4 _PartsColor;

#if defined(RESTRICT_SHADER_MODEL_3)
/* MEMO: ".x" is not used, now. */
static const float4 _OverlayParameter_Mix = {1.0f, 1.0f, 0.0f, 1.0f};
static const float4 _OverlayParameter_Add = {1.0f, 0.0f, 0.0f, 1.0f};
static const float4 _OverlayParameter_Sub = {1.0f, 0.0f, 0.0f, -1.0f};
static const float4 _OverlayParameter_Mul = {1.0f, 1.0f, 1.0f, 1.0f};
// #else
#endif

InputPS VS_main(InputVS input)
{
	InputPS output;
	float4 temp;
	float indexBlend = floor(_BlendParam.x);

	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

	temp.xy = input.texcoord.xy;
	temp.z = indexBlend;
	temp.w = 0.0f;
	output.Texture00UV = temp;

#if defined(RESTRICT_SHADER_MODEL_3)
	output.ParameterOverlay = (2.0f > indexBlend)
								? ((1.0f > indexBlend) ? _OverlayParameter_Mix : _OverlayParameter_Add)
								: ((3.0f > indexBlend) ? _OverlayParameter_Sub : _OverlayParameter_Mul);
// #else
#endif

	output.ColorOverlay = _PartsColor;

	temp = input.color * _RendererColor * _Color;
	temp.a *= _BlendParam.y * _BlendParam.z;
	output.ColorMain = temp;

	temp = UnityObjectToClipPos(input.vertex);
	output.PositionDraw = temp;
	output.Position = temp;

	return(output);
}
