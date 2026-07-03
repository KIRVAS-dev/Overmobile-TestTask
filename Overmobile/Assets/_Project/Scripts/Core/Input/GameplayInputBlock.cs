using Input;

namespace Core.Input
{
    public sealed class GameplayInputBlock : IGameplayInputBlock
    {
        private readonly IPlayerPointerInputActivation _playerPointerInputActivation;
        private int _blockCount;

        public GameplayInputBlock(IPlayerPointerInputActivation playerPointerInputActivation)
        {
            _playerPointerInputActivation = playerPointerInputActivation;
        }

        public bool IsBlocked => _blockCount > 0;

        public void Block()
        {
            _blockCount++;

            if (_blockCount == 1)
            {
                _playerPointerInputActivation.Disable();
            }
        }

        public void Unblock()
        {
            if (_blockCount == 0)
            {
                throw new GameplayInputBlockReleaseWithoutActiveBlockException();
            }

            _blockCount--;

            if (_blockCount == 0)
            {
                _playerPointerInputActivation.Enable();
            }
        }
    }
}
