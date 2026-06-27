namespace Core.Gameplay.Character
{
    public sealed class ActiveCharacterViewRegistry
        : IActiveCharacterViewProvider,
          IActiveCharacterViewRegistry
    {
        private ICharacterView _activeCharacterView;

        public ICharacterView ActiveCharacterView =>
            _activeCharacterView ?? throw new ActiveCharacterViewNotRegisteredException();

        public void SetActiveCharacterView(ICharacterView characterView)
        {
            _activeCharacterView = characterView ?? throw new InvalidActiveCharacterViewRegistrationException();
        }
    }
}
