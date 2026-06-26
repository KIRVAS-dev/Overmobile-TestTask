using System.Collections.Generic;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public sealed class MovementRouteRegistry : IMovementRouteRegistry
    {
        private readonly IMovementRouteProvider _routeProvider;

        private Dictionary<(string from, string to), IReadOnlyList<Vector3>> _routes;

        public MovementRouteRegistry(IMovementRouteProvider routeProvider)
        {
            _routeProvider = routeProvider;
        }

        public string SpawnLocationKey => _routeProvider.SpawnLocationKey;

        public IReadOnlyList<Vector3> ResolvePath(string fromEndpointKey, string toEndpointKey)
        {
            EnsureRoutesBuilt();

            return _routes.TryGetValue((fromEndpointKey, toEndpointKey), out IReadOnlyList<Vector3> worldPoints)
                ? worldPoints
                : throw new RouteNotFoundException(fromEndpointKey, toEndpointKey);
        }

        private void EnsureRoutesBuilt()
        {
            if (_routes != null)
            {
                return;
            }

            IReadOnlyList<MovementRouteData> routeData = _routeProvider.GetRoutes();

            if (routeData == null
             || routeData.Count == 0)
            {
                throw new InvalidMovementRouteException(nameof(MovementRouteRegistry), "Movement routes are not provided");
            }

            _routes = new Dictionary<(string from, string to), IReadOnlyList<Vector3>>();

            foreach (MovementRouteData route in routeData)
            {
                (string from, string to) key = (route.FromEndpointKey, route.ToEndpointKey);

                if (!_routes.TryAdd(key, route.WorldPoints))
                {
                    throw new DuplicateMovementRouteException(route.FromEndpointKey, route.ToEndpointKey);
                }
            }
        }
    }
}
