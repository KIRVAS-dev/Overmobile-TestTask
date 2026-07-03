using Core.Camera;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExtendedExceptions;
using System;
using System.Threading;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraTransitionView
        : MonoBehaviour,
          ICameraTransitionView
    {
        [SerializeField] private UnityCamera _camera;
        [SerializeField] private CameraTransitionConfig _config;

        private void Awake()
        {
            Validate();
        }

        public async UniTask PlayTransitionAsync(CancellationToken cancellationToken)
        {
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
            KillRegisteredTweens(tween: null, _camera.transform, _camera);
        }

        private async UniTask AwaitTweensAsync(
            Tween moveTween,
            Tween sizeTween,
            CancellationToken cancellationToken)
        {
            Transform cameraTransform = _camera.transform;
            UnityCamera camera = _camera;

            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                this.GetCancellationTokenOnDestroy()
            );

            CancellationToken linkedToken = linkedCts.Token;

            await using CancellationTokenRegistration registration = linkedToken.Register(() =>
                {
                    KillRegisteredTweens(tween: null, cameraTransform, camera);
                }
            );

            while (moveTween.IsActive()
             || sizeTween.IsActive())
            {
                await UniTask.Yield(linkedToken);
            }

            linkedToken.ThrowIfCancellationRequested();
        }

        private void KillRegisteredTweens(
            Tween tween,
            Transform cameraTransform,
            UnityCamera camera)
        {
            tween?.Kill();

            if (cameraTransform != null)
            {
                cameraTransform.DOKill();
            }

            if (camera != null)
            {
                camera.DOKill();
            }
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
            Guard.AgainstNull(_camera, () => new MissingCameraTransitionFieldException(nameof(_camera), gameObject.name));
            Guard.AgainstNull(_config, () => new MissingCameraTransitionFieldException(nameof(_config), gameObject.name));

            if (!_camera.orthographic)
            {
                throw new CameraTransitionOrthographicRequiredException(gameObject.name);
            }

            Guard.AgainstNegative(
                _config.DelayBeforeTransition,
                () => new InvalidCameraTransitionValueException(
                    nameof(_config.DelayBeforeTransition),
                    gameObject.name,
                    _config.DelayBeforeTransition
                )
            );
        }
    }
}
