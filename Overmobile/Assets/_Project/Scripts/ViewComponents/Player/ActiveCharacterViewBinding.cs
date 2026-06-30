using Core.Animation;
using Core.Gameplay.Character;
using Core.Gameplay.Movement;

namespace ViewComponents.Player
{
    public sealed class ActiveCharacterViewBinding : ICharacterView
    {
        public ActiveCharacterViewBinding(ICharacterAnimationView animationView, IMovementView movementView)
        {
            AnimationView = animationView;
            MovementView = movementView;
        }

        public ICharacterAnimationView AnimationView { get; }

        public IMovementView MovementView { get; }
    }
}
