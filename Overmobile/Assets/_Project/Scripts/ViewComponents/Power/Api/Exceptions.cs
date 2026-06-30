using ExtendedExceptions;

namespace ViewComponents.Power
{
    public sealed class MissingEntityPowerEntityIdReferenceException : ExtendedException
    {
        public MissingEntityPowerEntityIdReferenceException(string objectName)
            : base("entity-power-1", $"Entity id reference is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidEntityPowerProviderException : ExtendedException
    {
        public InvalidEntityPowerProviderException(string providerName, string reason)
            : base("entity-power-2", $"Invalid entity power provider '{providerName}': {reason}") { }
    }

    public sealed class MultiplePowerPanelViewException : ExtendedException
    {
        public MultiplePowerPanelViewException(string objectName)
            : base("entity-power-3", $"Multiple power panel views found under '{objectName}'") { }
    }

    public sealed class InvalidEntityPowerPanelBinderException : ExtendedException
    {
        public InvalidEntityPowerPanelBinderException(string binderName, string reason)
            : base("entity-power-4", $"Invalid entity power panel binder '{binderName}': {reason}") { }
    }
}
