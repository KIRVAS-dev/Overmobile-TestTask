using Core.Animation;
using Core.Gameplay.Attack;
using Core.Gameplay.Movement;

namespace Core.Gameplay.Character
{
    public interface ICharacterView
    {
        ICharacterAnimationView AnimationView { get; }
        IAttackView AttackView { get; }
        IMovementView MovementView { get; }
    }
}
