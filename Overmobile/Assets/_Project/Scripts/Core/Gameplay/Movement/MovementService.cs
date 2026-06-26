using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public sealed class MovementService : IMovementService
    {
        private readonly MovementModel _movementModel;
        private readonly MovementConfig _movementConfig;
        private readonly IMovementRouteRegistry _routeRegistry;
        private readonly IMovementView _movementView;

        public MovementService(
            MovementModel movementModel,
            MovementConfig movementConfig,
            IMovementRouteRegistry routeRegistry,
            IMovementView movementView)
        {
            _movementModel = movementModel;
            _movementConfig = movementConfig;
            _routeRegistry = routeRegistry;
            _movementView = movementView;

            _movementModel.SetCurrentEndpointKey(_routeRegistry.SpawnLocationKey);
        }

        public bool IsMoving => _movementModel.IsMoving;

        public string CurrentEndpointKey => _movementModel.CurrentEndpointKey;

        public async UniTask MoveToAsync(string toEndpointKey, CancellationToken cancellationToken)
        {
            if (_movementModel.IsMoving)
            {
                throw new MovementInProgressException();
            }

            string fromEndpointKey = _movementModel.CurrentEndpointKey;

            if (fromEndpointKey == toEndpointKey)
            {
                return;
            }

            IReadOnlyList<Vector3> pathPoints = _routeRegistry.ResolvePath(fromEndpointKey, toEndpointKey);

            if (pathPoints == null
             || pathPoints.Count == 0)
            {
                throw new InvalidMovementRouteException($"{fromEndpointKey}->{toEndpointKey}", "Path has no waypoints");
            }

            if (_movementConfig.MoveSpeed <= 0f)
            {
                throw new InvalidMovementRouteException(nameof(MovementService), "Move speed must be greater than zero");
            }

            _movementModel.SetMoving(true);

            try
            {
                await _movementView.MoveAlongPathAsync(pathPoints, _movementConfig.MoveSpeed, cancellationToken);

                _movementModel.SetCurrentEndpointKey(toEndpointKey);
            }
            finally
            {
                _movementModel.SetMoving(false);
            }
        }
    }
}
