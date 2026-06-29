using Core;
using Core.Gameplay.Movement;
using Core.Input;
using Cysharp.Threading.Tasks;
using Input;
using Input.Binds;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Input.Movement
{
    public sealed class MovementInputHandler
    {
        private readonly ICoreScopeCancellation _coreScopeCancellation;
        private readonly IGameplayInputBlock _gameplayInputBlock;
        private readonly IPlayerPointerInput _pointerInput;
        private readonly IMovementInputTargetProvider _movementInputTargetProvider;
        private readonly IMovementService _movementService;
        private readonly MovementRouteDisplayService _routeDisplayService;
        private readonly List<Bind> _binds = new List<Bind>();

        private MovementInputTarget? _pendingTarget;
        private MovementInputTarget? _pointerDownTarget;
        private CancellationTokenSource _movementCancellation;
        private bool _isPointerPressed;
        private bool _isPointerOverTarget;

        public MovementInputHandler(
            ICoreScopeCancellation coreScopeCancellation,
            IGameplayInputBlock gameplayInputBlock,
            IPlayerPointerInput pointerInput,
            IMovementInputTargetProvider movementInputTargetProvider,
            IMovementService movementService,
            MovementRouteDisplayService routeDisplayService)
        {
            _coreScopeCancellation = coreScopeCancellation;
            _gameplayInputBlock = gameplayInputBlock;
            _pointerInput = pointerInput;
            _movementInputTargetProvider = movementInputTargetProvider;
            _movementService = movementService;
            _routeDisplayService = routeDisplayService;
        }

        public void StartListening()
        {
            if (_movementCancellation != null)
            {
                throw new MovementInputHandlerAlreadyListeningException();
            }

            _movementCancellation = CancellationTokenSource.CreateLinkedTokenSource(_coreScopeCancellation.Token);

            _pointerInput.Pressed += OnGlobalPointerPressed;
            _pointerInput.Released += OnPointerUp;

            IReadOnlyList<MovementInputTarget> inputTargets = _movementInputTargetProvider.GetInputTargets();

            foreach (MovementInputTarget inputTarget in inputTargets)
            {
                MovementInputTarget capturedTarget = inputTarget;

                AddBind(capturedTarget.PointerDown, () => OnPointerDown(capturedTarget));
                AddBind(capturedTarget.PointerEnter, () => OnPointerEnter(capturedTarget));
                AddBind(capturedTarget.PointerExit, () => OnPointerExit(capturedTarget));
            }
        }

        public void StopListening()
        {
            _pointerInput.Pressed -= OnGlobalPointerPressed;
            _pointerInput.Released -= OnPointerUp;

            foreach (Bind bind in _binds)
            {
                bind.Disable();
            }

            _binds.Clear();

            _movementCancellation?.Cancel();
            _movementCancellation?.Dispose();
            _movementCancellation = null;

            ResetPointerState();
        }

        private void AddBind(ITrigger trigger, Action handler)
        {
            Bind bind = new Bind(trigger);

            bind.OnTriggered += () =>
            {
                if (_gameplayInputBlock.IsBlocked)
                {
                    return;
                }

                handler();
            };

            bind.Enable();
            _binds.Add(bind);
        }

        private void OnGlobalPointerPressed()
        {
            if (_gameplayInputBlock.IsBlocked)
            {
                return;
            }

            _isPointerPressed = true;
        }

        private void OnPointerDown(MovementInputTarget inputTarget)
        {
            _pointerDownTarget = inputTarget;
            _isPointerOverTarget = true;
            _pendingTarget = inputTarget;
            _routeDisplayService.PreviewRouteTo(inputTarget.EndpointKey);
        }

        private void OnPointerEnter(MovementInputTarget inputTarget)
        {
            if (!_isPointerPressed)
            {
                return;
            }

            _isPointerOverTarget = true;
            _pendingTarget = inputTarget;
            _routeDisplayService.PreviewRouteTo(inputTarget.EndpointKey);
        }

        private void OnPointerExit(MovementInputTarget inputTarget)
        {
            if (_isPointerPressed)
            {
                _isPointerOverTarget = false;
                _routeDisplayService.ClearPreview();
                return;
            }

            if (_pendingTarget.HasValue
             && _pendingTarget.Value.EndpointKey == inputTarget.EndpointKey)
            {
                ClearPendingPreview();
            }
        }

        private void OnPointerUp()
        {
            _isPointerPressed = false;

            if (_gameplayInputBlock.IsBlocked)
            {
                ClearPendingPreview();
                _pointerDownTarget = null;
                return;
            }

            bool canMove = _pendingTarget.HasValue
             && (_isPointerOverTarget
                 || _pointerDownTarget.HasValue && _pointerDownTarget.Value.EndpointKey == _pendingTarget.Value.EndpointKey);

            if (!canMove)
            {
                ClearPendingPreview();
                _pointerDownTarget = null;
                return;
            }

            MovementInputTarget pendingTarget = _pendingTarget.Value;
            _pendingTarget = null;
            _pointerDownTarget = null;

            MoveToTargetAsync(pendingTarget).Forget();
        }

        private void ClearPendingPreview()
        {
            _pendingTarget = null;
            _routeDisplayService.ClearPreview();
        }

        private void ResetPointerState()
        {
            _isPointerPressed = false;
            _isPointerOverTarget = false;
            _pendingTarget = null;
            _pointerDownTarget = null;
        }

        private async UniTaskVoid MoveToTargetAsync(MovementInputTarget inputTarget)
        {
            try
            {
                await _movementService.MoveToAsync(
                    inputTarget.EndpointKey,
                    inputTarget.FacingWorldPosition,
                    _movementCancellation.Token
                );
            }
            catch (MovementInProgressException) { }
            catch (OperationCanceledException) { }
        }
    }
}
