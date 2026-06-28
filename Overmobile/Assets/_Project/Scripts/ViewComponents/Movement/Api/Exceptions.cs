using ExtendedExceptions;

namespace ViewComponents.Movement
{
    public sealed class InvalidMovementRouteViewException : ExtendedException
    {
        public InvalidMovementRouteViewException(string routeName, string reason)
            : base("movement-view-1", $"Invalid movement route '{routeName}': {reason}") { }
    }
}
