Shader "UI/SmokeFbmRise_PhysLUT_OptC_NoiseTex_Curl"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // ---- 布局 / 上升 ----
        _Center       ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Scale        ("Domain Scale", Float) = 2.0
        _RiseSpeed    ("Rise Speed (uv/sec)", Float) = 0.25

        // ---- FBM 域扭曲（保留 FBM，Octaves=2）----
        _WarpFreq1    ("Warp1 Frequency", Float) = 1.5
        _WarpAmp1     ("Warp1 Amplitude", Float) = 0.50
        _WarpFlow1    ("Warp1 Flow Speed", Float) = 0.10
        _WarpFreq2    ("Warp2 Frequency", Float) = 3.0
        _WarpAmp2     ("Warp2 Amplitude", Float) = 0.30
        _WarpFlow2    ("Warp2 Flow Speed", Float) = 0.15

        // ---- 主噪声改为贴图采样 ----
        _NoiseTex     ("Noise Tex (Grayscale)", 2D) = "white" {}
        _NoiseScale   ("Noise Scale", Float) = 2.0
        _NoiseScroll  ("Noise Scroll (uv/sec)", Vector) = (0.0, 0.03, 0, 0)
        _Contrast     ("Density Contrast", Float) = 1.15

        // ---- 顶部淡出 / 收窄 ----
        _TopFadeStartY("Top Fade Start (uv.y)", Range(0,1)) = 0.35
        _TopFadeEndY  ("Top Fade End   (uv.y)", Range(0,1)) = 0.65
        _WidthBase    ("Base Half-Width (uv)", Float) = 0.25
        _WidthMinMul  ("Min Width Mult", Range(0.05,1)) = 0.5
        _NarrowStartY ("Narrow Start (uv.y)", Range(0,1)) = 0.2
        _NarrowEndY   ("Narrow End   (uv.y)", Range(0,1)) = 0.85
        _SideSoftness ("Side Softness (pow)", Float) = 2.5

        // ---- 写实体渲（吸收/散射） ----
        _OpticalDepthScale ("OpticalDepth Scale", Float) = 2.0
        _SigmaA            ("Absorption σa (RGB)", Color) = (1,1,1,1)
        _SmokeAlbedo       ("Smoke Albedo (RGB)", Color) = (0.82, 0.85, 0.90, 1)
        _AmbientColor      ("Ambient Color", Color) = (0.10, 0.11, 0.12, 1)
        _LightDirUV        ("Light Dir in UV", Vector) = (0.7, -0.4, 0, 0)
        _LightColor        ("Light Color", Color) = (1.0, 0.98, 0.95, 1)
        _ScatterStrength   ("Scatter Strength", Float) = 0.8
        _PhaseSharpness    ("Phase Sharpness", Float) = 4.0

        // ---- Gradient LUT（横向 1×N）----
        _GradientTex       ("Gradient LUT (1xN, Horizontal)", 2D) = "white" {}
        _LUTBlend          ("LUT Blend 0..1", Range(0,1)) = 0.6
        _LUTOffset         ("LUT Offset", Float) = 0.0
        _LUTScale          ("LUT Scale",  Float) = 1.0
        _LUTGamma          ("LUT Gamma",  Float) = 1.0

        // ---- 抗雪花 / 卷曲控制 ----
        _NoiseMipBias   ("Noise Mip Bias", Range(-1,2)) = 0.0
        //_CurlStrength   ("Vorticity Strength (uv)", Range(0,0.08)) = 0.02
        //_CurlMix        ("Vorticity Mix", Range(0,1)) = 0.25
        //_CurlOscAmp     ("Vorticity Osc Amp", Range(0,1)) = 0.0
        //_CurlOscFreq    ("Vorticity Osc Freq", Float) = 1.7
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

            sampler2D _MainTex; float4 _MainTex_TexelSize;
            float4 _Color, _Center; half _Scale, _RiseSpeed;

            // FBM warp
            half  _WarpFreq1, _WarpAmp1, _WarpFlow1;
            half  _WarpFreq2, _WarpAmp2, _WarpFlow2;

            // NoiseTex
            sampler2D _NoiseTex;
            half _NoiseScale; half4 _NoiseScroll; half _Contrast;

            // fades
            half  _TopFadeStartY, _TopFadeEndY;
            half  _WidthBase, _WidthMinMul, _NarrowStartY, _NarrowEndY, _SideSoftness;

            // physical
            half  _OpticalDepthScale;
            half4 _SigmaA, _SmokeAlbedo, _AmbientColor;
            half4 _LightDirUV, _LightColor;
            half  _ScatterStrength, _PhaseSharpness;

            // LUT
            sampler2D _GradientTex;
            half  _LUTBlend, _LUTOffset, _LUTScale, _LUTGamma;

            // anti-sparkle / curl
            half _NoiseMipBias;
            //half _CurlStrength, _CurlMix, _CurlOscAmp, _CurlOscFreq;

            float4 _ClipRect;

            struct appdata_t { float4 vertex:POSITION; float4 color:COLOR; float2 texcoord:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; float4 worldPosition:TEXCOORD1; };

            v2f vert(appdata_t v){
                v2f o; o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                o.worldPosition = v.vertex;
                return o;
            }

            // ---- 轻量 hash + value noise（仅用于 FBM warp）----
            inline half hash21(half2 p){ p=frac(p*half2(123.34h,345.45h)); p+=dot(p,p+half2(34.345h,34.345h)); return frac(p.x*p.y); }
            inline half valueNoise(half2 p){
                half2 i=floor(p), f=frac(p);
                half a=hash21(i), b=hash21(i+half2(1,0)), c=hash21(i+half2(0,1)), d=hash21(i+half2(1,1));
                half2 u=f*f*f*(f*(f*6.0h-15.0h)+10.0h);
                return lerp(lerp(a,b,u.x), lerp(c,d,u.x), u.y);
            }
            inline half fbm2_oct2(half2 p, half lac, half gain){
                half amp=0.5h, sum=amp*valueNoise(p); p*=lac; amp*=gain; sum+=amp*valueNoise(p); return sum;
            }
            inline half adjustContrast01(half x, half c){ return saturate(pow(saturate(x), max(c, 0.0001h))); }

            // ---- 使用屏幕导数的采样（避免 warp 后雪花） ----
            inline half  SampleNoiseGrad(half2 uv) {
                half2 dx = ddx(uv), dy = ddy(uv);
                // 手动 mip 偏置：在 grad 基础上额外偏一点 mip 可更稳
                #if defined(SHADER_API_D3D11) || defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES3)
                    // 有 tex2Dgrad，仅在支持的平台生效
                    return tex2Dgrad(_NoiseTex, uv, dx, dy).r;
                #else
                    return tex2Dbias(_NoiseTex, half4(uv,0,_NoiseMipBias)).r;
                #endif
            }

            inline half3 SampleGradientLUT(half density){
                half u = saturate(pow(saturate(density * _LUTScale + _LUTOffset), max(_LUTGamma, 1e-4h)));
                return tex2D(_GradientTex, half2(u, 0.5h)).rgb;
            }

            fixed4 frag(v2f i):SV_Target
            {
                // 1) 局部域 + 上升
                half2 local = (i.uv - _Center.xy) * _Scale;
                local.y -= _Time.y * _RiseSpeed;

                // 2) FBM 域扭曲（两层，Octaves=2）
                half2 p = local;
                half2 w1 = half2(
                    fbm2_oct2(p * _WarpFreq1 + _Time.y * _WarpFlow1 + half2(37.3h,17.2h), 2.0h, 0.5h),
                    fbm2_oct2(p * _WarpFreq1 + _Time.y * _WarpFlow1 + half2(11.7h,41.1h), 2.0h, 0.5h)
                );
                p += (w1 - 0.5h) * _WarpAmp1;

                half2 w2 = half2(
                    fbm2_oct2(p * _WarpFreq2 + _Time.y * _WarpFlow2 + half2(17.9h,7.1h), 2.0h, 0.5h),
                    fbm2_oct2(p * _WarpFreq2 + _Time.y * _WarpFlow2 + half2(5.5h,23.3h), 2.0h, 0.5h)
                );
                p += (w2 - 0.5h) * _WarpAmp2;

                // 3) 主噪声（贴图） + 顶部淡出 / 收窄
                half2 noiseUV = p * _NoiseScale + _Time.y * _NoiseScroll.xy;
                half n = SampleNoiseGrad(noiseUV); // 0..1

                half topFade = 1.0h - smoothstep(_TopFadeStartY, _TopFadeEndY, i.uv.y);
                half tNarrow   = saturate((i.uv.y - _NarrowStartY) / max(_NarrowEndY - _NarrowStartY, 1e-4h));
                half halfWidth = _WidthBase * lerp(1.0h, _WidthMinMul, tNarrow);
                half xDist     = abs(i.uv.x - _Center.x) / max(halfWidth, 1e-4h);
                half sideFade  = exp(-pow(xDist, _SideSoftness));
                half fade      = saturate(topFade * sideFade);

                // ===================== 气体密度：核心值 =====================
                half density = adjustContrast01(n * fade, _Contrast);
                /*
                // —— 轻量“卷”增强：密度梯度的垂直方向做微偏移再采一次 —— 
                if (_CurlStrength > 0.0001h || _CurlOscAmp > 0.0001h)
                {
                    half2 gradD   = half2(ddx(density), ddy(density));
                    half2 curlDir = normalize(half2(gradD.y, -gradD.x) + 1e-6h);
                    half osc      = (_CurlOscAmp <= 0.0h) ? 1.0h : (0.75h + 0.25h * sin(_Time.y * _CurlOscFreq));
                    half  curlAmt = _CurlStrength * saturate(density) * osc;  // 随密度略增强
                    half2 noiseUV_curl = noiseUV + curlDir * curlAmt;

                    half n_curl = SampleNoiseGrad(noiseUV_curl);
                    half density_curl = adjustContrast01(n_curl * fade, _Contrast);

                    density = lerp(density, density_curl, _CurlMix);
                }*/

                // 4) 写实颜色（吸收 + 环境）
                half3 sigmaA = _SigmaA.rgb;
                half  od     = density * _OpticalDepthScale;
                half3 T      = exp(-sigmaA * od);
                half3 ambient= _AmbientColor.rgb * T;
                half3 baseScatter = _SmokeAlbedo.rgb * (1.0h - T);
                half3 colorPhys   = ambient + baseScatter;

                // 5) 单次散射（ddx/ddy 梯度，一直开启）
                half2 grad = half2(ddx(density), ddy(density));
                half2 L    = normalize(_LightDirUV.xy);
                half  NdotL= saturate(dot(-normalize(grad + 1e-6h), L));
                half  phase= pow(NdotL, _PhaseSharpness);
                half3 scatterCol = _LightColor.rgb * _ScatterStrength * phase * density * (1.0h - T);
                colorPhys += scatterCol;

                // 6) Gradient LUT 混合
                half3 colorLUT   = SampleGradientLUT(density);
                half3 colorFinal = lerp(colorPhys, colorLUT, _LUTBlend);

                // 7) Alpha（体积近似）
                half T_luma     = dot(T, half3(0.299h, 0.587h, 0.114h));
                half smokeAlpha = saturate(1.0h - T_luma);

                half alpha = 1.0h;
                alpha = tex2D(_MainTex, i.uv).a;

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

