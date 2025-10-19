Shader "UI/EdgeRadialSweepFlash_SobelOnly_RadialBoost"
{
    Properties
    {
        [PerRendererData]_MainTex("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // 高亮色与强度
        _HighlightColor ("Highlight Color", Color) = (1,0.95,0.75,1)
        _FlashIntensity ("Flash Intensity", Range(0,5)) = 2.0

        // 扫光（从中心向外）
        _CenterUV ("Center UV (x,y)", Vector) = (0.5,0.5,0,0)
        _SweepSpeed ("Sweep Speed (cycles/sec)", Range(0.1,5)) = 1.0
        _BandWidth ("Band Width (radial)", Range(0.01,0.5)) = 0.15

        // 间歇门控
        _BurstRate ("Burst Rate (bursts/sec)", Range(0.1,5)) = 1.0
        _BurstDuty ("Burst Duty (0~1)", Range(0.05,1)) = 0.4

        // 细小闪烁
        _JitterScale ("Jitter Scale", Range(2,80)) = 30
        _JitterSpeed ("Jitter Speed", Range(0,20)) = 8
        _JitterStrength ("Jitter Strength", Range(0,1)) = 0.35

        // Sobel 边缘（基于主贴图亮度）
        _EdgeThreshold ("Edge Threshold", Range(0,1)) = 0.15
        _EdgeSoftness  ("Edge Softness",  Range(0.001,0.5)) = 0.08
        _EdgeGain      ("Edge Gain",      Range(0.5,5)) = 1.6

        // —— 距中心增强（r 越大越强）——
        _RadialStart ("Radial Boost Start", Range(0,1)) = 0.2
        _RadialEnd   ("Radial Boost End",   Range(0,1)) = 1.0
        _RadialPower ("Radial Boost Power", Range(0.1,8)) = 1.5
        _RadialBoost ("Radial Boost Strength", Range(0,5)) = 1.2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            half4  _Color;

            half4  _HighlightColor;
            half   _FlashIntensity;

            half2  _CenterUV;
            half   _SweepSpeed;
            half   _BandWidth;

            half   _BurstRate;
            half   _BurstDuty;

            half   _JitterScale;
            half   _JitterSpeed;
            half   _JitterStrength;

            half   _EdgeThreshold;
            half   _EdgeSoftness;
            half   _EdgeGain;

            half   _RadialStart;
            half   _RadialEnd;
            half   _RadialPower;
            half   _RadialBoost;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            inline half luminance(half3 c) { return dot(c, half3(0.299h, 0.587h, 0.114h)); }

            // 简易 hash 噪声（用于细小闪烁）
            inline half hash21(half2 p)
            {
                p = frac(p * half2(123.34h, 456.21h));
                p += dot(p, p + 45.32h);
                return frac(p.x * p.y);
            }

            // Sobel 3×3（9 taps），基于亮度
            inline half edgeSobel(half2 uv)
            {
                float2 t = _MainTex_TexelSize.xy;

                half4 c00 = tex2D(_MainTex, uv + t * float2(-1,-1));
                half4 c01 = tex2D(_MainTex, uv + t * float2( 0,-1));
                half4 c02 = tex2D(_MainTex, uv + t * float2( 1,-1));
                half4 c10 = tex2D(_MainTex, uv + t * float2(-1, 0));
                half4 c11 = tex2D(_MainTex, uv + t * float2( 0, 0));
                half4 c12 = tex2D(_MainTex, uv + t * float2( 1, 0));
                half4 c20 = tex2D(_MainTex, uv + t * float2(-1, 1));
                half4 c21 = tex2D(_MainTex, uv + t * float2( 0, 1));
                half4 c22 = tex2D(_MainTex, uv + t * float2( 1, 1));

                half s00 = luminance(c00.rgb);
                half s01 = luminance(c01.rgb);
                half s02 = luminance(c02.rgb);
                half s10 = luminance(c10.rgb);
                half s11 = luminance(c11.rgb);
                half s12 = luminance(c12.rgb);
                half s20 = luminance(c20.rgb);
                half s21 = luminance(c21.rgb);
                half s22 = luminance(c22.rgb);

                half gx = -s00 - 2*s10 - s20 + s02 + 2*s12 + s22;
                half gy = -s00 - 2*s01 - s02 + s20 + 2*s21 + s22;

                half mag = sqrt(gx*gx + gy*gy) * _EdgeGain;
                return smoothstep(_EdgeThreshold, _EdgeThreshold + _EdgeSoftness, mag);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                half4 baseCol = tex2D(_MainTex, uv) * _Color;

                // 1) 边缘强度（Sobel）
                half edge = edgeSobel(uv);

                // 2) 半径 r（以 _CenterUV 为中心，0~1）
                half2 d = (half2)uv - _CenterUV;
                half  r = saturate(length(d) * 1.41421356h);

                // 3) 扫光（向外推进的带）
                half sweepPhase = frac(_Time.y * _SweepSpeed);
                half band = 1.0h - saturate(abs(r - sweepPhase) / max(_BandWidth, 1e-4h));
                band = band * band; // 轻微加权

                // 4) 间歇门控
                half burstPhase = frac(_Time.y * _BurstRate);
                half gate = step(0.0h, _BurstDuty - burstPhase);

                // 5) 细小闪烁
                half jitter = hash21(floor((half2)uv * _JitterScale) + floor(_Time.y * _JitterSpeed));
                jitter = lerp(1.0h, (0.8h + 0.2h * jitter), _JitterStrength);

                // 6) 距中心增强：r 在 [Start, End] 内从 0 过渡到 1，再以 Power 调曲线，最后乘加强度
                half radialMask = saturate((r - _RadialStart) / max(_RadialEnd - _RadialStart, 1e-4h));
                radialMask = pow(radialMask, _RadialPower);
                half radialGain = lerp(1.0h, 1.0h + _RadialBoost, radialMask);

                // 最终强度
                half flash = edge * band * gate * jitter * _FlashIntensity * radialGain;

                // 叠加高亮颜色
                baseCol.rgb += flash * _HighlightColor.rgb;

                return baseCol;
            }
            ENDCG
        }
    }
}
