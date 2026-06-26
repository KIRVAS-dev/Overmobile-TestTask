using ExtendedExceptions;

namespace ViewComponents.Movement
{
    public sealed class InvalidMovementRouteViewException : ExtendedException
    {
        public InvalidMovementRouteViewException(string routeName, string reason)
            : base("movement-view-1", $"Invalid movement route '{routeName}': {reason}") { }
    }

    public sealed class MovementTargetEndpointKeyMissingException : ExtendedException
    {
        public MovementTargetEndpointKeyMissingException(string objectName)
            : base("movement-view-2", $"Endpoint key is empty on '{objectName}'") { }
    }

    public sealed class InvalidMovementTargetViewException : ExtendedException
    {
        public InvalidMovementTargetViewException(string providerName, string reason)
            : base("movement-view-4", $"Invalid movement target provider '{providerName}': {reason}") { }
    }
}
