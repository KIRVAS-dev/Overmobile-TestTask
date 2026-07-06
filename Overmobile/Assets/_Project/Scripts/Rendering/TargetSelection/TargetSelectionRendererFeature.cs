using ExtendedExceptions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering.TargetSelection
{
    public sealed class TargetSelectionRendererFeature : ScriptableRendererFeature
    {
        private const string FeatureName = nameof(TargetSelectionRendererFeature);

        [SerializeField] private Shader _meshMaskShader;
        [SerializeField] private Shader _spriteMaskShader;
        [SerializeField] private Shader _compositeShader;
        [SerializeField] private TargetSelectionRenderingConfig _renderingConfig;
        [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        private Material _compositeMaterial;
        private TargetSelectionRenderPass _renderPass;

        public override void Create()
        {
            Validate();

            _compositeMaterial = CoreUtils.CreateEngineMaterial(_compositeShader);

            _compositeMaterial.SetFloat(
                TargetSelectionShaderProperties.OutlineThickness,
                _renderingConfig.OutlineThicknessPixels
            );

            _renderPass =
                new TargetSelectionRenderPass(_compositeMaterial, _meshMaskShader, _spriteMaskShader)
                {
                    renderPassEvent = _renderPassEvent
                };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!TargetSelectionHighlightActivity.HasActiveHighlights)
            {
                return;
            }

            if (_compositeMaterial == null)
            {
                throw new MissingTargetSelectionMaterialException(FeatureName);
            }

            renderer.EnqueuePass(_renderPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_compositeMaterial);
        }

        private void Validate()
        {
            Guard.AgainstNull(_meshMaskShader, () => new MissingTargetSelectionShaderException(FeatureName, "mesh mask"));
            Guard.AgainstNull(_spriteMaskShader, () => new MissingTargetSelectionShaderException(FeatureName, "sprite mask"));
            Guard.AgainstNull(_compositeShader, () => new MissingTargetSelectionShaderException(FeatureName, "composite"));
            Guard.AgainstNull(_renderingConfig, () => new MissingTargetSelectionRenderingConfigException(FeatureName));

            _renderingConfig.Validate();
        }
    }
}
