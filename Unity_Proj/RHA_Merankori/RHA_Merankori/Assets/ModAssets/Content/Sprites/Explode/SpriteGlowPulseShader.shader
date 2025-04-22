Shader "Custom/SpriteGlowPulse"
{
    Properties
    {
        [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _GlowStrength("Glow Strength", Float) = 0.2
        _GlowSpeed("Glow Speed", Float) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Sprite" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _GlowStrength;
            float _GlowSpeed;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float t = _Time.y* _GlowSpeed;
                float glow = (sin(t) * 0.5 + 0.5); // [0,1]
                float glowIntensity = lerp(1.0, 1.0 + _GlowStrength, glow);

                fixed4 texColor = tex2D(_MainTex, IN.uv) * _Color;
                texColor.rgb *= glowIntensity;

                return texColor;
            }
            ENDCG
        }
    }
}
