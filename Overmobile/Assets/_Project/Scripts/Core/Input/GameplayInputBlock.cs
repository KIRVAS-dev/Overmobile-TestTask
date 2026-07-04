using Input;
using R3;

namespace Core.Input
{
    public sealed class GameplayInputBlock : IGameplayInputBlock
    {
        private readonly IPlayerPointerInputActivation _playerPointerInputActivation;
        private readonly ReactiveProperty<bool> _isBlocked = new ReactiveProperty<bool>(false);

        private int _blockCount;

        public GameplayInputBlock(IPlayerPointerInputActivation playerPointerInputActivation)
        {
            _playerPointerInputActivation = playerPointerInputActivation;
        }

        public ReadOnlyReactiveProperty<bool> IsBlocked => _isBlocked;

        public void Block()
        {
            _blockCount++;

            if (_blockCount != 1)
            {
                return;
            }

            _playerPointerInputActivation.Disable();
            _isBlocked.Value = true;
        }

        public void Unblock()
        {
            if (_blockCount == 0)
            {
                throw new GameplayInputBlockReleaseWithoutActiveBlockException();
            }

            _blockCount--;

            if (_blockCount != 0)
            {
                return;
            }

            _playerPointerInputActivation.Enable();
            _isBlocked.Value = false;
        }
    }
}
