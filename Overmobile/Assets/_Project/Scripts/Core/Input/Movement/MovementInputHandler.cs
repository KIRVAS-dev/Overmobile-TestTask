using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using Input.Binds;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Input.Movement
{
    public sealed class MovementInputHandler
    {
        private readonly IMovementService _movementService;
        private readonly IMovementInputTargetProvider _movementInputTargetProvider;
        private readonly List<Bind> _binds = new List<Bind>();

        private CancellationTokenSource _cancellationTokenSource;

        public MovementInputHandler(
            IMovementService movementService,
            IMovementInputTargetProvider movementInputTargetProvider)
        {
            _movementService = movementService;
            _movementInputTargetProvider = movementInputTargetProvider;
        }

        public void StartListening()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            IReadOnlyList<MovementInputTarget> inputTargets = _movementInputTargetProvider.GetInputTargets();

            foreach (MovementInputTarget inputTarget in inputTargets)
            {
                MovementInputTarget capturedTarget = inputTarget;
                Bind bind = new Bind(capturedTarget.PointerUp);

                bind.OnTriggered += () => OnPointerUp(capturedTarget);
                bind.Enable();
                _binds.Add(bind);
            }
        }

        public void StopListening()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            foreach (Bind bind in _binds)
            {
                bind.Disable();
            }

            _binds.Clear();
        }

        private void OnPointerUp(MovementInputTarget inputTarget)
        {
            if (_movementService.IsMoving)
            {
                return;
            }

            MoveToTargetAsync(inputTarget).Forget();
        }

        private async UniTaskVoid MoveToTargetAsync(MovementInputTarget inputTarget)
        {
            try
            {
                await _movementService.MoveToAsync(
                    inputTarget.EndpointKey,
                    inputTarget.FacingWorldPosition,
                    _cancellationTokenSource.Token
                );
            }
            catch (MovementInProgressException) { }
            catch (OperationCanceledException) { }
        }
    }
}
