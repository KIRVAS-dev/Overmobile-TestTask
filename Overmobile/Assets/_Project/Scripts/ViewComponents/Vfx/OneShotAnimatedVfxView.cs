using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Vfx
{
    [DisallowMultipleComponent]
    public sealed class OneShotAnimatedVfxView : MonoBehaviour
    {
        private const int AnimatorBaseLayer = 0;
        private const float AnimationEndNormalizedTimeThreshold = 0.99f;

        [SerializeField] private Animator _animator;

        private CancellationTokenSource _playCancellationTokenSource;


        private void OnEnable()
        {
            _playCancellationTokenSource?.Cancel();
            _playCancellationTokenSource?.Dispose();
            _playCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                this.GetCancellationTokenOnDestroy()
            );

            PlayAndDeactivateAsync(_playCancellationTokenSource.Token).Forget();
        }

        private void OnDisable()
        {
            _playCancellationTokenSource?.Cancel();
            _playCancellationTokenSource?.Dispose();
            _playCancellationTokenSource = null;
        }

        private async UniTaskVoid PlayAndDeactivateAsync(CancellationToken cancellationToken)
        {
            try
            {
                RestartCurrentState();

                await UniTask.WaitUntil(
                    IsAnimationComplete,
                    PlayerLoopTiming.Update,
                    cancellationToken
                );

                gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RestartCurrentState()
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(AnimatorBaseLayer);
            _animator.Play(stateInfo.shortNameHash, AnimatorBaseLayer, normalizedTime: 0f);
            _animator.Update(deltaTime: 0f);
        }

        private bool IsAnimationComplete()
        {
            if (_animator.IsInTransition(AnimatorBaseLayer))
            {
                return false;
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(AnimatorBaseLayer);

            if (stateInfo.loop)
            {
                throw new LoopingOneShotAnimatedVfxException(gameObject.name);
            }

            return stateInfo.normalizedTime >= AnimationEndNormalizedTimeThreshold;
        }
    }
}
