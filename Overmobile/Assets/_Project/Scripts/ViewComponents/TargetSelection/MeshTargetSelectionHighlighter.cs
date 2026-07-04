using ExtendedExceptions;
using Rendering.TargetSelection;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.TargetSelection
{
    public sealed class MeshTargetSelectionHighlighter : TargetSelectionHighlighter
    {
        protected override uint HighlightRenderingLayerMask => TargetSelectionRenderingLayers.Mesh;
        protected override IReadOnlyList<Renderer> Renderers => _renderers;

        [SerializeField] private Renderer[] _renderers;

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNullOrEmpty(
                _renderers,
                () => new MissingTargetSelectionFieldException(nameof(_renderers), gameObject.name)
            );

            foreach (Renderer highlightRenderer in _renderers)
            {
                if (highlightRenderer == null)
                {
                    throw new MissingTargetSelectionFieldException(nameof(_renderers), gameObject.name);
                }
            }
        }
    }
}
