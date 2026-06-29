using ExtendedExceptions;

namespace Core.Input
{
    public sealed class GameplayInputBlockReleaseWithoutActiveBlockException : ExtendedException
    {
        public GameplayInputBlockReleaseWithoutActiveBlockException()
            : base("core-input-1", "Gameplay input block was released without an active block") { }
    }

    public sealed class MovementInputHandlerAlreadyListeningException : ExtendedException
    {
        public MovementInputHandlerAlreadyListeningException()
            : base("core-input-2", "Movement input handler is already listening") { }
    }
}
