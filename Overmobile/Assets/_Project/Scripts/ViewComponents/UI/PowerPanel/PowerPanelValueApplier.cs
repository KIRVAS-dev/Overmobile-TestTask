using Core.Gameplay.Power;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class PowerPanelValueApplier
    {
        private readonly IPowerPanelView _powerPanelView;
        private readonly PowerPanelValueChangeView _valueChangeView;

        private bool _hasDisplayedPower;
        private int _displayedPower;

        public PowerPanelValueApplier(IPowerPanelView powerPanelView, PowerPanelValueChangeView valueChangeView)
        {
            _powerPanelView = powerPanelView;
            _valueChangeView = valueChangeView;
        }

        public void Apply(int power)
        {
            bool isValueChange = _hasDisplayedPower && power != _displayedPower;

            _powerPanelView.SetPower(power);

            if (isValueChange)
            {
                _valueChangeView?.PlayValueChangeAnimation();
            }

            _hasDisplayedPower = true;
            _displayedPower = power;
        }

        public void SetWithoutAnimation(int power)
        {
            _powerPanelView.SetPower(power);
            _hasDisplayedPower = true;
            _displayedPower = power;
        }
    }
}
