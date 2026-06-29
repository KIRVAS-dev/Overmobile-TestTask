using Core.Gameplay.Movement;
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
        private readonly IPlayerPointerInput _pointerInput;
        private readonly IMovementService _movementService;
        private readonly IMovementInputTargetProvider _movementInputTargetProvider;
        private readonly MovementRouteDisplayService _routeDisplayService;
        private readonly List<Bind> _binds = new List<Bind>();

        private bool _isPointerPressed;
        private bool _isPointerOverTarget;
        private MovementInputTarget? _pendingTarget;
        private MovementInputTarget? _pointerDownTarget;
        private CancellationTokenSource _movementCancellation;

        public MovementInputHandler(
            IPlayerPointerInput pointerInput,
            IMovementService movementService,
            IMovementInputTargetProvider movementInputTargetProvider,
            MovementRouteDisplayService routeDisplayService)
        {
            _pointerInput = pointerInput;
            _movementService = movementService;
            _movementInputTargetProvider = movementInputTargetProvider;
            _routeDisplayService = routeDisplayService;
        }

        public void StartListening()
        {
            _movementCancellation = new CancellationTokenSource();

            _pointerInput.Pressed += OnGlobalPointerPressed;
            _pointerInput.Released += OnPointerUp;

            IReadOnlyList<MovementInputTarget> inputTargets = _movementInputTargetProvider.GetInputTargets();

            foreach (MovementInputTarget inputTarget in inputTargets)
            {
                MovementInputTarget capturedTarget = inputTarget;

                Bind pointerDownBind = new Bind(capturedTarget.PointerDown);
                pointerDownBind.OnTriggered += () => OnPointerDown(capturedTarget);
                pointerDownBind.Enable();
                _binds.Add(pointerDownBind);

                Bind pointerEnterBind = new Bind(capturedTarget.PointerEnter);
                pointerEnterBind.OnTriggered += () => OnPointerEnter(capturedTarget);
                pointerEnterBind.Enable();
                _binds.Add(pointerEnterBind);

                Bind pointerExitBind = new Bind(capturedTarget.PointerExit);
                pointerExitBind.OnTriggered += () => OnPointerExit(capturedTarget);
                pointerExitBind.Enable();
                _binds.Add(pointerExitBind);
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
            ResetPointerState();

            _movementCancellation?.Cancel();
            _movementCancellation?.Dispose();
            _movementCancellation = null;
        }

        private void OnGlobalPointerPressed()
        {
            _isPointerPressed = true;
        }

        private void OnPointerDown(MovementInputTarget inputTarget)
        {
            if (_movementService.IsMoving)
            {
                return;
            }

            _pointerDownTarget = inputTarget;
            _isPointerOverTarget = true;
            _pendingTarget = inputTarget;
            _routeDisplayService.PreviewRouteTo(inputTarget.EndpointKey);
        }

        private void OnPointerEnter(MovementInputTarget inputTarget)
        {
            if (!_isPointerPressed
             || _movementService.IsMoving)
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

            if (_movementService.IsMoving)
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
