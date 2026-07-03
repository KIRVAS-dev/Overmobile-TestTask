using Core.Gameplay.Power;
using R3;
using System;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class PowerPanelBinding : IDisposable
    {
        private readonly IDisposable _powerSubscription;

        public PowerPanelBinding(
            IPowerRegistry powerRegistry,
            string entityId,
            IPowerPanelView powerPanelView,
            PowerPanelValueChangeView valueChangeView)
        {
            PowerPanelValueApplier valueApplier = new PowerPanelValueApplier(powerPanelView, valueChangeView);
            IPowerEntity powerEntity = powerRegistry.Get(entityId);
            valueApplier.Apply(powerEntity.Power.CurrentValue);

            _powerSubscription = powerEntity.Power.Subscribe(power =>
                {
                    valueApplier.Apply(power);
                }
            );
        }

        public void Dispose()
        {
            _powerSubscription?.Dispose();
        }
    }
}
