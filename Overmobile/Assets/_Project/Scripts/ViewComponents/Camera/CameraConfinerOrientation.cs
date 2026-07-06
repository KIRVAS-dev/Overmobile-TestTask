using ExtendedExceptions;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraConfinerOrientation
        : MonoBehaviour,
          ICameraConfinerView,
          ICameraOrthoFitProvider,
          ICameraBoundsFrameUpdater
    {
        [SerializeField] private BoxCollider2D _portraitBounds;
        [SerializeField] private BoxCollider2D _landscapeBounds;
        [SerializeField] private CinemachineConfiner2D _confiner;
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private CinemachineBrain _brain;
        [SerializeField] private CameraConfig _config;

        private Collider2D _lastAppliedBounds;
        private Vector2Int _lastScreenSize;
        private Vector3 _restBoundsPosition;
        private float _fitOrthographicSize;
        private float _minOrthographicSize;
        private bool _boundsFollowActive;

        private bool IsAlive => this;

        public float FitOrthographicSize => _fitOrthographicSize;
        public float MinOrthographicSize => _minOrthographicSize;
        public event Action OrthoFitChanged;

        public void BeginBoundsFollow()
        {
            _boundsFollowActive = true;
        }

        public void UpdateBoundsFollow(Vector3 followPosition)
        {
            if (!_boundsFollowActive
             || !IsAlive)
            {
                return;
            }

            transform.position = CameraConfinerHelper.CalculateBoundsFollowPosition(
                _restBoundsPosition,
                followPosition,
                _config.TargetPosition
            );
        }

        public void EndBoundsFollow()
        {
            if (!_boundsFollowActive)
            {
                return;
            }

            _boundsFollowActive = false;

            if (!IsAlive)
            {
                return;
            }

            transform.position = _restBoundsPosition;
            RefreshOrthoFit();
            SetOrthographicSize(_fitOrthographicSize);
            _confiner.InvalidateLensCache();
        }

        public void RefreshOrthoFit()
        {
            ApplyBoundsForCurrentOrientation(force: true, applyOrthoFit: false);
            OrthoFitChanged?.Invoke();
        }

        private void Awake()
        {
            Validate();
            _restBoundsPosition = transform.position;
        }

        private void OnDestroy()
        {
            _boundsFollowActive = false;
        }

        public void TickBoundsFrame()
        {
            if (!ScreenSizeChanged())
            {
                return;
            }

            ApplyBoundsForCurrentOrientation(force: false, applyOrthoFit: true);
        }

        private bool ScreenSizeChanged()
        {
            return _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height;
        }

        private void ApplyBoundsForCurrentOrientation(bool force, bool applyOrthoFit)
        {
            bool isLandscape = CameraConfinerHelper.IsLandscape(Screen.width, Screen.height);

            _portraitBounds.gameObject.SetActive(!isLandscape);
            _landscapeBounds.gameObject.SetActive(isLandscape);

            Collider2D targetBounds =
                CameraConfinerHelper.GetBoundsForOrientation(_portraitBounds, _landscapeBounds, isLandscape);

            float previousFitOrthographicSize = _fitOrthographicSize;

            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            if (force || targetBounds != _lastAppliedBounds)
            {
                _confiner.BoundingShape2D = targetBounds;
                _confiner.InvalidateBoundingShapeCache();
                _lastAppliedBounds = targetBounds;
            }

            RecalculateOrthoFit(targetBounds);

            if (applyOrthoFit)
            {
                ApplyOrthoFitOnOrientationChange(previousFitOrthographicSize);
            }
        }

        private Collider2D GetActiveBounds()
        {
            return CameraConfinerHelper.GetBoundsForOrientation(
                _portraitBounds,
                _landscapeBounds,
                CameraConfinerHelper.IsLandscape(Screen.width, Screen.height)
            );
        }

        private float GetScreenAspect()
        {
            return CameraConfinerHelper.GetScreenAspect(Screen.width, Screen.height);
        }

        private float GetClampAspect()
        {
            UnityCamera outputCamera = _brain.OutputCamera;

            return CameraConfinerHelper.GetClampAspect(outputCamera, GetScreenAspect());
        }

        private void RecalculateOrthoFit(Collider2D bounds)
        {
            float introStartToTargetRatio = _config.StartOrthographicSize / _config.TargetOrthographicSize;

            CameraConfinerHelper.OrthoFitSizes orthoFit = CameraConfinerHelper.CalculateOrthoFit(
                bounds.bounds,
                GetScreenAspect(),
                introStartToTargetRatio
            );

            _fitOrthographicSize = orthoFit.FitOrthographicSize;
            _minOrthographicSize = orthoFit.MinOrthographicSize;
        }

        private void ApplyOrthoFitOnOrientationChange(float previousFitOrthographicSize)
        {
            float clampedOrthographicSize = CameraConfinerHelper.RescaleAndClampOrthographicSize(
                _cinemachineCamera.Lens.OrthographicSize,
                previousFitOrthographicSize,
                _fitOrthographicSize,
                _minOrthographicSize
            );

            SetOrthographicSize(clampedOrthographicSize);
            ApplyClampedCameraPosition(clampedOrthographicSize);
            OrthoFitChanged?.Invoke();
        }

        private void ApplyClampedCameraPosition(float orthographicSize)
        {
            Transform cameraTransform = _cinemachineCamera.transform;

            cameraTransform.position = CameraZoomHelper.ClampCameraPositionToBounds(
                cameraTransform.position,
                GetActiveBounds().bounds,
                orthographicSize,
                GetClampAspect()
            );

            _confiner.InvalidateLensCache();
        }

        private void SetOrthographicSize(float orthographicSize)
        {
            CinemachineCamera cinemachineCamera = _cinemachineCamera;
            LensSettings lens = cinemachineCamera.Lens;
            lens.OrthographicSize = orthographicSize;
            cinemachineCamera.Lens = lens;
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _confiner,
                () => new MissingCameraConfinerOrientationFieldException(nameof(_confiner), gameObject.name)
            );

            Guard.AgainstNull(
                _cinemachineCamera,
                () => new MissingCameraConfinerOrientationFieldException(nameof(_cinemachineCamera), gameObject.name)
            );

            Guard.AgainstNull(_brain, () => new MissingCameraConfinerOrientationFieldException(nameof(_brain), gameObject.name));

            Guard.AgainstNull(
                _config,
                () => new MissingCameraConfinerOrientationFieldException(nameof(_config), gameObject.name)
            );

            Guard.AgainstNull(
                _portraitBounds,
                () => new MissingCameraConfinerOrientationFieldException(nameof(_portraitBounds), gameObject.name)
            );

            Guard.AgainstNull(
                _landscapeBounds,
                () => new MissingCameraConfinerOrientationFieldException(nameof(_landscapeBounds), gameObject.name)
            );

            _config.Validate();

            Guard.AgainstTrue(
                _cinemachineCamera.Lens.ModeOverride != LensSettings.OverrideModes.Orthographic,
                () => new CameraOrthographicRequiredException(gameObject.name)
            );
        }
    }
}
