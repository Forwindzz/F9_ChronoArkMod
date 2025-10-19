Shader "UI/GasSpread_Swirl"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // —— 基本布局 / 外扩速度 ——
        _Center       ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Scale        ("Domain Scale", Float) = 2.0
        _SpreadSpeed  ("Spread Speed (uv/sec)", Float) = 0.35

        // —— Swirl 涡旋（核心） ——
        _SwirlStrength("Swirl Strength", Range(0,3)) = 1.0     // 螺旋强度
        _SwirlFalloff ("Swirl Falloff (power)", Range(0,2)) = 1.0 // 1/r^k 的 k
        _SwirlClampR  ("Inner Clamp Radius", Range(0.001,0.2)) = 0.02 // 防止中心过强
        _RadialBias   ("Outward Bias (0..1)", Range(0,1)) = 0.65      // 与外向分量的混合

        // —— Swirl 噪声调制（让涡流更自然，不死板） ——
        _SwirlNoiseFreq ("Swirl Noise Freq", Float) = 2.0
        _SwirlNoiseAmp  ("Swirl Noise Amp",  Range(0,1)) = 0.35
        _SwirlNoiseFlow ("Swirl Noise Flow", Float) = 0.15

        // —— FBM 域扭曲（保留） ——
        _WarpFreq1    ("Warp1 Frequency", Float) = 1.5
        _WarpAmp1     ("Warp1 Amplitude", Float) = 0.50
        _WarpFlow1    ("Warp1 Flow Speed", Float) = 0.10

        // —— 主噪声（密度） ——
        _NoiseTex     ("Noise Tex (Grayscale)", 2D) = "white" {}
        _NoiseScale   ("Noise Scale", Float) = 2.0
        _NoiseScroll  ("Noise Scroll (uv/sec)", Vector) = (0.0, 0.03, 0, 0)
        _NoiseMipBias ("Noise Mip Bias", Range(-1,2)) = 0.3
        _Contrast     ("Density Contrast", Float) = 1.15

        // —— 径向淡化 + 耗散 ——
        _RadialFadeStart ("Radial Fade Start (dist)", Range(0,1)) = 0.30
        _RadialFadeEnd   ("Radial Fade End   (dist)", Range(0,1)) = 0.70
        _Dissipation     ("Radial Dissipation", Float) = 1.2

        // —— 写实渲（吸收/散射/LUT）——
        _OpticalDepthScale ("OpticalDepth Scale", Float) = 2.0
        _SigmaA            ("Absorption σa (RGB)", Color) = (1,1,1,1)
        _SmokeAlbedo       ("Smoke Albedo (RGB)", Color) = (0.82, 0.85, 0.90, 1)
        _AmbientColor      ("Ambient Color", Color) = (0.10, 0.11, 0.12, 1)
        _LightDirUV        ("Light Dir in UV", Vector) = (0.7, -0.4, 0, 0)
        _LightColor        ("Light Color", Color) = (1.0, 0.98, 0.95, 1)
        _ScatterStrength   ("Scatter Strength", Float) = 0.8
        _PhaseSharpness    ("Phase Sharpness", Float) = 4.0

        _GradientTex       ("Gradient LUT (1xN, Horizontal)", 2D) = "white" {}
        _LUTBlend          ("LUT Blend 0..1", Range(0,1)) = 0.6
        _LUTOffset         ("LUT Offset", Float) = 0.0
        _LUTScale          ("LUT Scale",  Float) = 1.0
        _LUTGamma          ("LUT Gamma",  Float) = 1.0
    }

    SubShader
    {
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Stencil{ Ref 1 Comp Always Pass Keep Fail Keep ZFail Keep }
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "UI"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex; float4 _MainTex_TexelSize; float4 _ClipRect;
            float4 _Color, _Center; half _Scale, _SpreadSpeed;

            // swirl
            half _SwirlStrength, _SwirlFalloff, _SwirlClampR, _RadialBias;
            half _SwirlNoiseFreq, _SwirlNoiseAmp, _SwirlNoiseFlow;

            // warp1
            half  _WarpFreq1, _WarpAmp1, _WarpFlow1;

            // density noise
            sampler2D _NoiseTex;
            half _NoiseScale; half4 _NoiseScroll; half _NoiseMipBias; half _Contrast;

            // fades
            half _RadialFadeStart, _RadialFadeEnd, _Dissipation;

            // physical & LUT
            half  _OpticalDepthScale;
            half4 _SigmaA, _SmokeAlbedo, _AmbientColor;
            half4 _LightDirUV, _LightColor;
            half  _ScatterStrength, _PhaseSharpness;
            sampler2D _GradientTex;
            half  _LUTBlend, _LUTOffset, _LUTScale, _LUTGamma;

            struct appdata_t { float4 vertex:POSITION; float4 color:COLOR; float2 texcoord:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float4 worldPosition:TEXCOORD1; };

            v2f vert(appdata_t v){ v2f o; o.pos=UnityObjectToClipPos(v.vertex); o.uv=v.texcoord; o.worldPosition=v.vertex; return o; }

            inline half adjustContrast01(half x, half c){ return saturate(pow(saturate(x), max(c, 1e-4h))); }

            inline half  SampleNoiseGrad(half2 uv) {
                half2 dx = ddx(uv), dy = ddy(uv);
                #if defined(SHADER_API_D3D11) || defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES3)
                    return tex2Dgrad(_NoiseTex, uv, dx, dy).r;
                #else
                    return tex2Dbias(_NoiseTex, half4(uv,0,_NoiseMipBias)).r;
                #endif
            }

            inline half3 SampleGradientLUT(half density){
                half u = saturate(pow(saturate(density * _LUTScale + _LUTOffset), max(_LUTGamma, 1e-4h)));
                return tex2D(_GradientTex, half2(u, 0.5h)).rgb;
            }

            // —— 计算 Swirl 速度向量（切向/螺旋），避免 atan2 接缝 ——
            inline half2 SwirlVec(half2 local)
            {
                half r = max(length(local), 1e-6h);
                // 噪声调制（让 swirl 强度/方向有局部变化）
                half n = SampleNoiseGrad(local * _SwirlNoiseFreq + _Time.y * _SwirlNoiseFlow);
                half mod = 1.0h + _SwirlNoiseAmp * (n - 0.5h);

                // 切向向量（与径向正交）
                half2 tang = half2(-local.y, local.x) / r;

                // 1/r^k 衰减 + 中心夹紧，避免 r→0 爆炸
                half rEff = max(r, _SwirlClampR);
                half fall = 1.0h / pow(rEff, max(_SwirlFalloff, 0.0h));

                return normalize(tang) * (_SwirlStrength * fall * mod);
            }

            fixed4 frag(v2f i):SV_Target
            {
                // 1) 域与基础方向
                half2 local = (i.uv - _Center.xy) * _Scale;
                half  r     = length(local) + 1e-6h;
                half2 dirR  = local / r; // 外向

                // 2) 组合流向：外向 + swirl（切向）
                half2 vSwirl = SwirlVec(local);
                half2 v      = normalize( lerp(vSwirl, dirR, _RadialBias) );

                // 3) 回溯采样：形成“向外扩散且带卷曲”的错觉
                half2 p = local - v * (_SpreadSpeed * _Time.y);

                // 4) 域扭曲（Warp1 保留）
                half2 w1a = half2(
                    SampleNoiseGrad(p * _WarpFreq1 + _Time.y * _WarpFlow1 + half2(37.3h,17.2h)),
                    SampleNoiseGrad(p * _WarpFreq1 + _Time.y * _WarpFlow1 + half2(11.7h,41.1h))
                );
                p += (w1a - 0.5h) * _WarpAmp1;

                // 5) 主噪声密度
                half2 noiseUV = p * _NoiseScale + _Time.y * _NoiseScroll.xy;
                half n = SampleNoiseGrad(noiseUV); // 0..1

                // 6) 径向淡化 + 耗散（越远越淡）
                half r_uv    = length(i.uv - _Center.xy);
                half edgeFade= 1.0h - smoothstep(_RadialFadeStart, _RadialFadeEnd, r_uv);
                half dissip  = exp(-_Dissipation * r_uv);
                half fade    = saturate(edgeFade * dissip);

                // 7) 密度 & 对比
                half density = adjustContrast01(n * fade, _Contrast);

                // 8) 写实着色
                half3 sigmaA = _SigmaA.rgb;
                half  od     = density * _OpticalDepthScale;
                half3 T      = exp(-sigmaA * od);
                half3 ambient= _AmbientColor.rgb * T;
                half3 baseScatter = _SmokeAlbedo.rgb * (1.0h - T);
                half3 colorPhys   = ambient + baseScatter;

                half2 grad = half2(ddx(density), ddy(density));
                half2 L    = normalize(_LightDirUV.xy);
                half  NdotL= saturate(dot(-normalize(grad + 1e-6h), L));
                half  phase= pow(NdotL, _PhaseSharpness);
                half3 scatterCol = _LightColor.rgb * _ScatterStrength * phase * density * (1.0h - T);
                colorPhys += scatterCol;

                // 9) LUT
                half3 colorLUT   = SampleGradientLUT(density);
                half3 colorFinal = lerp(colorPhys, colorLUT, _LUTBlend);

                // 10) Alpha
                half T_luma     = dot(T, half3(0.299h, 0.587h, 0.114h));
                half smokeAlpha = saturate(1.0h - T_luma);

                half alpha = tex2D(_MainTex, i.uv).a;
                #ifdef UNITY_UI_CLIP_RECT
                    alpha *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                return half4(colorFinal, smokeAlpha) * half4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
