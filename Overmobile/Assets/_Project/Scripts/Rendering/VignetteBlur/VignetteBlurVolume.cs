using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.VignetteBlur
{
    [Serializable]
    [VolumeComponentMenu("Post-processing/Vignette Blur")]
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    public sealed class VignetteBlurVolume
        : VolumeComponent,
          IPostProcessComponent
    {
        [Tooltip("Blur mix strength at the outer vignette band.")]
        public ClampedFloatParameter BlurIntensity =
            new ClampedFloatParameter(value: 0.85f, min: 0f, max: 1f, overrideState: true);

        [Tooltip("Vignette-space radius of the sharp center. Blur starts outside this boundary.")]
        public ClampedFloatParameter ClearRadius = new ClampedFloatParameter(value: 0.25f, min: 0f, max: 1f, overrideState: true);

        [Tooltip("Gaussian blur sample radius in texels.")]
        public ClampedFloatParameter MaxRadius = new ClampedFloatParameter(value: 2.5f, min: 0.1f, max: 8f, overrideState: true);

        public bool IsActive()
        {
            return BlurIntensity.value > 0f && MaxRadius.value > 0f;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
