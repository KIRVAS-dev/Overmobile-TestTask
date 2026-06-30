using Core.Gameplay.Power;

namespace ViewComponents.Interaction
{
    public sealed class DropBinder : IDropBinder
    {
        private readonly InteractableTargetProvider _interactableTargetProvider;
        private readonly IPowerRegistry _powerRegistry;

        public DropBinder(InteractableTargetProvider interactableTargetProvider, IPowerRegistry powerRegistry)
        {
            _interactableTargetProvider = interactableTargetProvider;
            _powerRegistry = powerRegistry;
        }

        public void BindLootDrops()
        {
            _interactableTargetProvider.BindLootDrops(_powerRegistry);
        }
    }
}
