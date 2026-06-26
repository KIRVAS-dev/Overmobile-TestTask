using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementRoute : MonoBehaviour
    {
        public string FromEndpointKey => GetEndpointKeyAt(0);

        public string ToEndpointKey => GetEndpointKeyAt(_waypoints.Length - 1);

        [SerializeField] private MovementWaypoint[] _waypoints;

        public IReadOnlyList<Vector3> GetWorldPositions()
        {
            ValidateRoute(minimumWaypointCount: 1);

            Vector3[] positions = new Vector3[_waypoints.Length];

            for (int i = 0; i < _waypoints.Length; i++)
            {
                MovementWaypoint waypoint = ValidateWaypointAt(i, requireEndpointKey: false);
                positions[i] = waypoint.transform.position;
            }

            return positions;
        }

        private string GetEndpointKeyAt(int index)
        {
            MovementWaypoint waypoint = ValidateWaypointAt(index, requireEndpointKey: true);

            return waypoint.EndpointKey;
        }

        private void ValidateRoute(int minimumWaypointCount)
        {
            if (_waypoints == null
             || _waypoints.Length < minimumWaypointCount)
            {
                throw new InvalidMovementRouteViewException(
                    gameObject.name,
                    $"Route must have at least {minimumWaypointCount} waypoints"
                );
            }
        }

        private MovementWaypoint ValidateWaypointAt(int index, bool requireEndpointKey)
        {
            int minimumWaypointCount = requireEndpointKey
                ? 2
                : 1;

            ValidateRoute(minimumWaypointCount);

            if (index < 0
             || index >= _waypoints.Length)
            {
                throw new InvalidMovementRouteViewException(gameObject.name, $"Waypoint index {index} is out of range");
            }

            MovementWaypoint waypoint = _waypoints[index];

            if (waypoint == null)
            {
                throw new InvalidMovementRouteViewException(gameObject.name, $"Waypoint at index {index} is missing");
            }

            if (requireEndpointKey && !waypoint.HasEndpointKey)
            {
                throw new InvalidMovementRouteViewException(gameObject.name, $"Waypoint at index {index} has no endpoint key");
            }

            return waypoint;
        }
    }
}
