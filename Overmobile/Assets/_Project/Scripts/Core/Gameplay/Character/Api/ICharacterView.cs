using Core.Gameplay.Animation;
using Core.Gameplay.Movement;

namespace Core.Gameplay.Character
{
    public interface ICharacterView
    {
        ICharacterAnimationView AnimationView { get; }

        IMovementView MovementView { get; }
    }
}
