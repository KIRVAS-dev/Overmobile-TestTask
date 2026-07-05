using Cysharp.Threading.Tasks;
using DG.Tweening;
using ExtendedExceptions;
using System;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraTransitionView
        : MonoBehaviour,
          ICameraTransitionView
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private CameraConfig _config;

        private ICameraOrthoFitProvider _cameraOrthoFitProvider;
        private ICameraConfinerView _cameraConfinerView;

        [Inject]
        public void Construct(ICameraOrthoFitProvider cameraOrthoFitProvider, ICameraConfinerView cameraConfinerView)
        {
            _cameraOrthoFitProvider = cameraOrthoFitProvider;
            _cameraConfinerView = cameraConfinerView;
        }

        private void Awake()
        {
            Validate();
        }

        public async UniTask PlayTransitionAsync(CancellationToken cancellationToken)
        {
            float startOrthographicSize = _cameraOrthoFitProvider.MinOrthographicSize;
            float targetOrthographicSize = _cameraOrthoFitProvider.FitOrthographicSize;

            Transform cinemachineCameraTransform = _cinemachineCamera.transform;
            cinemachineCameraTransform.DOKill();

            _cameraConfinerView.BeginBoundsFollow();

            try
            {
                cinemachineCameraTransform.position = _config.StartPosition;
                SetOrthographicSize(startOrthographicSize);
                _cameraConfinerView.UpdateBoundsFollow(_config.StartPosition);

                await UniTask.Delay(TimeSpan.FromSeconds(_config.DelayBeforeTransition), cancellationToken: cancellationToken);

                float duration = CalculateTransitionDuration(
                    startOrthographicSize,
                    targetOrthographicSize,
                    _config.StartPosition,
                    _config.TargetPosition
                );

                Tween moveTween = cinemachineCameraTransform.DOMove(_config.TargetPosition, duration).SetEase(_config.Ease);
                Tween sizeTween = CreateOrthographicSizeTween(targetOrthographicSize, duration);

                await AwaitTweensAsync(moveTween, sizeTween, cinemachineCameraTransform, cancellationToken);

                KillRegisteredTweens(moveTween, cinemachineCameraTransform);
                sizeTween.Kill();

                cinemachineCameraTransform.position = _config.TargetPosition;
            }
            finally
            {
                _cameraConfinerView.EndBoundsFollow();
            }
        }

        private void OnDestroy()
        {
            if (_cinemachineCamera != null)
            {
                KillRegisteredTweens(tween: null, _cinemachineCamera.transform);
            }
        }

        private async UniTask AwaitTweensAsync(
            Tween moveTween,
            Tween sizeTween,
            Transform cinemachineCameraTransform,
            CancellationToken cancellationToken)
        {
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                this.GetCancellationTokenOnDestroy()
            );

            CancellationToken linkedToken = linkedCts.Token;

            await using CancellationTokenRegistration registration = linkedToken.Register(() =>
                {
                    KillRegisteredTweens(tween: null, cinemachineCameraTransform);
                }
            );

            while (moveTween.IsActive()
             || sizeTween.IsActive())
            {
                _cameraConfinerView.UpdateBoundsFollow(cinemachineCameraTransform.position);

                await UniTask.Yield(linkedToken);
            }

            linkedToken.ThrowIfCancellationRequested();
        }

        private Tween CreateOrthographicSizeTween(float targetSize, float duration)
        {
            CinemachineCamera cinemachineCamera = _cinemachineCamera;

            return DOTween
               .To(() => cinemachineCamera.Lens.OrthographicSize, value => SetOrthographicSize(value), targetSize, duration)
               .SetEase(_config.Ease);
        }

        private void SetOrthographicSize(float orthographicSize)
        {
            CinemachineCamera cinemachineCamera = _cinemachineCamera;
            LensSettings lens = cinemachineCamera.Lens;
            lens.OrthographicSize = orthographicSize;
            cinemachineCamera.Lens = lens;
        }

        private void KillRegisteredTweens(Tween tween, Transform cinemachineCameraTransform)
        {
            tween?.Kill();

            if (cinemachineCameraTransform != null)
            {
                cinemachineCameraTransform.DOKill();
            }

            if (_cinemachineCamera != null)
            {
                DOTween.Kill(_cinemachineCamera);
            }
        }

        private float CalculateTransitionDuration(
            float startOrthographicSize,
            float targetOrthographicSize,
            Vector3 startPosition,
            Vector3 targetPosition)
        {
            float positionDistance = Vector3.Distance(startPosition, targetPosition);
            float sizeDelta = Mathf.Abs(targetOrthographicSize - startOrthographicSize);
            float positionDuration = positionDistance / _config.TransitionSpeed;
            float sizeDuration = sizeDelta / _config.TransitionSpeed;

            return Mathf.Max(positionDuration, sizeDuration);
        }

        private void Validate()
        {
            _config.Validate();

            Guard.AgainstNull(
                _cinemachineCamera,
                () => new MissingCameraConfigFieldException(nameof(_cinemachineCamera), gameObject.name)
            );

            Guard.AgainstNull(_config, () => new MissingCameraConfigFieldException(nameof(_config), gameObject.name));

            if (_cinemachineCamera.Lens.ModeOverride != LensSettings.OverrideModes.Orthographic)
            {
                throw new CameraOrthographicRequiredException(gameObject.name);
            }
        }
    }
}
