using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using R3;
using System;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class InteractionDeferPowerPanelBinding : IDisposable
    {
        private readonly IPlayerPowerDisplayState _playerPowerDisplayState;
        private readonly IDisposable _powerSubscription;
        private readonly IDisposable _phaseSubscription;

        public InteractionDeferPowerPanelBinding(
            IPowerRegistry powerRegistry,
            string entityId,
            IPowerPanelView powerPanelView,
            IInteractionPhaseSource interactionPhaseSource,
            PowerPanelValueChangeView valueChangeView,
            IPlayerPowerDisplayState playerPowerDisplayState)
        {
            _playerPowerDisplayState = playerPowerDisplayState;

            PowerPanelValueApplier valueApplier = new PowerPanelValueApplier(powerPanelView, valueChangeView);
            IPowerEntity powerEntity = powerRegistry.Get(entityId);
            int currentPower = powerEntity.Power.CurrentValue;

            if (interactionPhaseSource.CurrentPhase.CurrentValue == InteractionPhase.Idle)
            {
                valueApplier.Apply(currentPower);
                RecordDisplayedPower(currentPower);
            }
            else
            {
                int? displayedBaseline = ResolveDisplayedBaseline(currentPower);
                valueApplier.SetWithoutAnimation(currentPower, displayedBaseline);
            }

            _powerSubscription = powerEntity.Power.Subscribe(power =>
                {
                    if (interactionPhaseSource.CurrentPhase.CurrentValue != InteractionPhase.Idle)
                    {
                        valueApplier.UpdateSilently(power);

                        return;
                    }

                    valueApplier.Apply(power);
                    RecordDisplayedPower(power);
                }
            );

            _phaseSubscription = interactionPhaseSource.CurrentPhase.Subscribe(phase =>
                {
                    if (phase != InteractionPhase.Idle)
                    {
                        return;
                    }

                    int power = powerEntity.Power.CurrentValue;
                    valueApplier.Apply(power);
                    RecordDisplayedPower(power);
                }
            );
        }

        void IDisposable.Dispose()
        {
            _phaseSubscription?.Dispose();
            _powerSubscription?.Dispose();
        }

        private int? ResolveDisplayedBaseline(int currentPower)
        {
            return _playerPowerDisplayState.LastDisplayed ?? currentPower;
        }

        private void RecordDisplayedPower(int power)
        {
            _playerPowerDisplayState.Record(power);
        }
    }
}
