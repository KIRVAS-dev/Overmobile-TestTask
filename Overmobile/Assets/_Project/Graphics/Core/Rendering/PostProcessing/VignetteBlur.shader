Shader "Hidden/Rendering/VignetteBlur"
{
    HLSLINCLUDE

        #pragma target 3.5

        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _VignetteCenterIntensity;
            float4 _VignetteSmoothnessRoundness;
            float _BlurIntensity;
            float _MaxRadius;
            float _ClearRadius;
        CBUFFER_END

        const static int kTapCount = 3;
        const static float kOffsets[3] =
        {
            -1.33333333,
             0.00000000,
             1.33333333
        };
        const static half kCoeffs[3] =
        {
             0.35294118,
             0.29411765,
             0.35294118
        };

        float GetVignetteDistSq(float2 uv)
        {
            float2 center = _VignetteCenterIntensity.xy;
            float intensity = _VignetteCenterIntensity.z;
            float roundness = _VignetteSmoothnessRoundness.y;

            float2 dist = abs(uv - center) * intensity;
            dist.x *= roundness;
            return dot(dist, dist);
        }

        float GetVignetteFactorFromDistSq(float distSq)
        {
            float smoothness = _VignetteSmoothnessRoundness.x;
            return pow(saturate(1.0 - distSq), smoothness);
        }

        float GetBlurMask(float2 uv)
        {
            float distSq = GetVignetteDistSq(uv);
            float clearDistSq = _ClearRadius * _ClearRadius;

            if (distSq <= clearDistSq)
            {
                return 0.0;
            }

            float edgeAmount = 1.0 - GetVignetteFactorFromDistSq(distSq);
            float clearEdgeAmount = 1.0 - GetVignetteFactorFromDistSq(clearDistSq);
            float remapped = (edgeAmount - clearEdgeAmount) / max(1.0 - clearEdgeAmount, 1e-4);
            return saturate(remapped) * _BlurIntensity;
        }

        half4 SampleGaussianBlur(float2 uv)
        {
            float2 texelSize = _BlitTexture_TexelSize.xy * _MaxRadius;
            half4 color = 0.0;

            UNITY_UNROLL
            for (int x = 0; x < kTapCount; x++)
            {
                UNITY_UNROLL
                for (int y = 0; y < kTapCount; y++)
                {
                    float2 offset = float2(kOffsets[x], kOffsets[y]) * texelSize;
                    color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset)
                        * kCoeffs[x] * kCoeffs[y];
                }
            }

            return color;
        }

        half4 Frag(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

            half4 sharp = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
            half4 blurred = SampleGaussianBlur(uv);
            float blurMask = GetBlurMask(uv);
            return lerp(sharp, blurred, blurMask);
        }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "Vignette Blur"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
