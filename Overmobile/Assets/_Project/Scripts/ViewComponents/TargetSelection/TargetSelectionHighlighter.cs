using Rendering.TargetSelection;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.TargetSelection
{
    public abstract class TargetSelectionHighlighter : MonoBehaviour
    {
        private readonly List<Material> _maskMaterials = new List<Material>();

        private bool _isHighlightEnabled;

        protected abstract uint HighlightRenderingLayerMask { get; }

        protected abstract IReadOnlyList<Renderer> Renderers { get; }

        public void Initialize(TargetSelectionConfig config)
        {
            foreach (Renderer highlightRenderer in Renderers)
            {
                foreach (Material maskMaterial in highlightRenderer.materials)
                {
                    maskMaterial.SetColor(TargetSelectionShaderProperties.FillColor, config.FillColor);
                    maskMaterial.SetColor(TargetSelectionShaderProperties.OutlineColor, config.OutlineColor);
                    maskMaterial.SetFloat(TargetSelectionShaderProperties.Progress, 0f);
                    _maskMaterials.Add(maskMaterial);
                }
            }
        }

        public void EnableHighlight()
        {
            if (_isHighlightEnabled)
            {
                return;
            }

            _isHighlightEnabled = true;
            TargetSelectionHighlightActivity.Register();

            uint mask = HighlightRenderingLayerMask;

            foreach (Renderer highlightRenderer in Renderers)
            {
                highlightRenderer.renderingLayerMask |= mask;
            }
        }

        public void DisableHighlight()
        {
            if (!_isHighlightEnabled)
            {
                return;
            }

            _isHighlightEnabled = false;
            TargetSelectionHighlightActivity.Unregister();

            uint mask = HighlightRenderingLayerMask;

            foreach (Renderer highlightRenderer in Renderers)
            {
                highlightRenderer.renderingLayerMask &= ~mask;
            }
        }

        public void SetProgress(float progress)
        {
            foreach (Material maskMaterial in _maskMaterials)
            {
                maskMaterial.SetFloat(TargetSelectionShaderProperties.Progress, progress);
            }
        }

        private void OnDestroy()
        {
            if (_isHighlightEnabled)
            {
                _isHighlightEnabled = false;
                TargetSelectionHighlightActivity.Unregister();
            }

            foreach (Material maskMaterial in _maskMaterials)
            {
                Destroy(maskMaterial);
            }

            _maskMaterials.Clear();
        }
    }
}
