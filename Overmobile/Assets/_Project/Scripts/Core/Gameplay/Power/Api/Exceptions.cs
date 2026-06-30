using ExtendedExceptions;

namespace Core.Gameplay.Power
{
    public sealed class DuplicateEntityIdException : ExtendedException
    {
        public DuplicateEntityIdException(string entityId)
            : base("power-1", $"Duplicate entity id '{entityId}'") { }
    }

    public sealed class EntityPowerNotFoundException : ExtendedException
    {
        public EntityPowerNotFoundException(string entityId)
            : base("power-2", $"Entity not found for id '{entityId}'") { }
    }

    public sealed class PlayerEntityNotFoundException : ExtendedException
    {
        public PlayerEntityNotFoundException()
            : base("power-3", "Player entity is not registered") { }
    }

    public sealed class InvalidEntityPowerProviderException : ExtendedException
    {
        public InvalidEntityPowerProviderException(string providerName, string reason)
            : base("power-4", $"Invalid entity power provider '{providerName}': {reason}") { }
    }

    public sealed class MultiplePlayerEntitiesException : ExtendedException
    {
        public MultiplePlayerEntitiesException()
            : base("power-5", "Multiple player entities are registered") { }
    }
}
