using Core.Gameplay.Movement;

namespace Core.Gameplay.Player
{
    public sealed class PlayerUpgradeService : IPlayerUpgradeService
    {
        private readonly IMovementService _movementService;
        private readonly IPlayerUpgradeView _playerUpgradeView;

        public PlayerUpgradeService(
            IMovementService movementService,
            IPlayerUpgradeView playerUpgradeView)
        {
            _movementService = movementService;
            _playerUpgradeView = playerUpgradeView;
        }

        public void Upgrade()
        {
            if (_movementService.IsMoving)
            {
                throw new PlayerUpgradeBlockedWhileMovingException();
            }

            _playerUpgradeView.Upgrade();
        }
    }
}
