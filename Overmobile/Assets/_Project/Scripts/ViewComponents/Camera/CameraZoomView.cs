using ExtendedExceptions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraZoomView
        : MonoBehaviour,
          ICameraZoomView,
          ICameraZoomFrameUpdater
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private CinemachineBrain _brain;
        [SerializeField] private CinemachineConfiner2D _confiner;
        [SerializeField] private CameraConfig _config;

        private ICameraOrthoFitProvider _cameraOrthoFitProvider;
        private InputActionAsset _actionsAsset;
        private InputAction _scrollWheelAction;
        private Vector2 _focalScreenPoint;
        private bool _isZoomEnabled;
        private float? _previousPinchDistance;
        private float _targetOrthographicSize;

        [Inject]
        public void Construct(InputActionAsset inputActionsAsset, ICameraOrthoFitProvider cameraOrthoFitProvider)
        {
            _actionsAsset = inputActionsAsset;
            _cameraOrthoFitProvider = cameraOrthoFitProvider;
        }

        public void StartListening()
        {
            if (_isZoomEnabled)
            {
                throw new CameraZoomViewAlreadyListeningException();
            }

            _cameraOrthoFitProvider.OrthoFitChanged += OnOrthoFitChanged;
            _scrollWheelAction.Enable();
            _isZoomEnabled = true;
            SyncZoomTargetToCurrentSize();
        }

        public void StopListening()
        {
            _cameraOrthoFitProvider.OrthoFitChanged -= OnOrthoFitChanged;
            _scrollWheelAction.Disable();
            _isZoomEnabled = false;
            _previousPinchDistance = null;
        }

        public void TickZoomFrame()
        {
            if (!_isZoomEnabled)
            {
                return;
            }

            if (TryApplyPinchZoom())
            {
                ApplySmoothZoom();

                return;
            }

            _previousPinchDistance = null;
            QueueScrollZoomTarget();
            ApplySmoothZoom();
        }

        private void SyncZoomTargetToCurrentSize()
        {
            _targetOrthographicSize = _cinemachineCamera.Lens.OrthographicSize;
        }

        private void Awake()
        {
            Validate();
            _isZoomEnabled = false;
        }

        private void OnOrthoFitChanged()
        {
            float currentOrthographicSize = _cinemachineCamera.Lens.OrthographicSize;

            _targetOrthographicSize = CameraZoomHelper.ClampOrthographicSize(
                currentOrthographicSize,
                _cameraOrthoFitProvider.MinOrthographicSize,
                _cameraOrthoFitProvider.FitOrthographicSize
            );
        }

        private bool TryApplyPinchZoom()
        {
            if (!CameraZoomHelper.TryGetActivePinch(out Vector2 firstTouchPosition, out Vector2 secondTouchPosition))
            {
                return false;
            }

            Vector2 focalScreenPoint = CameraZoomHelper.GetPinchMidpoint(firstTouchPosition, secondTouchPosition);
            float pinchDistance = Vector2.Distance(firstTouchPosition, secondTouchPosition);

            if (!_previousPinchDistance.HasValue)
            {
                _previousPinchDistance = pinchDistance;

                return true;
            }

            float pinchDelta = pinchDistance - _previousPinchDistance.Value;
            _previousPinchDistance = pinchDistance;

            if (Mathf.Approximately(pinchDelta, 0f))
            {
                return true;
            }

            float targetSize = CameraZoomHelper.CalculatePinchTargetOrthographicSize(
                _targetOrthographicSize,
                pinchDelta,
                _config.PinchSensitivity
            );

            QueueZoomTarget(focalScreenPoint, targetSize);

            return true;
        }

        private void QueueScrollZoomTarget()
        {
            float scrollDelta = _scrollWheelAction.ReadValue<Vector2>().y;

            if (Mathf.Approximately(scrollDelta, 0f))
            {
                return;
            }

            Mouse mouse = Mouse.current;

            if (mouse == null)
            {
                return;
            }

            Vector2 focalScreenPoint = mouse.position.ReadValue();

            float targetSize = CameraZoomHelper.CalculateScrollTargetOrthographicSize(
                _targetOrthographicSize,
                scrollDelta,
                _config.ScrollSensitivity
            );

            QueueZoomTarget(focalScreenPoint, targetSize);
        }

        private void QueueZoomTarget(Vector2 focalScreenPoint, float targetOrthographicSize)
        {
            _focalScreenPoint = focalScreenPoint;

            _targetOrthographicSize = CameraZoomHelper.ClampOrthographicSize(
                targetOrthographicSize,
                _cameraOrthoFitProvider.MinOrthographicSize,
                _cameraOrthoFitProvider.FitOrthographicSize
            );
        }

        private void ApplySmoothZoom()
        {
            float currentSize = _cinemachineCamera.Lens.OrthographicSize;

            if (Mathf.Approximately(currentSize, _targetOrthographicSize))
            {
                return;
            }

            float nextSize = CameraZoomHelper.SmoothOrthographicSize(
                currentSize,
                _targetOrthographicSize,
                _config.ZoomSmoothSpeed,
                Time.deltaTime
            );

            if (Mathf.Abs(nextSize - _targetOrthographicSize) < 0.001f)
            {
                nextSize = _targetOrthographicSize;
            }

            ApplyZoomStep(_focalScreenPoint, currentSize, nextSize);
        }

        private void ApplyZoomStep(
            Vector2 focalScreenPoint,
            float fromOrthographicSize,
            float toOrthographicSize)
        {
            if (Mathf.Approximately(fromOrthographicSize, toOrthographicSize))
            {
                return;
            }

            UnityEngine.Camera outputCamera = _brain.OutputCamera;
            Transform cameraTransform = _cinemachineCamera.transform;

            Vector3 worldPointBeforeZoom = CameraZoomHelper.ConvertScreenPointToWorld(
                outputCamera,
                cameraTransform,
                focalScreenPoint,
                fromOrthographicSize
            );

            Vector3 worldPointAfterZoom = CameraZoomHelper.ConvertScreenPointToWorld(
                outputCamera,
                cameraTransform,
                focalScreenPoint,
                toOrthographicSize
            );

            SetOrthographicSize(toOrthographicSize);

            Vector3 positionOffset =
                CameraZoomHelper.CalculateFocalPointPositionOffset(worldPointBeforeZoom, worldPointAfterZoom);

            cameraTransform.position += positionOffset;
            ClampCameraPositionIfNeeded(outputCamera, cameraTransform, toOrthographicSize);
        }

        private void ClampCameraPositionIfNeeded(
            UnityEngine.Camera outputCamera,
            Transform cameraTransform,
            float orthographicSize)
        {
            if (!_confiner.enabled)
            {
                return;
            }

            Collider2D boundingShape = _confiner.BoundingShape2D;

            if (boundingShape == null)
            {
                return;
            }

            cameraTransform.position = CameraZoomHelper.ClampCameraPositionToBounds(
                cameraTransform.position,
                boundingShape.bounds,
                orthographicSize,
                outputCamera.aspect
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
            Guard.AgainstNull(_brain, () => new MissingCameraZoomFieldException(nameof(_brain), gameObject.name));
            Guard.AgainstNull(_confiner, () => new MissingCameraZoomFieldException(nameof(_confiner), gameObject.name));
            Guard.AgainstNull(_config, () => new MissingCameraZoomFieldException(nameof(_config), gameObject.name));
            Guard.AgainstNull(_actionsAsset, () => new MissingCameraZoomFieldException(nameof(_actionsAsset), gameObject.name));

            Guard.AgainstNull(
                _cinemachineCamera,
                () => new MissingCameraZoomFieldException(nameof(_cinemachineCamera), gameObject.name)
            );

            _config.Validate();

            InputActionMap scrollActionMap = _actionsAsset.FindActionMap(CameraZoomInputActions.MapName, throwIfNotFound: false);

            Guard.AgainstNull(
                scrollActionMap,
                () => new MissingCameraZoomInputActionMapException(CameraZoomInputActions.MapName)
            );

            _scrollWheelAction = scrollActionMap.FindAction(CameraZoomInputActions.ScrollWheelActionName, throwIfNotFound: false);

            Guard.AgainstNull(
                _scrollWheelAction,
                () => new MissingCameraZoomInputActionException(
                    CameraZoomInputActions.MapName,
                    CameraZoomInputActions.ScrollWheelActionName
                )
            );

            Guard.AgainstTrue(
                _cinemachineCamera.Lens.ModeOverride != LensSettings.OverrideModes.Orthographic,
                () => new CameraOrthographicRequiredException(gameObject.name)
            );
        }
    }
}
