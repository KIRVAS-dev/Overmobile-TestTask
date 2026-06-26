using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.VignetteBlur
{
    public sealed class VignetteBlurRendererFeature : ScriptableRendererFeature
    {
        private const string FeatureName = nameof(VignetteBlurRendererFeature);

        [SerializeField] private Shader _shader;
        [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        private Material _material;
        private VignetteBlurRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new VignetteBlurRenderPass { renderPassEvent = _renderPassEvent };

            if (_shader == null)
            {
                throw new MissingVignetteBlurShaderException(FeatureName);
            }

            _material = CoreUtils.CreateEngineMaterial(_shader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            if (_material == null)
            {
                throw new MissingVignetteBlurMaterialException(FeatureName);
            }

            _renderPass.Setup(_material);
            renderer.EnqueuePass(_renderPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_material);
        }
    }
}
