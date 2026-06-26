using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using Input.Binds;
using System;
using System.Collections.Generic;
using System.Threading;
using ViewComponents.Movement;

namespace Core.Input.Movement
{
    public sealed class MovementInputHandler
    {
        private readonly IMovementService _movementService;
        private readonly MovementTargetProvider _movementTargetProvider;
        private readonly List<Bind> _binds = new List<Bind>();

        private CancellationTokenSource _cancellationTokenSource;

        public MovementInputHandler(IMovementService movementService, MovementTargetProvider movementTargetProvider)
        {
            _movementService = movementService;
            _movementTargetProvider = movementTargetProvider;
        }

        public void StartListening()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            IReadOnlyList<MovementTarget> movementTargets = _movementTargetProvider.GetTargets();

            foreach (MovementTarget movementTarget in movementTargets)
            {
                string endpointKey = movementTarget.EndpointKey;
                Bind bind = new Bind(movementTarget.PointArea.PointerUp);

                bind.OnTriggered += () => OnPointerUp(endpointKey);
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

        private void OnPointerUp(string endpointKey)
        {
            if (_movementService.IsMoving)
            {
                return;
            }

            MoveToTargetAsync(endpointKey).Forget();
        }

        private async UniTaskVoid MoveToTargetAsync(string endpointKey)
        {
            try
            {
                await _movementService.MoveToAsync(endpointKey, _cancellationTokenSource.Token);
            }
            catch (MovementInProgressException) { }
            catch (OperationCanceledException) { }
        }
    }
}
