using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewComponents.WaterWaves
{
    public sealed class WaterWavesInstance : MonoBehaviour
    {
        private const float HIDDEN_ALPHA = 0f;
        private const float VISIBLE_ALPHA = 1f;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private WaterWavesInstanceConfig _config;

        public void Validate()
        {
            if (_spriteRenderer == null)
            {
                throw new MissingWaterWavesSpriteRendererException(gameObject.name);
            }

            if (_config == null)
            {
                throw new MissingWaterWavesInstanceConfigException(gameObject.name);
            }

            _config.Validate();
        }

        public async UniTask PlayAsync(
            Vector3 normalizedMoveDirection,
            float moveSpeed,
            float lifetime,
            Vector3 worldPosition,
            CancellationToken cancellationToken)
        {
            try
            {
                _spriteRenderer.sprite = GetRandomSprite();

                Color color = _spriteRenderer.color;
                color.a = HIDDEN_ALPHA;
                _spriteRenderer.color = color;

                transform.position = worldPosition;
                gameObject.SetActive(true);

                Vector3 destination = worldPosition + normalizedMoveDirection * (moveSpeed * lifetime);
                Tween fadeInTween = _spriteRenderer.DOFade(VISIBLE_ALPHA, _config.FadeInDuration).SetEase(_config.FadeInEase);
                Tween moveTween = transform.DOMove(destination, lifetime).SetEase(_config.MoveEase);
                float fadeOutStartDelay = Mathf.Max(_config.FadeInDuration, lifetime - _config.FadeOutDuration);

                Tween fadeOutTween = _spriteRenderer
                   .DOFade(HIDDEN_ALPHA, _config.FadeOutDuration)
                   .SetEase(_config.FadeOutEase)
                   .SetDelay(fadeOutStartDelay);

                await UniTask.WhenAll(
                    AwaitTweenAsync(fadeInTween, cancellationToken),
                    AwaitTweenAsync(moveTween, cancellationToken),
                    AwaitTweenAsync(fadeOutTween, cancellationToken)
                );
            }
            finally
            {
                if (this != null)
                {
                    Transform movementTransform = transform;
                    KillRegisteredTweens(tween: null, movementTransform, _spriteRenderer);
                    gameObject.SetActive(false);
                }
            }
        }

        private async UniTask AwaitTweenAsync(Tween tween, CancellationToken cancellationToken)
        {
            Transform movementTransform = transform;
            SpriteRenderer spriteRenderer = _spriteRenderer;

            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                this.GetCancellationTokenOnDestroy()
            );

            CancellationToken linkedToken = linkedCts.Token;

            await using CancellationTokenRegistration registration = linkedToken.Register(() =>
                {
                    KillRegisteredTweens(tween, movementTransform, spriteRenderer);
                }
            );

            while (tween.IsActive())
            {
                await UniTask.Yield();
            }

            linkedToken.ThrowIfCancellationRequested();
        }

        private Sprite GetRandomSprite()
        {
            Sprite[] sprites = _config.Sprites;
            int spriteIndex = Random.Range(minInclusive: 0, sprites.Length);

            return sprites[spriteIndex];
        }

        private void OnDestroy()
        {
            Transform movementTransform = transform;
            KillRegisteredTweens(tween: null, movementTransform, _spriteRenderer);
        }

        private void KillRegisteredTweens(Tween tween, Transform movementTransform, SpriteRenderer spriteRenderer)
        {
            tween?.Kill();

            if (movementTransform != null)
            {
                movementTransform.DOKill();
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.DOKill();
            }
        }
    }
}
