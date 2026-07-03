using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using R3;
using System;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class EntityGuardPowerPanelBinding : IDisposable
    {
        private readonly IDisposable _blockingSubscription;

        public EntityGuardPowerPanelBinding(IEntityGuardAccessRegistry guardAccessRegistry, string entityId,
            IPowerPanelVisibilityView powerPanelVisibilityView)
        {
            ReadOnlyReactiveProperty<bool> areGuardsBlocking = guardAccessRegistry.GetAreGuardsBlocking(entityId);

            ApplyVisibility(powerPanelVisibilityView, areGuardsBlocking.CurrentValue);

            _blockingSubscription = areGuardsBlocking.Subscribe(isBlocking =>
                ApplyVisibility(powerPanelVisibilityView, isBlocking)
            );
        }

        void IDisposable.Dispose()
        {
            _blockingSubscription.Dispose();
        }

        private void ApplyVisibility(IPowerPanelVisibilityView powerPanelVisibilityView, bool areGuardsBlocking)
        {
            if (areGuardsBlocking)
            {
                powerPanelVisibilityView.Hide();
            }
            else
            {
                powerPanelVisibilityView.Show();
            }
        }
    }
}
