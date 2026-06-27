namespace Core.Gameplay.Movement
{
    public sealed class ActiveMovementViewRegistry
        : IActiveMovementViewProvider,
          IActiveMovementViewRegistry
    {
        private IMovementView _activeMovementView;

        public IMovementView ActiveMovementView => _activeMovementView ?? throw new ActiveMovementViewNotRegisteredException();

        public void SetActiveMovementView(IMovementView movementView)
        {
            _activeMovementView = movementView ?? throw new InvalidActiveMovementViewRegistrationException();
        }
    }
}
