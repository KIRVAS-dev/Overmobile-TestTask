using DG.Tweening;
using ExtendedExceptions;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraShakeView
        : MonoBehaviour,
          ICameraShakeView
    {
        [SerializeField] private UnityCamera _camera;
        [SerializeField] private CameraShakeConfig _config;

        private void Awake()
        {
            Validate();
        }

        public void PlayShake()
        {
            Transform cameraTransform = _camera.transform;
            cameraTransform.DOKill();

            cameraTransform.DOShakePosition(
                _config.Duration,
                _config.Strength,
                _config.Vibrato,
                _config.Randomness,
                snapping: false,
                _config.FadeOut
            );
        }

        private void OnDestroy()
        {
            if (_camera != null)
            {
                _camera.transform.DOKill();
            }
        }

        private void Validate()
        {
            Guard.AgainstNull(_camera, () => new MissingCameraShakeFieldException(nameof(_camera), gameObject.name));
            Guard.AgainstNull(_config, () => new MissingCameraShakeFieldException(nameof(_config), gameObject.name));

            if (!_camera.orthographic)
            {
                throw new CameraShakeOrthographicRequiredException(gameObject.name);
            }

            _config.Validate();
        }
    }
}
