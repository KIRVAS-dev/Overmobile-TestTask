using System.Collections.Generic;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public interface IMovementRouteRegistry
    {
        string SpawnLocationKey { get; }

        IReadOnlyList<Vector3> ResolvePath(string fromEndpointKey, string toEndpointKey);
    }
}
