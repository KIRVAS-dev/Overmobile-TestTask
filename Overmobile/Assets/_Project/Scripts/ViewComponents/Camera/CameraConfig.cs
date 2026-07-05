using DG.Tweening;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Camera
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Project/Configs/Camera/Camera Config")]
    public sealed class CameraConfig : ScriptableObject
    {
        public float DelayBeforeTransition => _delayBeforeTransition;
        public float TransitionSpeed => _transitionSpeed;
        public Ease Ease => _ease;
        public Vector3 StartPosition => _startPosition;
        public float StartOrthographicSize => _startOrthographicSize;
        public Vector3 TargetPosition => _targetPosition;
        public float TargetOrthographicSize => _targetOrthographicSize;
        public float ScrollSensitivity => _scrollSensitivity;
        public float PinchSensitivity => _pinchSensitivity;
        public float ZoomSmoothSpeed => _zoomSmoothSpeed;

        [Min(0f)]
        [SerializeField] private float _delayBeforeTransition;

        [Min(0f)]
        [SerializeField] private float _transitionSpeed = 2f;
        [SerializeField] private Ease _ease = Ease.InOutQuad;

        [Header("Start Parameters")]
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private float _startOrthographicSize = 5f;

        [Header("Target Parameters")]
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private float _targetOrthographicSize = 8f;

        [Header("Zoom")]
        [SerializeField] private float _scrollSensitivity = 0.05f;
        [SerializeField] private float _pinchSensitivity = 0.005f;

        [Min(0.01f)]
        [SerializeField] private float _zoomSmoothSpeed = 12f;

        public void Validate()
        {
            Guard.AgainstNegative(
                _delayBeforeTransition,
                () => new InvalidCameraConfigValueException(nameof(_delayBeforeTransition), _delayBeforeTransition)
            );

            Guard.AgainstNonPositive(
                _transitionSpeed,
                () => new InvalidCameraConfigValueException(nameof(_transitionSpeed), _transitionSpeed)
            );

            Guard.AgainstNonPositive(
                _startOrthographicSize,
                () => new InvalidCameraConfigValueException(nameof(_startOrthographicSize), _startOrthographicSize)
            );

            Guard.AgainstNonPositive(
                _targetOrthographicSize,
                () => new InvalidCameraConfigValueException(nameof(_targetOrthographicSize), _targetOrthographicSize)
            );

            Guard.AgainstNonPositive(
                _scrollSensitivity,
                () => new InvalidCameraConfigValueException(nameof(_scrollSensitivity), _scrollSensitivity)
            );

            Guard.AgainstNonPositive(
                _pinchSensitivity,
                () => new InvalidCameraConfigValueException(nameof(_pinchSensitivity), _pinchSensitivity)
            );

            Guard.AgainstNonPositive(
                _zoomSmoothSpeed,
                () => new InvalidCameraConfigValueException(nameof(_zoomSmoothSpeed), _zoomSmoothSpeed)
            );

            Guard.AgainstInvalidRange(
                _startOrthographicSize,
                _targetOrthographicSize,
                () => new InvalidCameraConfigOrthographicRangeException(_startOrthographicSize, _targetOrthographicSize)
            );
        }
    }
}
