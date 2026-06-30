using Core.Gameplay.Power;
using R3;
using System;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class PowerPanelBinding : IDisposable
    {
        private readonly IDisposable _powerSubscription;

        public PowerPanelBinding(IPowerRegistry powerRegistry, string entityId, IPowerPanelView powerPanelView)
        {
            IPowerEntity powerEntity = powerRegistry.Get(entityId);
            powerPanelView.SetPower(powerEntity.Power.CurrentValue);
            _powerSubscription = powerEntity.Power.Subscribe(powerPanelView.SetPower);
        }

        public void Dispose()
        {
            _powerSubscription?.Dispose();
        }
    }
}
