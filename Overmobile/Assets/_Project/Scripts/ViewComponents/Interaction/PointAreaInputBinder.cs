using Input;

namespace ViewComponents.Interaction
{
    public sealed class PointAreaInputBinder : IPointAreaInputBinder
    {
        private readonly InteractableTargetProvider _interactableTargetProvider;
        private readonly IPlayerPointerIntentGate _playerPointerIntentGate;

        public PointAreaInputBinder(
            InteractableTargetProvider interactableTargetProvider,
            IPlayerPointerIntentGate playerPointerIntentGate)
        {
            _interactableTargetProvider = interactableTargetProvider;
            _playerPointerIntentGate = playerPointerIntentGate;
        }

        public void BindPointAreas()
        {
            foreach (InteractableTarget interactableTarget in _interactableTargetProvider.InteractableTargets)
            {
                interactableTarget.PointArea.BindPlayerPointerInput(_playerPointerIntentGate);
            }
        }
    }
}
