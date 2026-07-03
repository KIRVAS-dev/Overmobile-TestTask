using ExtendedExceptions;

namespace ViewComponents.Power
{
    public sealed class MissingEntityPowerFieldException : ExtendedException
    {
        public MissingEntityPowerFieldException(string fieldName, string objectName)
            : base("entity-power-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
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

    public sealed class MultiplePowerPanelVisibilityViewException : ExtendedException
    {
        public MultiplePowerPanelVisibilityViewException(string objectName)
            : base("entity-power-5", $"Multiple power panel visibility views found under '{objectName}'") { }
    }

    public sealed class InvalidEntityGuardPowerPanelBinderException : ExtendedException
    {
        public InvalidEntityGuardPowerPanelBinderException(string binderName, string reason)
            : base("entity-power-6", $"Invalid entity guard power panel binder '{binderName}': {reason}") { }
    }

    public sealed class InvalidEntityInteractionDeferPowerPanelBinderException : ExtendedException
    {
        public InvalidEntityInteractionDeferPowerPanelBinderException(string binderName, string reason)
            : base("entity-power-7", $"Invalid entity interaction defer power panel binder '{binderName}': {reason}") { }
    }

    public sealed class MultiplePowerPanelInteractionDeferViewException : ExtendedException
    {
        public MultiplePowerPanelInteractionDeferViewException(string objectName)
            : base("entity-power-8", $"Multiple power panel interaction defer views found under '{objectName}'") { }
    }

    public sealed class MultiplePowerPanelValueChangeViewException : ExtendedException
    {
        public MultiplePowerPanelValueChangeViewException(string objectName)
            : base("entity-power-9", $"Multiple power panel value change views found under '{objectName}'") { }
    }
}
