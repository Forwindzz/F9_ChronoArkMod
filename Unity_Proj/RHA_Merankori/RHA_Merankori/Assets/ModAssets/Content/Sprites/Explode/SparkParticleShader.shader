Shader "Custom/ParticleBreathingAlpha"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        _Size("Particle Size", Float) = 1.0
        _BreathSpeed("Breathing Speed", Float) = 1.0
        _BreathStrength("Breathing Strength", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Lighting Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Properties
            sampler2D _MainTex;
            float4 _Color;
            float _Size;
            float _BreathSpeed;
            float _BreathStrength;

            // Vertex input structure
            struct appdata_t
            {
                float3 vertex : POSITION;
                //float4 color : COLOR;
                float4 uv : TEXCOORD0; //xy->uv, z->lifetime, w=> stable random
                float uv2 : TEXCOORD1; //x->speed
            };

            // Fragment output structure
            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float uv2 : TEXCOORD1; //x->speed
            };

            // Vertex shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2=v.uv2;
                return o;
            }

            // Fragment shader
            half4 frag(v2f i) : SV_Target
            {
                // Calculate the breathing effect based on time and world position
                float lifeTime = i.uv.z;
                float randomValue = i.uv.w;
                float speed = i.uv2.x;
                float lifeTimeAlpha = smoothstep(0.0, 0.1,lifeTime) * (1.0 - smoothstep(0.5, 1.0, lifeTime));
                float timeFactor = sin(randomValue*lifeTime * _BreathSpeed*speed) * 0.5 + 0.5;
                float breathingAlpha = (0.5 + timeFactor * _BreathStrength*0.5)*lifeTimeAlpha;

                half4 texColor = tex2D(_MainTex, i.uv.xy) * _Color;
                texColor.a *= breathingAlpha; // Apply breathing effect to alpha

                // You can modify the color here too, e.g., make the particle brighter or change hue
                //texColor.rgb *= breathingAlpha; // Optional: make particle brighter when breathing in
                //return fixed4(i.lifetimeAlpha,i.lifetimeAlpha,0,1.0);
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
