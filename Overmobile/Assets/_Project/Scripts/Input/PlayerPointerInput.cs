using ExtendedExceptions;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Input
{
    public sealed class PlayerPointerInput
        : MonoBehaviour,
          IPlayerPointerInput,
          IPlayerPointerInputActivation,
          IPlayerPointerIntentGate
    {
        private IPendingPointerDownTrigger _pendingPointerDown;
        private InputActionAsset _actionsAsset;
        private InputAction _click;
        private InputAction _point;
        private PlayerPointerInputConfig _config;
        private PointerIntentStateMachine _intentStateMachine;

        public Vector2 ScreenPosition => PointerIntentHelper.ReadScreenPosition(_point, _intentStateMachine.ActivePressControl);
        public bool IsPressed => _intentStateMachine.IsPressed;
        public event Action Pressed;
        public event Action<PointerReleaseType> Released;

        [Inject]
        public void Construct(InputActionAsset inputActionsAsset, PlayerPointerInputConfig config)
        {
            _actionsAsset = inputActionsAsset;
            _config = config;
        }

        public void Enable()
        {
            _click.Enable();
            _point.Enable();
        }

        public void Disable()
        {
            _intentStateMachine.ResetForDisable();
            _click.Disable();
            _point.Disable();
        }

        void IPlayerPointerIntentGate.RegisterPendingPointerDown(IPendingPointerDownTrigger trigger)
        {
            _pendingPointerDown = trigger;

            if (!_intentStateMachine.IsPressed)
            {
                return;
            }

            FlushPendingPointerDown();
        }

        private void Awake()
        {
            _intentStateMachine = new PointerIntentStateMachine(gameObject.name);
            _intentStateMachine.PendingDownFlushed += OnPendingDownFlushed;
            _intentStateMachine.PendingDownCleared += ClearPendingPointerDown;
            _intentStateMachine.IntentReleased += OnIntentReleased;

            Validate();
            Enable();
        }

        private void OnDestroy()
        {
            _intentStateMachine.PendingDownFlushed -= OnPendingDownFlushed;
            _intentStateMachine.PendingDownCleared -= ClearPendingPointerDown;
            _intentStateMachine.IntentReleased -= OnIntentReleased;
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
            _intentStateMachine.ReleaseActivePressIfActive(PointerReleaseType.PointerUp);
        }

        private void Update()
        {
            if (!_intentStateMachine.IsPressed
             && !_intentStateMachine.IsPending)
            {
                return;
            }

            InputControl activePressControl = _intentStateMachine.ActivePressControl;
            Vector2 currentScreenPosition = PointerIntentHelper.ReadScreenPosition(_point, activePressControl);

            _intentStateMachine.Tick(
                Time.unscaledDeltaTime,
                currentScreenPosition,
                _config,
                TouchInputHelper.IsMultiTouchActive(),
                activePressControl.IsPressed()
            );
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (!context.ReadValueAsButton())
            {
                return;
            }

            Vector2 screenPosition = PointerIntentHelper.ReadScreenPosition(_point, context.control);
            _intentStateMachine.BeginPending(context.control, screenPosition, _config);
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            _intentStateMachine.HandleClickCanceled(context.control);
        }

        private void OnPendingDownFlushed()
        {
            FlushPendingPointerDown();
            Pressed?.Invoke();
        }

        private void OnIntentReleased(PointerReleaseType releaseType)
        {
            Released?.Invoke(releaseType);
        }

        private void FlushPendingPointerDown()
        {
            if (_pendingPointerDown == null)
            {
                return;
            }

            _pendingPointerDown.InvokeConfirmed();
            _pendingPointerDown = null;
        }

        private void ClearPendingPointerDown()
        {
            _pendingPointerDown = null;
        }

        private void Validate()
        {
            Guard.AgainstNull(_actionsAsset, () => new MissingPlayerPointerActionsAssetException(gameObject.name));
            Guard.AgainstNull(_config, () => new MissingPlayerPointerInputConfigException(gameObject.name));
            _config.Validate();

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
