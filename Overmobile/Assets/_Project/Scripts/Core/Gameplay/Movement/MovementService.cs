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
        private readonly IActiveMovementViewProvider _activeMovementViewProvider;

        public MovementService(
            MovementModel movementModel,
            MovementConfig movementConfig,
            IMovementRouteRegistry routeRegistry,
            IActiveMovementViewProvider activeMovementViewProvider)
        {
            _movementModel = movementModel;
            _movementConfig = movementConfig;
            _routeRegistry = routeRegistry;
            _activeMovementViewProvider = activeMovementViewProvider;

            _movementModel.SetCurrentEndpointKey(_routeRegistry.SpawnLocationKey);
        }

        public bool IsMoving => _movementModel.IsMoving;

        public async UniTask MoveToAsync(string toEndpointKey, Vector3 destinationFacingWorldPosition,
            CancellationToken cancellationToken)
        {
            if (_movementModel.IsMoving)
            {
                throw new MovementInProgressException();
            }

            string fromEndpointKey = _movementModel.CurrentEndpointKey;

            if (_movementConfig.FacingRotationDuration <= 0f)
            {
                throw new InvalidMovementRouteException(
                    nameof(MovementService),
                    "Facing rotation duration must be greater than zero"
                );
            }

            if (fromEndpointKey == toEndpointKey)
            {
                _movementModel.SetMoving(true);

                try
                {
                    await _activeMovementViewProvider.ActiveMovementView.FaceTowardAsync(
                        destinationFacingWorldPosition,
                        _movementConfig.FacingRotationDuration,
                        cancellationToken
                    );
                }
                finally
                {
                    _movementModel.SetMoving(false);
                }

                return;
            }

            IReadOnlyList<Vector3> pathPoints = _routeRegistry.ResolvePath(fromEndpointKey, toEndpointKey);

            if (pathPoints == null
             || pathPoints.Count == 0)
            {
                throw new InvalidMovementRouteException($"{fromEndpointKey}->{toEndpointKey}", "Path has no waypoints");
            }

            IReadOnlyList<Vector3> movementPath = GetMovementPath(pathPoints);

            if (_movementConfig.MoveSpeed <= 0f)
            {
                throw new InvalidMovementRouteException(nameof(MovementService), "Move speed must be greater than zero");
            }

            _movementModel.SetMoving(true);

            try
            {
                await _activeMovementViewProvider.ActiveMovementView.MoveAlongPathAsync(
                    movementPath,
                    _movementConfig.MoveSpeed,
                    _movementConfig.FacingRotationDuration,
                    destinationFacingWorldPosition,
                    cancellationToken
                );

                _movementModel.SetCurrentEndpointKey(toEndpointKey);
            }
            finally
            {
                _movementModel.SetMoving(false);
            }
        }

        private IReadOnlyList<Vector3> GetMovementPath(IReadOnlyList<Vector3> pathPoints)
        {
            if (pathPoints.Count == 1)
            {
                return pathPoints;
            }

            Vector3[] trimmedPath = new Vector3[pathPoints.Count - 1];

            for (int i = 1; i < pathPoints.Count; i++)
            {
                trimmedPath[i - 1] = pathPoints[i];
            }

            return trimmedPath;
        }
    }
}
