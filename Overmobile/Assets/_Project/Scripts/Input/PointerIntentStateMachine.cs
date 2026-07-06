using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public sealed class PointerIntentStateMachine
    {
        private enum State
        {
            Idle,
            Pending,
            Active
        }

        private readonly string _ownerName;
        private State _state;
        private InputControl _activePressControl;
        private Vector2 _pendingScreenPosition;
        private float _pendingElapsed;

        public PointerIntentStateMachine(string ownerName)
        {
            _ownerName = ownerName;
        }

        public bool IsPressed => _state == State.Active;
        public bool IsPending => _state == State.Pending;
        public InputControl ActivePressControl => _activePressControl;

        public event Action PendingDownFlushed;
        public event Action PendingDownCleared;
        public event Action<PointerReleaseType> IntentReleased;

        public void ResetForDisable()
        {
            switch (_state)
            {
                case State.Active:
                    ReleaseActivePress(PointerReleaseType.PointerUp);
                    break;

                case State.Pending:
                    CancelPending();
                    break;

                case State.Idle:
                    break;

                default:
                    throw new InvalidPointerIntentStateException(_ownerName, (int)_state);
            }
        }

        public void BeginPending(
            InputControl pressControl,
            Vector2 screenPosition,
            PlayerPointerInputConfig config)
        {
            if (_state != State.Idle)
            {
                return;
            }

            _activePressControl = pressControl;
            _pendingScreenPosition = screenPosition;
            _pendingElapsed = 0f;
            _state = State.Pending;

            if (PointerIntentHelper.ShouldUseInstantConfirm(config, pressControl))
            {
                ConfirmIntent();
            }
        }

        public void Tick(
            float unscaledDeltaTime,
            Vector2 currentScreenPosition,
            PlayerPointerInputConfig config,
            bool isMultiTouchActive,
            bool isPressStillHeld)
        {
            if (_state == State.Pending)
            {
                TickPending(
                    unscaledDeltaTime,
                    currentScreenPosition,
                    config,
                    isMultiTouchActive,
                    isPressStillHeld
                );

                return;
            }

            if (_state != State.Active)
            {
                return;
            }

            if (isMultiTouchActive)
            {
                ReleaseActivePress(PointerReleaseType.MultiTouch);

                return;
            }

            if (!isPressStillHeld)
            {
                ReleaseActivePress(PointerReleaseType.PointerUp);
            }
        }

        public void HandleClickCanceled(InputControl canceledControl)
        {
            if (_state == State.Idle)
            {
                return;
            }

            if (canceledControl != null
             && canceledControl != _activePressControl)
            {
                return;
            }

            if (_state == State.Pending)
            {
                CancelPending();

                return;
            }

            ReleaseActivePress(PointerReleaseType.PointerUp);
        }

        public void ReleaseActivePressIfActive(PointerReleaseType releaseType)
        {
            if (_state == State.Active)
            {
                ReleaseActivePress(releaseType);
            }
            else
            {
                CancelPending();
            }
        }

        private void TickPending(
            float unscaledDeltaTime,
            Vector2 currentScreenPosition,
            PlayerPointerInputConfig config,
            bool isMultiTouchActive,
            bool isPressStillHeld)
        {
            if (isMultiTouchActive
             || !isPressStillHeld
             || PointerIntentHelper.IsSlopExceeded(_pendingScreenPosition, currentScreenPosition, config.TouchSlopPixels))
            {
                CancelPending();

                return;
            }

            _pendingElapsed += unscaledDeltaTime;

            if (_pendingElapsed < config.ConfirmDelaySeconds)
            {
                return;
            }

            ConfirmIntent();
        }

        private void ConfirmIntent()
        {
            if (_state != State.Pending
             || _activePressControl == null)
            {
                return;
            }

            _state = State.Active;
            PendingDownFlushed?.Invoke();
        }

        private void CancelPending()
        {
            if (_state != State.Pending)
            {
                return;
            }

            PendingDownCleared?.Invoke();
            _activePressControl = null;
            _state = State.Idle;
        }

        private void ReleaseActivePress(PointerReleaseType releaseType)
        {
            if (_state != State.Active)
            {
                return;
            }

            _activePressControl = null;
            _state = State.Idle;
            IntentReleased?.Invoke(releaseType);
        }
    }
}
