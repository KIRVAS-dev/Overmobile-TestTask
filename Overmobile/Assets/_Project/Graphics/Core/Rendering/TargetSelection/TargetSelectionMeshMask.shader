Shader "Hidden/Rendering/TargetSelectionMeshMask"
{
    Properties
    {
        _TargetSelectionFillColor ("Fill Color", Color) = (0, 0, 0, 0)
        _TargetSelectionOutlineColor ("Outline Color", Color) = (0, 0, 0, 0)
        _TargetSelectionProgress ("Progress", Float) = 0
    }

    HLSLINCLUDE
    #pragma target 3.5

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
        half4 _TargetSelectionFillColor;
        half4 _TargetSelectionOutlineColor;
        half _TargetSelectionProgress;
    CBUFFER_END

    struct Attributes {
        float4 positionOS : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings {
        float4 positionCS : SV_POSITION;
        float linearEyeDepth : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.linearEyeDepth = LinearEyeDepth(positionWS, GetWorldToViewMatrix());
        return output;
    }

    struct MaskPassOutput {
        half4 fill : SV_Target0;
        half4 outline : SV_Target1;
        float targetDepth : SV_Target2;
    };

    MaskPassOutput Frag(Varyings input)
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        MaskPassOutput output;
        output.fill = half4(_TargetSelectionFillColor.rgb, _TargetSelectionFillColor.a * _TargetSelectionProgress);
        output.outline = half4(_TargetSelectionOutlineColor.rgb, _TargetSelectionOutlineColor.a * _TargetSelectionProgress);
        output.targetDepth = input.linearEyeDepth;
        return output;
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZWrite On
        ZTest LEqual
        Cull Back

        Pass
        {
            Name "TargetSelectionMeshMask"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
