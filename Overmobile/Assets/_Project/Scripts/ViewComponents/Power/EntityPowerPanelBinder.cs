using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using ViewComponents.UI.PowerPanel;

namespace ViewComponents.Power
{
    public sealed class EntityPowerPanelBinder : IEntityPowerPanelBinder
    {
        private readonly IPowerRegistry _powerRegistry;
        private readonly IInteractionPhaseSource _interactionPhaseSource;
        private readonly IPlayerPowerDisplayState _playerPowerDisplayState;
        private readonly EntityPowerProvider _entityPowerProvider;

        public EntityPowerPanelBinder(
            IPowerRegistry powerRegistry,
            IInteractionPhaseSource interactionPhaseSource,
            IPlayerPowerDisplayState playerPowerDisplayState,
            EntityPowerProvider entityPowerProvider)
        {
            _powerRegistry = powerRegistry;
            _interactionPhaseSource = interactionPhaseSource;
            _playerPowerDisplayState = playerPowerDisplayState;
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

            PowerPanelValueChangeView valueChangeView = ResolveValueChangeView(entityPowerView);
            PowerPanelInteractionDeferView interactionDeferView = ResolveInteractionDeferView(entityPowerView);

            if (interactionDeferView != null)
            {
                interactionDeferView.Bind(
                    _interactionPhaseSource,
                    _powerRegistry,
                    entityId,
                    valueChangeView,
                    _playerPowerDisplayState
                );

                return;
            }

            entityPowerView.BindPowerPanel(_powerRegistry, entityId, valueChangeView);
        }

        private PowerPanelInteractionDeferView ResolveInteractionDeferView(EntityPowerView entityPowerView)
        {
            PowerPanelInteractionDeferView[] interactionDeferViews =
                entityPowerView.GetComponentsInChildren<PowerPanelInteractionDeferView>(includeInactive: true);

            switch (interactionDeferViews.Length)
            {
                case 0:
                    return null;

                case > 1:
                    throw new MultiplePowerPanelInteractionDeferViewException(entityPowerView.gameObject.name);

                default:
                    return interactionDeferViews[0];
            }
        }

        private PowerPanelValueChangeView ResolveValueChangeView(EntityPowerView entityPowerView)
        {
            PowerPanelValueChangeView[] valueChangeViews =
                entityPowerView.GetComponentsInChildren<PowerPanelValueChangeView>(includeInactive: true);

            switch (valueChangeViews.Length)
            {
                case 0:
                    return null;

                case > 1:
                    throw new MultiplePowerPanelValueChangeViewException(entityPowerView.gameObject.name);

                default:
                    return valueChangeViews[0];
            }
        }
    }
}
