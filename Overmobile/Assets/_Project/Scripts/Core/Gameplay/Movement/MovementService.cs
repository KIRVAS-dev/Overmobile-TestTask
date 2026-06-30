using Core.Gameplay.Character;
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
        private readonly IActiveCharacterViewProvider _activeCharacterViewProvider;
        private readonly MovementRouteDisplayService _routeDisplayService;
        private readonly IGameplayInputBlock _gameplayInputBlock;

        public MovementService(
            MovementModel movementModel,
            MovementConfig movementConfig,
            IMovementRouteRegistry routeRegistry,
            IActiveCharacterViewProvider activeCharacterViewProvider,
            MovementRouteDisplayService routeDisplayService,
            IGameplayInputBlock gameplayInputBlock)
        {
            _movementModel = movementModel;
            _movementConfig = movementConfig;
            _routeRegistry = routeRegistry;
            _activeCharacterViewProvider = activeCharacterViewProvider;
            _routeDisplayService = routeDisplayService;
            _gameplayInputBlock = gameplayInputBlock;

            _movementModel.SetCurrentEndpointKey(_routeRegistry.SpawnLocationKey);
        }

        public bool IsMoving => _movementModel.IsMoving;

        public async UniTask MoveToAsync(
            string toEndpointKey,
            Vector3 destinationFacingWorldPosition,
            CancellationToken cancellationToken)
        {
            if (_movementModel.IsMoving)
            {
                throw new MovementInProgressException();
            }

            _gameplayInputBlock.Block();

            try
            {
                await MoveToInternalAsync(toEndpointKey, destinationFacingWorldPosition, cancellationToken);
            }
            finally
            {
                _gameplayInputBlock.Unblock();
            }
        }

        private async UniTask MoveToInternalAsync(
            string toEndpointKey,
            Vector3 destinationFacingWorldPosition,
            CancellationToken cancellationToken)
        {
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
                    await _activeCharacterViewProvider.ActiveCharacterView.MovementView.FaceTowardAsync(
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

            if (_movementConfig.MoveSpeed <= 0f)
            {
                throw new InvalidMovementRouteException(nameof(MovementService), "Move speed must be greater than zero");
            }

            _movementModel.SetMoving(true);

            try
            {
                _routeDisplayService.OnMovementStarted();

                await _activeCharacterViewProvider.ActiveCharacterView.MovementView.MoveAlongPathAsync(
                    pathPoints,
                    _movementConfig.MoveSpeed,
                    _movementConfig.FacingRotationDuration,
                    destinationFacingWorldPosition,
                    _routeDisplayService.OnMovementWaypointReached,
                    cancellationToken
                );

                _movementModel.SetCurrentEndpointKey(toEndpointKey);
            }
            finally
            {
                _movementModel.SetMoving(false);
                _routeDisplayService.OnMovementEnded();
            }
        }
    }
}
