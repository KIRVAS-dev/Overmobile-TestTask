using Rendering.VignetteBlur.Api;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Rendering.VignetteBlur
{
    sealed class VignetteBlurRenderPass : ScriptableRenderPass
    {
        private const float UrpVignetteIntensityScale = 3f;
        private const float UrpVignetteSmoothnessScale = 5f;

        private static readonly int BlurIntensityId = Shader.PropertyToID("_BlurIntensity");
        private static readonly int MaxRadiusId = Shader.PropertyToID("_MaxRadius");
        private static readonly int ClearRadiusId = Shader.PropertyToID("_ClearRadius");
        private static readonly int VignetteCenterIntensityId = Shader.PropertyToID("_VignetteCenterIntensity");
        private static readonly int VignetteSmoothnessRoundnessId = Shader.PropertyToID("_VignetteSmoothnessRoundness");

        private readonly MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();
        private Material _material;

        public void Setup(Material blurMaterial)
        {
            if (blurMaterial == null)
            {
                throw new MissingVignetteBlurMaterialException(nameof(VignetteBlurRenderPass));
            }

            _material = blurMaterial;
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_material == null)
            {
                throw new MissingVignetteBlurMaterialException(nameof(VignetteBlurRenderPass));
            }

            VignetteBlurVolume vignetteBlur = VolumeManager.instance.stack.GetComponent<VignetteBlurVolume>();

            if (vignetteBlur == null
             || !vignetteBlur.IsActive())
            {
                return;
            }

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle source = resourceData.activeColorTexture;

            if (!source.IsValid())
            {
                return;
            }

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            Vignette vignette = VolumeManager.instance.stack.GetComponent<Vignette>();

            if (vignette == null)
            {
                throw new MissingVignetteVolumeException(cameraData.camera.name);
            }

            CalculateVignetteMaskParameters(
                vignette,
                cameraData.camera.pixelWidth,
                cameraData.camera.pixelHeight,
                out Vector4 centerIntensity,
                out Vector4 smoothnessRoundness
            );

            _propertyBlock.SetFloat(BlurIntensityId, vignetteBlur.BlurIntensity.value);
            _propertyBlock.SetFloat(MaxRadiusId, vignetteBlur.MaxRadius.value);
            _propertyBlock.SetFloat(ClearRadiusId, vignetteBlur.ClearRadius.value);
            _propertyBlock.SetVector(VignetteCenterIntensityId, centerIntensity);
            _propertyBlock.SetVector(VignetteSmoothnessRoundnessId, smoothnessRoundness);

            TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "VignetteBlur";
            destinationDesc.clearBuffer = false;
            destinationDesc.msaaSamples = MSAASamples.None;
            destinationDesc.depthBufferBits = 0;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    destination,
                    _material,
                    shaderPass: 0,
                    _propertyBlock
                ),
                "Vignette Blur"
            );

            resourceData.cameraColor = destination;
        }

        private static void CalculateVignetteMaskParameters(Vignette vignette, int width, int height, out Vector4 centerIntensity,
            out Vector4 smoothnessRoundness)
        {
            Vector2 center = vignette.center.value;
            float intensity = vignette.intensity.value * UrpVignetteIntensityScale;
            float smoothness = vignette.smoothness.value * UrpVignetteSmoothnessScale;

            float roundness = vignette.rounded.value
                ? width / (float)height
                : 1f;

            centerIntensity = new Vector4(center.x, center.y, intensity, w: 0f);
            smoothnessRoundness = new Vector4(smoothness, roundness, z: 0f, w: 0f);
        }
    }
}
