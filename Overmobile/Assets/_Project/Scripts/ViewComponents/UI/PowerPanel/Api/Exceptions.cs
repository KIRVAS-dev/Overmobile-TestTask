using ExtendedExceptions;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class MissingPowerPanelConfigException : ExtendedException
    {
        public MissingPowerPanelConfigException(string objectName)
            : base("power-panel-1", $"Power panel config is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPowerPanelViewException : ExtendedException
    {
        public MissingPowerPanelViewException(string objectName)
            : base("power-panel-2", $"Power panel view is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPowerPanelVisibilityViewException : ExtendedException
    {
        public MissingPowerPanelVisibilityViewException(string objectName)
            : base("power-panel-3", $"Power panel visibility view is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPowerPanelVisibilityConfigException : ExtendedException
    {
        public MissingPowerPanelVisibilityConfigException(string objectName)
            : base("power-panel-4", $"Power panel visibility config is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidPowerPanelVisibilityFadeDurationException : ExtendedException
    {
        public InvalidPowerPanelVisibilityFadeDurationException(string objectName, float fadeDuration)
            : base("power-panel-5", $"Power panel visibility fade duration on '{objectName}' is invalid: {fadeDuration}") { }
    }

    public sealed class InvalidPowerPanelVisibilityViewException : ExtendedException
    {
        public InvalidPowerPanelVisibilityViewException(string objectName, string reason)
            : base("power-panel-6", $"Invalid power panel visibility view on '{objectName}': {reason}") { }
    }
}
