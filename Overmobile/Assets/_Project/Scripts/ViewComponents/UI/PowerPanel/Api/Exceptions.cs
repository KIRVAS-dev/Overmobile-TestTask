using ExtendedExceptions;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class MissingPowerPanelConfigException : ExtendedException
    {
        public MissingPowerPanelConfigException(string objectName)
            : base("power-panel-1", $"Power panel config is not assigned on '{objectName}'") { }
    }
}
