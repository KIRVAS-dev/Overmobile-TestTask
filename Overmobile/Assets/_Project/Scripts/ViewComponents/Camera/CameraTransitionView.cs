using Core.Camera;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace ViewComponents.Camera
{
    public sealed class CameraTransitionView
        : MonoBehaviour,
          ICameraTransitionView
    {
        [SerializeField] private UnityCamera _camera;
        [SerializeField] private CameraTransitionConfig _config;

        public async UniTask PlayTransitionAsync(CancellationToken cancellationToken)
        {
            Validate();

            _camera.transform.DOKill();
            _camera.DOKill();

            _camera.transform.position = _config.StartPosition;
            _camera.orthographicSize = _config.StartOrthographicSize;

            await UniTask.Delay(TimeSpan.FromSeconds(_config.DelayBeforeTransition), cancellationToken: cancellationToken);

            float duration = CalculateTransitionDuration();

            Tween moveTween = _camera.transform.DOMove(_config.TargetPosition, duration).SetEase(_config.Ease);
            Tween sizeTween = _camera.DOOrthoSize(_config.TargetOrthographicSize, duration).SetEase(_config.Ease);

            await AwaitTweensAsync(moveTween, sizeTween, cancellationToken);
        }

        private void OnDestroy()
        {
            _camera.transform.DOKill();
            _camera.DOKill();
        }

        private async UniTask AwaitTweensAsync(Tween moveTween, Tween sizeTween, CancellationToken cancellationToken)
        {
            await using CancellationTokenRegistration registration = cancellationToken.Register(() =>
                {
                    _camera.transform.DOKill();
                    _camera.DOKill();
                }
            );

            while (moveTween.IsActive()
             || sizeTween.IsActive())
            {
                await UniTask.Yield();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        private float CalculateTransitionDuration()
        {
            float positionDistance = Vector3.Distance(_config.StartPosition, _config.TargetPosition);
            float sizeDelta = Mathf.Abs(_config.TargetOrthographicSize - _config.StartOrthographicSize);
            float positionDuration = positionDistance / _config.TransitionSpeed;
            float sizeDuration = sizeDelta / _config.TransitionSpeed;

            return Mathf.Max(positionDuration, sizeDuration);
        }

        private void Validate()
        {
            if (_config == null)
            {
                throw new MissingCameraTransitionConfigException(gameObject.name);
            }

            if (!_camera.orthographic)
            {
                throw new CameraTransitionOrthographicRequiredException(gameObject.name);
            }

            if (_config.DelayBeforeTransition < 0f)
            {
                throw new CameraTransitionInvalidDelayException(gameObject.name, _config.DelayBeforeTransition);
            }
        }
    }
}
