using Core.Gameplay.Movement;

namespace Core.Input
{
    public readonly struct GameplayInputTarget
    {
        public MovementInputTarget Movement { get; }
        public string PowerId { get; }

        public GameplayInputTarget(MovementInputTarget movement, string powerId)
        {
            Movement = movement;
            PowerId = powerId;
        }
    }
}
