using ExtendedExceptions;
using Unity.Cinemachine;
using UnityEngine;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraShakeView
        : MonoBehaviour,
          ICameraShakeView
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private CinemachineImpulseListener _impulseListener;
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {
            Validate();
        }

        public void PlayShake()
        {
            _impulseSource.GenerateImpulseWithVelocity(_impulseSource.DefaultVelocity);
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _cinemachineCamera,
                () => new MissingCameraShakeFieldException(nameof(_cinemachineCamera), gameObject.name)
            );

            Guard.AgainstNull(
                _impulseSource,
                () => new MissingCameraShakeFieldException(nameof(_impulseSource), gameObject.name)
            );

            Guard.AgainstNull(
                _impulseListener,
                () => new MissingCameraShakeFieldException(nameof(_impulseListener), gameObject.name)
            );

            if (_cinemachineCamera.Lens.ModeOverride != LensSettings.OverrideModes.Orthographic)
            {
                throw new CameraShakeOrthographicRequiredException(gameObject.name);
            }
        }
    }
}
