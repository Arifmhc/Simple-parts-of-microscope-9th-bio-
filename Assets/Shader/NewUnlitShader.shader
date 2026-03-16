Shader "Custom/URP/BottomOnly_TaperedColumns"
{
    Properties
    {
        _Color ("Column Color", Color) = (0.2, 0.8, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 5
        _Power ("Column Power", Range(0,5)) = 2

        _ColumnCount ("Column Count", Range(1,50)) = 24

        _MaxWidth ("Max Width (Near Gap)", Range(0.01,0.5)) = 0.12
        _TipWidth ("Tip Width (Bottom)", Range(0.001,0.2)) = 0.03
        _TaperStrength ("Taper Strength", Range(0.1,5)) = 2

        _GapPosition ("Gap Position (UV Y)", Range(0,1)) = 0.6
        _GapSize ("Gap Size", Range(0,0.4)) = 0.15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend One One
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _GlowIntensity;
            float _Power;
            float _ColumnCount;
            float _MaxWidth;
            float _TipWidth;
            float _TaperStrength;
            float _GapPosition;
            float _GapSize;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // ❌ Remove everything above the gap
                if (i.uv.y >= _GapPosition)
                    discard;

                // ❌ Remove gap zone
                if (i.uv.y >= (_GapPosition - _GapSize))
                    discard;

                // -----------------------------
                // COLUMN STRIPES
                // -----------------------------
                float stripe = frac(i.uv.x * _ColumnCount);
                float centerDist = abs(stripe - 0.5);

                // -----------------------------
                // LOCAL HEIGHT (0 = near gap)
                // -----------------------------
                float localHeight =
                    saturate(
                        (_GapPosition - _GapSize - i.uv.y) /
                        (_GapPosition - _GapSize)
                    );

                // -----------------------------
                // WIDTH TAPER (thick → thin)
                // -----------------------------
                float width =
                    lerp(_MaxWidth, _TipWidth,
                         pow(localHeight, _TaperStrength));

                float column =
                    smoothstep(width, 0.0, centerDist);

                // -----------------------------
                // FINAL OUTPUT
                // -----------------------------
                float energy = column * _Power;
                float glow = energy * _GlowIntensity;

                return half4(_Color.rgb * glow, glow);
            }
            ENDHLSL
        }
    }
}