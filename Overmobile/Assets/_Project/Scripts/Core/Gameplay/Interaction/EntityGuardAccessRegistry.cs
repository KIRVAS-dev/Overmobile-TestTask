using Core.Gameplay.Power;
using R3;
using System.Collections.Generic;

namespace Core.Gameplay.Interaction
{
    public sealed class EntityGuardAccessRegistry : IEntityGuardAccessRegistry
    {
        private readonly ReadOnlyReactiveProperty<bool> _notBlocking = new ReactiveProperty<bool>(false);
        private readonly IPowerRegistry _powerRegistry;
        private readonly Dictionary<string, ReadOnlyReactiveProperty<bool>> _blockingByEntityId =
            new Dictionary<string, ReadOnlyReactiveProperty<bool>>();

        public EntityGuardAccessRegistry(IInteractableTargetProvider interactableTargetProvider, IPowerRegistry powerRegistry)
        {
            _powerRegistry = powerRegistry;
            IReadOnlyList<InteractableTargetData> interactableTargets = interactableTargetProvider.GetInteractableTargets();

            foreach (InteractableTargetData targetData in interactableTargets)
            {
                if (targetData.GuardEntityIds.Count == 0)
                {
                    continue;
                }

                _blockingByEntityId[targetData.EntityId] = BuildBlockingProperty(targetData.GuardEntityIds);
            }
        }

        public bool HasGuards(string entityId)
        {
            return _blockingByEntityId.ContainsKey(entityId);
        }

        public ReadOnlyReactiveProperty<bool> GetAreGuardsBlocking(string entityId)
        {
            return _blockingByEntityId.TryGetValue(entityId, out ReadOnlyReactiveProperty<bool> blocking)
                ? blocking
                : _notBlocking;
        }

        private ReadOnlyReactiveProperty<bool> BuildBlockingProperty(IReadOnlyList<string> guardEntityIds)
        {
            ReactiveProperty<bool> blocking = new ReactiveProperty<bool>(IsAnyGuardUnresolved(guardEntityIds));

            foreach (string guardEntityId in guardEntityIds)
            {
                _powerRegistry
                   .Get(guardEntityId)
                   .IsResolved
                   .Subscribe(_ => blocking.Value = IsAnyGuardUnresolved(guardEntityIds));
            }

            return blocking;
        }

        private bool IsAnyGuardUnresolved(IReadOnlyList<string> guardEntityIds)
        {
            foreach (string guardEntityId in guardEntityIds)
            {
                if (!_powerRegistry.Get(guardEntityId).IsResolved.CurrentValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
