using ExtendedExceptions;

namespace ViewComponents.PowerPanel
{
    public sealed class MissingPowerPanelConfigException : ExtendedException
    {
        public MissingPowerPanelConfigException(string objectName)
            : base("Missing_power_panel_config", $"Power panel config is not assigned on '{objectName}'.") { }
    }
}
