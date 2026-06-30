using Core.Gameplay.Power;

namespace ViewComponents.Power
{
    public sealed class EntityPowerPanelBinder : IEntityPowerPanelBinder
    {
        private readonly IPowerRegistry _powerRegistry;
        private readonly EntityPowerProvider _entityPowerProvider;

        public EntityPowerPanelBinder(IPowerRegistry powerRegistry, EntityPowerProvider entityPowerProvider)
        {
            _powerRegistry = powerRegistry;
            _entityPowerProvider = entityPowerProvider;
        }

        public void BindPowerPanel(EntityPowerView entityPowerView)
        {
            if (entityPowerView == null)
            {
                throw new InvalidEntityPowerPanelBinderException(
                    nameof(EntityPowerPanelBinder),
                    "Entity power view is not assigned"
                );
            }

            EntityPower entityPower = entityPowerView.GetComponent<EntityPower>();

            string entityId = entityPower != null
                ? entityPower.EntityId
                : _entityPowerProvider.PlayerEntityId;

            entityPowerView.BindPowerPanel(_powerRegistry, entityId);
        }
    }
}
