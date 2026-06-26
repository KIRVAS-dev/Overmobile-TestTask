using System.Collections.Generic;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public readonly struct MovementRouteData
    {
        public string FromEndpointKey { get; }

        public string ToEndpointKey { get; }

        public IReadOnlyList<Vector3> WorldPoints { get; }

        public MovementRouteData(string fromEndpointKey, string toEndpointKey, IReadOnlyList<Vector3> worldPoints)
        {
            FromEndpointKey = fromEndpointKey;
            ToEndpointKey = toEndpointKey;
            WorldPoints = worldPoints;
        }
    }
}
