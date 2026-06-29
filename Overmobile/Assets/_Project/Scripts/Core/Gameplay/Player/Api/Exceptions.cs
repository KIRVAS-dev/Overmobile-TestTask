using Core.Gameplay.Movement;
using ExtendedExceptions;

namespace Core.Gameplay.Player
{
    public sealed class PlayerUpgradeBlockedWhileMovingException : ExtendedException
    {
        public PlayerUpgradeBlockedWhileMovingException()
            : base("player-1", "Player upgrade is blocked while character is moving") { }
    }
}
