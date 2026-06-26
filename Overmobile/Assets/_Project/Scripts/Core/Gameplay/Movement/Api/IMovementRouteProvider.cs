using System.Collections.Generic;

namespace Core.Gameplay.Movement
{
    public interface IMovementRouteProvider
    {
        string SpawnLocationKey { get; }

        IReadOnlyList<MovementRouteData> GetRoutes();
    }
}
