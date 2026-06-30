using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace ViewComponents.Presentation
{
    public static class PresentationSequenceHelper
    {
        private static readonly int SurfacePropertyId = Shader.PropertyToID("_Surface");
        private static readonly int BlendPropertyId = Shader.PropertyToID("_Blend");
        private static readonly int SrcBlendPropertyId = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendPropertyId = Shader.PropertyToID("_DstBlend");
        private static readonly int SrcBlendAlphaPropertyId = Shader.PropertyToID("_SrcBlendAlpha");
        private static readonly int DstBlendAlphaPropertyId = Shader.PropertyToID("_DstBlendAlpha");
        private static readonly int ZWritePropertyId = Shader.PropertyToID("_ZWrite");
        private static readonly int BaseColorPropertyId = Shader.PropertyToID("_BaseColor");

        private const string SurfaceTypeTransparentKeyword = "_SURFACE_TYPE_TRANSPARENT";
        private const string BaseColorPropertyName = "_BaseColor";
        private const string ColorPropertyName = "_Color";

        public static async UniTask AwaitTweenAsync(PresentationStepSequence owner, Tween tween,
            CancellationToken cancellationToken)
        {
            owner.RegisterTween(tween);

            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                owner.GetCancellationTokenOnDestroy()
            );

            CancellationToken linkedToken = linkedCts.Token;

            await using CancellationTokenRegistration registration = linkedToken.Register(() =>
                {
                    if (tween != null
                     && tween.IsActive())
                    {
                        tween.Kill();
                    }
                }
            );

            while (tween.IsActive())
            {
                await UniTask.Yield(linkedToken);
            }

            linkedToken.ThrowIfCancellationRequested();
        }

        public static async UniTask AwaitTweensAsync(PresentationStepSequence owner, IReadOnlyList<Tween> tweens,
            CancellationToken cancellationToken)
        {
            if (tweens == null
             || tweens.Count == 0)
            {
                return;
            }

            UniTask[] awaitTasks = new UniTask[tweens.Count];

            for (int i = 0; i < tweens.Count; i++)
            {
                awaitTasks[i] = AwaitTweenAsync(owner, tweens[i], cancellationToken);
            }

            await UniTask.WhenAll(awaitTasks);
        }

        public static async UniTask FadeSpriteRenderersAsync(PresentationStepSequence owner, SpriteRenderer[] spriteRenderers,
            float targetAlpha, float durationSeconds, Ease ease, CancellationToken cancellationToken)
        {
            if (spriteRenderers == null
             || spriteRenderers.Length == 0)
            {
                return;
            }

            UniTask[] fadeTasks = new UniTask[spriteRenderers.Length];

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];

                if (spriteRenderer == null)
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(AlphaFadeStepDefinition),
                        $"Sprite renderer at index {i} is missing"
                    );
                }

                Tween fadeTween = spriteRenderer.DOFade(targetAlpha, durationSeconds).SetEase(ease);
                fadeTasks[i] = AwaitTweenAsync(owner, fadeTween, cancellationToken);
            }

            await UniTask.WhenAll(fadeTasks);
        }

        public static async UniTask FadeRenderersAsync(PresentationStepSequence owner, Renderer[] renderers, float targetAlpha,
            float durationSeconds, Ease ease, CancellationToken cancellationToken)
        {
            if (renderers == null
             || renderers.Length == 0)
            {
                return;
            }

            List<UniTask> fadeTasks = new List<UniTask>();

            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Renderer renderer = renderers[rendererIndex];

                if (renderer == null)
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(AlphaFadeStepDefinition),
                        $"Renderer at index {rendererIndex} is missing"
                    );
                }

                Material[] materials = renderer.materials;

                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    Material material = materials[materialIndex];

                    if (material == null)
                    {
                        throw new InvalidPresentationStepDefinitionException(
                            nameof(AlphaFadeStepDefinition),
                            $"Material at index {materialIndex} on renderer at index {rendererIndex} is missing"
                        );
                    }

                    PrepareMaterialForAlphaFade(material);

                    string colorPropertyName = ResolveMaterialColorPropertyName(material);
                    Tween fadeTween = material.DOFade(targetAlpha, colorPropertyName, durationSeconds).SetEase(ease);
                    fadeTasks.Add(AwaitTweenAsync(owner, fadeTween, cancellationToken));
                }
            }

            await UniTask.WhenAll(fadeTasks);
        }

        private static void PrepareMaterialForAlphaFade(Material material)
        {
            if (!material.HasProperty(SurfacePropertyId))
            {
                return;
            }

            material.SetFloat(SurfacePropertyId, value: 1f);

            if (material.HasProperty(BlendPropertyId))
            {
                material.SetFloat(BlendPropertyId, value: 0f);
            }

            material.SetInt(SrcBlendPropertyId, (int)BlendMode.SrcAlpha);
            material.SetInt(DstBlendPropertyId, (int)BlendMode.OneMinusSrcAlpha);

            if (material.HasProperty(SrcBlendAlphaPropertyId))
            {
                material.SetInt(SrcBlendAlphaPropertyId, (int)BlendMode.One);
                material.SetInt(DstBlendAlphaPropertyId, (int)BlendMode.OneMinusSrcAlpha);
            }

            material.SetInt(ZWritePropertyId, value: 0);
            material.EnableKeyword(SurfaceTypeTransparentKeyword);
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        private static string ResolveMaterialColorPropertyName(Material material)
        {
            return material.HasProperty(BaseColorPropertyId)
                ? BaseColorPropertyName
                : ColorPropertyName;
        }

        public static async UniTask FadeCanvasGroupsAsync(PresentationStepSequence owner, CanvasGroup[] canvasGroups,
            float targetAlpha, float durationSeconds, Ease ease, CancellationToken cancellationToken)
        {
            if (canvasGroups == null
             || canvasGroups.Length == 0)
            {
                return;
            }

            UniTask[] fadeTasks = new UniTask[canvasGroups.Length];

            for (int i = 0; i < canvasGroups.Length; i++)
            {
                CanvasGroup canvasGroup = canvasGroups[i];

                if (canvasGroup == null)
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(AlphaFadeStepDefinition),
                        $"Canvas group at index {i} is missing"
                    );
                }

                Tween fadeTween = canvasGroup.DOFade(targetAlpha, durationSeconds).SetEase(ease);
                fadeTasks[i] = AwaitTweenAsync(owner, fadeTween, cancellationToken);
            }

            await UniTask.WhenAll(fadeTasks);
        }
    }
}
