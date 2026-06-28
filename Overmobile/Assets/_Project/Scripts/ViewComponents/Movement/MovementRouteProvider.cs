using Core.Gameplay.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementRouteProvider
        : MonoBehaviour,
          IMovementRouteProvider
    {
        public string SpawnLocationKey => _spawnLocationKey;

        [SerializeField] private string _spawnLocationKey = "Start";
        [SerializeField] private MovementRoute[] _movementRoutes;

        public IReadOnlyList<MovementRouteData> GetRoutes()
        {
            if (_movementRoutes == null
             || _movementRoutes.Length == 0)
            {
                throw new InvalidMovementRouteViewException(gameObject.name, "Movement routes are not assigned");
            }

            MovementRouteData[] routes = new MovementRouteData[_movementRoutes.Length];

            for (int i = 0; i < _movementRoutes.Length; i++)
            {
                MovementRoute route = _movementRoutes[i];

                if (route == null)
                {
                    throw new InvalidMovementRouteViewException(gameObject.name, $"Movement route at index {i} is missing");
                }

                routes[i] = new MovementRouteData(route.FromEndpointKey, route.ToEndpointKey, route.GetWorldPositions());
            }

            return routes;
        }

        public bool TryFindRoute(string fromEndpointKey, string toEndpointKey, out MovementRoute route)
        {
            if (_movementRoutes == null)
            {
                route = null;
                return false;
            }

            foreach (MovementRoute movementRoute in _movementRoutes)
            {
                if (movementRoute == null
                 || movementRoute.FromEndpointKey != fromEndpointKey
                 || movementRoute.ToEndpointKey != toEndpointKey)
                {
                    continue;
                }

                route = movementRoute;
                return true;
            }

            route = null;
            return false;
        }
    }
}
