using ExtendedExceptions;

namespace ViewComponents.Movement
{
    public sealed class InvalidMovementRouteViewException : ExtendedException
    {
        public InvalidMovementRouteViewException(string routeName, string reason)
            : base("movement-view-1", $"Invalid movement route '{routeName}': {reason}") { }
    }

    public sealed class MovementFacingTransformMissingException : ExtendedException
    {
        public MovementFacingTransformMissingException(string objectName)
            : base("movement-view-2", $"Facing transform is not assigned on '{objectName}'") { }
    }
}
