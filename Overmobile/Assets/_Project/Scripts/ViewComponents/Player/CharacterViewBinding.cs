using Core.Animation;
using Core.Gameplay.Character;
using Core.Gameplay.Movement;

namespace ViewComponents.Player
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
