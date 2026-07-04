using ExtendedExceptions;
using Rendering.TargetSelection;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.TargetSelection
{
    public sealed class SpriteTargetSelectionHighlighter : TargetSelectionHighlighter
    {
        protected override uint HighlightRenderingLayerMask => TargetSelectionRenderingLayers.Sprite;
        protected override IReadOnlyList<Renderer> Renderers => _spriteRenderers;

        [SerializeField] private SpriteRenderer[] _spriteRenderers;

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNullOrEmpty(
                _spriteRenderers,
                () => new MissingTargetSelectionFieldException(nameof(_spriteRenderers), gameObject.name)
            );

            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer == null)
                {
                    throw new MissingTargetSelectionFieldException(nameof(_spriteRenderers), gameObject.name);
                }
            }
        }
    }
}
