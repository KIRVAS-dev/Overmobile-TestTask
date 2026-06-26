using ExtendedExceptions;

namespace Core.Gameplay.Movement
{
    public sealed class DuplicateMovementRouteException : ExtendedException
    {
        public DuplicateMovementRouteException(string fromEndpointKey, string toEndpointKey)
            : base("movement-1", $"Duplicate movement route for '{fromEndpointKey}' -> '{toEndpointKey}'") { }
    }

    public sealed class RouteNotFoundException : ExtendedException
    {
        public RouteNotFoundException(string fromEndpointKey, string toEndpointKey)
            : base("movement-2", $"Movement route not found for '{fromEndpointKey}' -> '{toEndpointKey}'") { }
    }

    public sealed class InvalidMovementRouteException : ExtendedException
    {
        public InvalidMovementRouteException(string routeName, string reason)
            : base("movement-3", $"Invalid movement route '{routeName}': {reason}") { }
    }

    public sealed class MovementInProgressException : ExtendedException
    {
        public MovementInProgressException()
            : base("movement-4", "Movement is already in progress") { }
    }
}
