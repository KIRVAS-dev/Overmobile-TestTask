using Core.Gameplay.Interaction;
using ViewComponents.UI.PowerPanel;

namespace ViewComponents.Power
{
    public sealed class EntityGuardPowerPanelBinder : IEntityGuardPowerPanelBinder
    {
        private readonly IEntityGuardAccessRegistry _guardAccessRegistry;

        public EntityGuardPowerPanelBinder(IEntityGuardAccessRegistry guardAccessRegistry)
        {
            _guardAccessRegistry = guardAccessRegistry;
        }

        public void BindGuardPowerPanel(EntityPowerView entityPowerView)
        {
            if (entityPowerView == null)
            {
                throw new InvalidEntityGuardPowerPanelBinderException(
                    nameof(EntityGuardPowerPanelBinder),
                    "Entity power view is not assigned"
                );
            }

            EntityPower entityPower = entityPowerView.GetComponent<EntityPower>();

            if (entityPower == null)
            {
                throw new InvalidEntityGuardPowerPanelBinderException(
                    entityPowerView.gameObject.name,
                    "Entity power is not assigned"
                );
            }

            if (!_guardAccessRegistry.HasGuards(entityPower.EntityId))
            {
                return;
            }

            PowerPanelVisibilityView powerPanelVisibilityView = ResolvePowerPanelVisibilityView(entityPowerView);
            powerPanelVisibilityView.Bind(_guardAccessRegistry, entityPower.EntityId);
        }

        private PowerPanelVisibilityView ResolvePowerPanelVisibilityView(EntityPowerView entityPowerView)
        {
            PowerPanelVisibilityView[] powerPanelVisibilityViews =
                entityPowerView.GetComponentsInChildren<PowerPanelVisibilityView>(includeInactive: true);

            switch (powerPanelVisibilityViews.Length)
            {
                case 0:
                    throw new MissingPowerPanelVisibilityViewException(entityPowerView.gameObject.name);

                case > 1:
                    throw new MultiplePowerPanelVisibilityViewException(entityPowerView.gameObject.name);

                default:
                    return powerPanelVisibilityViews[0];
            }
        }
    }
}
