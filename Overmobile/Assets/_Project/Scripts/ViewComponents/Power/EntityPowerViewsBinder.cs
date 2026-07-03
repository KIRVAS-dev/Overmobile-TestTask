namespace ViewComponents.Power
{
    public sealed class EntityPowerViewsBinder : IEntityPowerViewsBinder
    {
        private readonly EntityPowerProvider _entityPowerProvider;
        private readonly IEntityPowerPanelBinder _entityPowerPanelBinder;
        private readonly IEntityGuardPowerPanelBinder _entityGuardPowerPanelBinder;

        public EntityPowerViewsBinder(
            EntityPowerProvider entityPowerProvider,
            IEntityPowerPanelBinder entityPowerPanelBinder,
            IEntityGuardPowerPanelBinder entityGuardPowerPanelBinder)
        {
            _entityPowerProvider = entityPowerProvider;
            _entityPowerPanelBinder = entityPowerPanelBinder;
            _entityGuardPowerPanelBinder = entityGuardPowerPanelBinder;
        }

        public void BindEntityPowerViews()
        {
            foreach (EntityPowerView entityPowerView in _entityPowerProvider.GetInteractableEntityPowerViews())
            {
                _entityPowerPanelBinder.BindPowerPanel(entityPowerView);
                _entityGuardPowerPanelBinder.BindGuardPowerPanel(entityPowerView);
            }
        }
    }
}
