namespace ViewComponents.UI.PowerPanel
{
    public sealed class PlayerPowerDisplayState : IPlayerPowerDisplayState
    {
        public int? LastDisplayed { get; private set; }

        public void Record(int power)
        {
            LastDisplayed = power;
        }
    }
}
