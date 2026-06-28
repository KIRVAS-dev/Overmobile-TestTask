using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Movement
{
    public static class MovementHelper
    {
        private const int ImplicitOriginWaypointCount = 1;

        public static Vector3[] BuildMovementPath(IReadOnlyList<Vector3> routePoints)
        {
            if (routePoints.Count <= ImplicitOriginWaypointCount)
            {
                Vector3[] fullPath = new Vector3[routePoints.Count];

                for (int i = 0; i < routePoints.Count; i++)
                {
                    fullPath[i] = routePoints[i];
                }

                return fullPath;
            }

            Vector3[] movementPath = new Vector3[routePoints.Count - ImplicitOriginWaypointCount];

            for (int i = ImplicitOriginWaypointCount; i < routePoints.Count; i++)
            {
                movementPath[i - ImplicitOriginWaypointCount] = routePoints[i];
            }

            return movementPath;
        }

        public static bool TryMapMovementPathIndexToRouteIndex(int reachedMovementPathIndex, int movementPathLength,
            out int routeWaypointIndex)
        {
            routeWaypointIndex = reachedMovementPathIndex + ImplicitOriginWaypointCount;

            return reachedMovementPathIndex >= 0 && reachedMovementPathIndex < movementPathLength;
        }

        public static float CalculatePolylineLength(Vector3 pathOrigin, IReadOnlyList<Vector3> pathPoints)
        {
            float length = 0f;
            Vector3 previous = pathOrigin;

            foreach (Vector3 pathPoint in pathPoints)
            {
                length += Vector3.Distance(previous, pathPoint);
                previous = pathPoint;
            }

            return length;
        }

        public static Vector3 CalculatePlanarTravelDirection(Vector3 worldTarget, Vector3 origin)
        {
            Vector3 travelDirection = worldTarget - origin;
            travelDirection.z = 0f;

            return travelDirection;
        }

        public static float CalculateLocalFacingY(Vector3 worldTarget, Vector3 facingWorldPosition,
            Quaternion? parentWorldRotation, float yawOffset)
        {
            Vector3 worldDirection = CalculatePlanarTravelDirection(worldTarget, facingWorldPosition);

            if (parentWorldRotation == null)
            {
                return Mathf.Atan2(worldDirection.x, worldDirection.y) * Mathf.Rad2Deg + yawOffset;
            }

            Vector3 localDirection = Quaternion.Inverse(parentWorldRotation.Value) * worldDirection.normalized;
            localDirection.z = 0f;
            float localY = Mathf.Atan2(localDirection.x, localDirection.y) * Mathf.Rad2Deg;

            return localY + yawOffset;
        }
    }
}
