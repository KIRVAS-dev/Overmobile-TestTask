using Input;

namespace Core.Input
{
    public sealed class GameplayInputBlock : IGameplayInputBlock
    {
        private readonly PlayerPointerInput _playerPointerInput;
        private int _blockCount;

        public GameplayInputBlock(PlayerPointerInput playerPointerInput)
        {
            _playerPointerInput = playerPointerInput;
        }

        public bool IsBlocked => _blockCount > 0;

        public void Block()
        {
            _blockCount++;

            if (_blockCount == 1)
            {
                _playerPointerInput.Disable();
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
                _playerPointerInput.Enable();
            }
        }
    }
}
