namespace ViewComponents.UI.PowerPanel
{
    public interface IPlayerPowerDisplayState
    {
        int? LastDisplayed { get; }

        void Record(int power);
    }
}
