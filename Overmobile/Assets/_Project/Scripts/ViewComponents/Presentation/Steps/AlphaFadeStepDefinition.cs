using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class AlphaFadeStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private PresentationActiveMode _mode;
        [SerializeField] private SpriteRenderer[] _spriteRenderers = Array.Empty<SpriteRenderer>();
        [SerializeField] private Renderer[] _renderers = Array.Empty<Renderer>();
        [SerializeField] private CanvasGroup[] _canvasGroups = Array.Empty<CanvasGroup>();
        [SerializeField] private float _durationSeconds = 1f;
        [SerializeField] private Ease _ease = Ease.Linear;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            ValidateFadeTargets();

            if (_durationSeconds <= 0f)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(AlphaFadeStepDefinition),
                    "Duration must be greater than zero"
                );
            }

            float targetAlpha = _mode == PresentationActiveMode.Enable
                ? 1f
                : 0f;

            await UniTask.WhenAll(
                PresentationSequenceHelper.FadeSpriteRenderersAsync(
                    context.Owner,
                    _spriteRenderers,
                    targetAlpha,
                    _durationSeconds,
                    _ease,
                    cancellationToken
                ),
                PresentationSequenceHelper.FadeRenderersAsync(
                    context.Owner,
                    _renderers,
                    targetAlpha,
                    _durationSeconds,
                    _ease,
                    cancellationToken
                ),
                PresentationSequenceHelper.FadeCanvasGroupsAsync(
                    context.Owner,
                    _canvasGroups,
                    targetAlpha,
                    _durationSeconds,
                    _ease,
                    cancellationToken
                )
            );
        }

        private void ValidateFadeTargets()
        {
            int spriteRendererCount = _spriteRenderers.Length;
            int rendererCount = _renderers.Length;
            int canvasGroupCount = _canvasGroups.Length;

            if (spriteRendererCount == 0
             && rendererCount == 0
             && canvasGroupCount == 0)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(AlphaFadeStepDefinition),
                    "At least one fade target is required"
                );
            }
        }
    }
}
