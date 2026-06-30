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
}
