using Core.Animation;
using Core.Gameplay.Attack;
using Core.Gameplay.Character;
using Core.Gameplay.Movement;

namespace ViewComponents.Player
{
    public sealed class ActiveCharacterViewBinding : ICharacterView
    {
        public ActiveCharacterViewBinding(
            ICharacterAnimationView animationView,
            IAttackView attackView,
            IMovementView movementView)
        {
            AnimationView = animationView;
            AttackView = attackView;
            MovementView = movementView;
        }

        public ICharacterAnimationView AnimationView { get; }
        public IAttackView AttackView { get; }
        public IMovementView MovementView { get; }
    }
}
