using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using R3;
using System;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class InteractionDeferPowerPanelBinding : IDisposable
    {
        private readonly IDisposable _powerSubscription;
        private readonly IDisposable _phaseSubscription;

        public InteractionDeferPowerPanelBinding(
            IPowerRegistry powerRegistry,
            string entityId,
            IPowerPanelView powerPanelView,
            IInteractionPhaseSource interactionPhaseSource,
            PowerPanelValueChangeView valueChangeView)
        {
            PowerPanelValueApplier valueApplier = new PowerPanelValueApplier(powerPanelView, valueChangeView);
            IPowerEntity powerEntity = powerRegistry.Get(entityId);

            if (interactionPhaseSource.CurrentPhase.CurrentValue == InteractionPhase.Idle)
            {
                valueApplier.Apply(powerEntity.Power.CurrentValue);
            }
            else
            {
                valueApplier.SetWithoutAnimation(powerEntity.Power.CurrentValue);
            }

            _powerSubscription = powerEntity.Power.Subscribe(power =>
                {
                    if (interactionPhaseSource.CurrentPhase.CurrentValue != InteractionPhase.Idle)
                    {
                        return;
                    }

                    valueApplier.Apply(power);
                }
            );

            _phaseSubscription = interactionPhaseSource.CurrentPhase.Subscribe(phase =>
                {
                    if (phase != InteractionPhase.Idle)
                    {
                        return;
                    }

                    valueApplier.Apply(powerEntity.Power.CurrentValue);
                }
            );
        }

        public void Dispose()
        {
            _phaseSubscription?.Dispose();
            _powerSubscription?.Dispose();
        }
    }
}
