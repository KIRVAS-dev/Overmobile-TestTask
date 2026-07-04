Shader "Hidden/Rendering/TargetSelectionComposite"
{
    HLSLINCLUDE
    #pragma target 3.5

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    TEXTURE2D_X(_TargetSelectionFill);
    SAMPLER(sampler_TargetSelectionFill);

    TEXTURE2D_X(_TargetSelectionOutline);
    SAMPLER(sampler_TargetSelectionOutline);

    TEXTURE2D_X_FLOAT(_TargetSelectionTargetDepth);
    SAMPLER(sampler_TargetSelectionTargetDepth);

    float _TargetSelectionOutlineThickness;

    static const float TargetSelectionOutlineDepthEpsilon = 0.01;
    static const half TargetSelectionPresenceEpsilon = 0.001;

    half4 SampleFill(float2 uv)
    {
        return SAMPLE_TEXTURE2D_X(_TargetSelectionFill, sampler_TargetSelectionFill, uv);
    }

    half4 SampleOutline(float2 uv)
    {
        return SAMPLE_TEXTURE2D_X(_TargetSelectionOutline, sampler_TargetSelectionOutline, uv);
    }

    float SampleTargetEyeDepth(float2 uv)
    {
        return SAMPLE_TEXTURE2D_X(_TargetSelectionTargetDepth, sampler_TargetSelectionTargetDepth, uv).r;
    }

    float SampleSceneEyeDepth(float2 uv)
    {
        float rawDepth = SampleSceneDepth(uv);

        if (IsPerspectiveProjection()) {
            return LinearEyeDepth(rawDepth, _ZBufferParams);
        }

        return LinearDepthToEyeDepth(rawDepth);
    }

    bool IsTargetOccluded(float sceneEyeDepth, float targetEyeDepth)
    {
        return sceneEyeDepth < targetEyeDepth - TargetSelectionOutlineDepthEpsilon;
    }

    half4 DetectOutline(float2 uv, float2 texelSize)
    {
        int sampleRadius = max(1, (int)round(_TargetSelectionOutlineThickness));
        float centerSceneEyeDepth = SampleSceneEyeDepth(uv);
        half4 edge = half4(0.0, 0.0, 0.0, 0.0);

        UNITY_LOOP
        for (int offsetY = -sampleRadius; offsetY <= sampleRadius; offsetY++) {
            UNITY_LOOP
            for (int offsetX = -sampleRadius; offsetX <= sampleRadius; offsetX++) {
                float2 sampleUv = uv + float2(offsetX, offsetY) * texelSize;
                half4 neighborOutline = SampleOutline(sampleUv);

                if (neighborOutline.a <= edge.a) {
                    continue;
                }

                float targetEyeDepth = SampleTargetEyeDepth(sampleUv);

                if (IsTargetOccluded(centerSceneEyeDepth, targetEyeDepth)) {
                    continue;
                }

                edge = neighborOutline;
            }
        }

        return edge;
    }

    half4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        float2 texelSize = _BlitTexture_TexelSize.xy;

        half4 sourceColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
        half4 fill = SampleFill(uv);
        half4 outline = SampleOutline(uv);
        half3 color = sourceColor.rgb;

        if (max(fill.a, outline.a) > TargetSelectionPresenceEpsilon) {
            color = lerp(color, fill.rgb, fill.a);
        }
        else {
            half4 edge = DetectOutline(uv, texelSize);
            color = lerp(color, edge.rgb, edge.a);
        }

        return half4(color, sourceColor.a);
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off
        Blend Off

        Pass
        {
            Name "TargetSelectionComposite"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
