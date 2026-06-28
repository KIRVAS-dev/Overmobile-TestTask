using Core.Gameplay.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementRouteWaypointDisplay
        : MonoBehaviour,
          IMovementRouteWaypointDisplay
    {
        [SerializeField] private MovementRouteProvider _movementRouteProvider;

        private MovementRoute _activeRoute;

        public void ShowRoute(string fromEndpointKey, string toEndpointKey)
        {
            if (fromEndpointKey == toEndpointKey
             || !_movementRouteProvider.TryFindRoute(fromEndpointKey, toEndpointKey, out MovementRoute route))
            {
                HideRoute();
                return;
            }

            if (_activeRoute != route)
            {
                HideRoute();
            }

            _activeRoute = route;
            _activeRoute.gameObject.SetActive(true);

            IReadOnlyList<MovementWaypoint> waypoints = _activeRoute.Waypoints;

            foreach (MovementWaypoint waypoint in waypoints)
            {
                waypoint.EnableVisual();
            }
        }

        public void HideRoute()
        {
            if (_activeRoute == null)
            {
                return;
            }

            IReadOnlyList<MovementWaypoint> waypoints = _activeRoute.Waypoints;

            foreach (MovementWaypoint waypoint in waypoints)
            {
                waypoint.DisableVisual();
            }

            _activeRoute.gameObject.SetActive(false);
            _activeRoute = null;
        }

        public void DisableWaypointAtRouteIndex(int routeWaypointIndex)
        {
            if (_activeRoute == null)
            {
                return;
            }

            IReadOnlyList<MovementWaypoint> waypoints = _activeRoute.Waypoints;

            if (routeWaypointIndex < 0
             || routeWaypointIndex >= waypoints.Count)
            {
                throw new InvalidMovementRouteViewException(
                    _activeRoute.gameObject.name,
                    $"Waypoint index {routeWaypointIndex} is out of range"
                );
            }

            waypoints[routeWaypointIndex].DisableVisual();
        }
    }
}
