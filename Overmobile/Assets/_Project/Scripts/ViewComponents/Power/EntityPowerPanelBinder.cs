using Core.Gameplay.Power;

namespace ViewComponents.Power
{
    public sealed class EntityPowerPanelBinder : IEntityPowerPanelBinder
    {
        private readonly EntityPowerProvider _entityPowerProvider;
        private readonly IPowerRegistry _powerRegistry;

        public EntityPowerPanelBinder(EntityPowerProvider entityPowerProvider, IPowerRegistry powerRegistry)
        {
            this._entityPowerProvider = entityPowerProvider;
            this._powerRegistry = powerRegistry;
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
