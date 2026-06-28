using Core.Animation;
using Core.Gameplay.Movement;

namespace Core.Gameplay.Character
{
    public sealed class CharacterViewBinding : ICharacterView
    {
        public CharacterViewBinding(ICharacterAnimationView animationView, IMovementView movementView)
        {
            AnimationView = animationView;
            MovementView = movementView;
        }

        public ICharacterAnimationView AnimationView { get; }

        public IMovementView MovementView { get; }
    }
}
