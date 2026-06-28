using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using Input.Binds;
using System;
using System.Collections.Generic;

namespace Core.Input.Movement
{
    public sealed class MovementInputHandler
    {
        private readonly IMovementService _movementService;
        private readonly IMovementInputTargetProvider _movementInputTargetProvider;
        private readonly MovementRouteDisplayService _routeDisplayService;
        private readonly List<Bind> _binds = new List<Bind>();

        private string _pendingEndpointKey;

        public MovementInputHandler(
            IMovementService movementService,
            IMovementInputTargetProvider movementInputTargetProvider,
            MovementRouteDisplayService routeDisplayService)
        {
            _movementService = movementService;
            _movementInputTargetProvider = movementInputTargetProvider;
            _routeDisplayService = routeDisplayService;
        }

        public void StartListening()
        {
            IReadOnlyList<MovementInputTarget> inputTargets = _movementInputTargetProvider.GetInputTargets();

            foreach (MovementInputTarget inputTarget in inputTargets)
            {
                MovementInputTarget capturedTarget = inputTarget;

                Bind pointerDownBind = new Bind(capturedTarget.PointerDown);
                pointerDownBind.OnTriggered += () => OnPointerDown(capturedTarget);
                pointerDownBind.Enable();
                _binds.Add(pointerDownBind);

                Bind pointerExitBind = new Bind(capturedTarget.PointerExit);
                pointerExitBind.OnTriggered += () => OnPointerExit(capturedTarget);
                pointerExitBind.Enable();
                _binds.Add(pointerExitBind);

                Bind pointerUpBind = new Bind(capturedTarget.PointerUp);
                pointerUpBind.OnTriggered += () => OnPointerUp(capturedTarget);
                pointerUpBind.Enable();
                _binds.Add(pointerUpBind);
            }
        }

        public void StopListening()
        {
            foreach (Bind bind in _binds)
            {
                bind.Disable();
            }

            _binds.Clear();
            _pendingEndpointKey = null;
        }

        private void OnPointerDown(MovementInputTarget inputTarget)
        {
            if (_movementService.IsMoving)
            {
                return;
            }

            _pendingEndpointKey = inputTarget.EndpointKey;
            _routeDisplayService.PreviewRouteTo(inputTarget.EndpointKey);
        }

        private void OnPointerExit(MovementInputTarget inputTarget)
        {
            if (_pendingEndpointKey == inputTarget.EndpointKey)
            {
                _pendingEndpointKey = null;
                _routeDisplayService.ClearPreview();
            }
        }

        private void OnPointerUp(MovementInputTarget inputTarget)
        {
            if (_movementService.IsMoving
             || _pendingEndpointKey != inputTarget.EndpointKey)
            {
                return;
            }

            _pendingEndpointKey = null;
            MoveToTargetAsync(inputTarget).Forget();
        }

        private async UniTaskVoid MoveToTargetAsync(MovementInputTarget inputTarget)
        {
            try
            {
                await _movementService.MoveToAsync(inputTarget.EndpointKey, inputTarget.FacingWorldPosition);
            }
            catch (MovementInProgressException) { }
            catch (OperationCanceledException) { }
        }
    }
}
