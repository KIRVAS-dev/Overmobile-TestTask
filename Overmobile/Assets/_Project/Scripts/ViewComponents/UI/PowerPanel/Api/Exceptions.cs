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

    public sealed class MissingPowerPanelChangeConfigException : ExtendedException
    {
        public MissingPowerPanelChangeConfigException(string objectName)
            : base("power-panel-7", $"Power panel change config is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidPanelScaleChangeDurationException : ExtendedException
    {
        public InvalidPanelScaleChangeDurationException(string objectName, float durationSeconds)
            : base("power-panel-8", $"Panel scale change duration on '{objectName}' is invalid: {durationSeconds}") { }
    }

    public sealed class MissingPowerPanelScaleTargetException : ExtendedException
    {
        public MissingPowerPanelScaleTargetException(string objectName)
            : base("power-panel-9", $"Power panel scale target is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidPowerPanelInteractionDeferViewException : ExtendedException
    {
        public InvalidPowerPanelInteractionDeferViewException(string objectName, string reason)
            : base("power-panel-10", $"Invalid power panel interaction defer view on '{objectName}': {reason}") { }
    }

    public sealed class InvalidTextScaleChangeDurationException : ExtendedException
    {
        public InvalidTextScaleChangeDurationException(string objectName, float durationSeconds)
            : base("power-panel-11", $"Text scale change duration on '{objectName}' is invalid: {durationSeconds}") { }
    }

    public sealed class MissingTextScaleTargetException : ExtendedException
    {
        public MissingTextScaleTargetException(string objectName)
            : base("power-panel-12", $"Text scale target is not assigned on '{objectName}'") { }
    }
}
