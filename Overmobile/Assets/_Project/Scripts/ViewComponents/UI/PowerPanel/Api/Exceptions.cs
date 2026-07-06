using ExtendedExceptions;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class MissingPowerPanelFieldException : ExtendedException
    {
        public MissingPowerPanelFieldException(string fieldName, string objectName)
            : base("power-panel-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidPowerPanelValueException : ExtendedException
    {
        public InvalidPowerPanelValueException(string fieldName, float value)
            : base("power-panel-2", $"Field '{fieldName}' has invalid value: {value}") { }

        public InvalidPowerPanelValueException(
            string fieldName,
            string objectName,
            float value)
            : base("power-panel-3", $"Field '{fieldName}' on '{objectName}' has invalid value: {value}") { }
    }

    public sealed class MissingPowerPanelViewException : ExtendedException
    {
        public MissingPowerPanelViewException(string objectName)
            : base("power-panel-4", $"Power panel view is not assigned on '{objectName}'") { }
    }
}
