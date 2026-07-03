using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public sealed class PlayerPointerInput
        : MonoBehaviour,
          IPlayerPointerInput,
          IPlayerPointerInputActivation
    {
        [SerializeField] private InputActionAsset _actionsAsset;

        private InputAction _click;
        private InputAction _point;
        private InputControl _activePressControl;

        public Vector2 ScreenPosition => ReadActiveScreenPosition();
        public bool IsPressed => _activePressControl != null;

        public event Action Pressed;
        public event Action Released;

        public void Enable()
        {
            _click.Enable();
            _point.Enable();
        }

        public void Disable()
        {
            ReleaseActivePress();
            _click.Disable();
            _point.Disable();
        }

        private void Awake()
        {
            Validate();
            Enable();
        }

        private void OnEnable()
        {
            _click.performed += OnClickPerformed;
            _click.canceled += OnClickCanceled;
        }

        private void OnDisable()
        {
            _click.performed -= OnClickPerformed;
            _click.canceled -= OnClickCanceled;
            _activePressControl = null;
        }

        private void Update()
        {
            if (_activePressControl != null
             && !_activePressControl.IsPressed())
            {
                ReleaseActivePress();
            }
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (!context.ReadValueAsButton()
             || _activePressControl != null)
            {
                return;
            }

            _activePressControl = context.control;
            Pressed?.Invoke();
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            if (_activePressControl == null)
            {
                return;
            }

            if (context.control != null
             && context.control != _activePressControl)
            {
                return;
            }

            ReleaseActivePress();
        }

        private void ReleaseActivePress()
        {
            if (_activePressControl == null)
            {
                return;
            }

            _activePressControl = null;
            Released?.Invoke();
        }

        private Vector2 ReadActiveScreenPosition()
        {
            return _activePressControl == null
                ? throw new ActivePlayerPointerNotAssignedException()
                : _point.ReadValue<Vector2>();
        }

        private void Validate()
        {
            if (_actionsAsset == null)
            {
                throw new MissingPlayerPointerActionsAssetException(gameObject.name);
            }

            InputActionMap actionMap = _actionsAsset.FindActionMap(PointerInputActions.MapName, throwIfNotFound: false);

            if (actionMap == null)
            {
                throw new MissingPlayerPointerActionMapException(PointerInputActions.MapName);
            }

            _click = actionMap.FindAction(PointerInputActions.Click, throwIfNotFound: false);

            if (_click == null)
            {
                throw new MissingPlayerPointerActionException(PointerInputActions.MapName, PointerInputActions.Click);
            }

            _point = actionMap.FindAction(PointerInputActions.Point, throwIfNotFound: false);

            if (_point == null)
            {
                throw new MissingPlayerPointerActionException(PointerInputActions.MapName, PointerInputActions.Point);
            }
        }
    }
}
