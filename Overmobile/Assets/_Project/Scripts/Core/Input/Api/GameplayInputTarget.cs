using Core.Gameplay.Movement;

namespace Core.Input
{
    public readonly struct GameplayInputTarget
    {
        public MovementInputTarget Movement { get; }
        public string EntityId { get; }

        public GameplayInputTarget(MovementInputTarget movement, string entityId)
        {
            Movement = movement;
            EntityId = entityId;
        }
    }
}
