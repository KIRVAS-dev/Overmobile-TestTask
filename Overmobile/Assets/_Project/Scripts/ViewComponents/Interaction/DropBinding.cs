using Core.Gameplay.Power;
using R3;
using System;

namespace ViewComponents.Interaction
{
    public sealed class DropBinding : IDisposable
    {
        private readonly IDisposable _resolvedSubscription;

        public DropBinding(IPowerRegistry powerRegistry, string entityId, DropView dropView)
        {
            IPowerEntity powerEntity = powerRegistry.Get(entityId);

            if (powerEntity.IsResolved.CurrentValue)
            {
                dropView.HideLootSource();

                return;
            }

            _resolvedSubscription = powerEntity.IsResolved
                .Where(isResolved => isResolved)
                .Subscribe(_ => dropView.HideLootSource());
        }

        public void Dispose()
        {
            _resolvedSubscription?.Dispose();
        }
    }
}
