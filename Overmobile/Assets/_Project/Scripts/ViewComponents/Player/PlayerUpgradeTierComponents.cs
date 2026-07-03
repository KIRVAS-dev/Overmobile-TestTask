using ViewComponents.Animation;
using ViewComponents.Attack;
using ViewComponents.Movement;
using ViewComponents.Power;
using ViewComponents.Presentation;
using ViewComponents.Presentation.Player;

namespace ViewComponents.Player
{
    public readonly struct PlayerUpgradeTierComponents
    {
        public PlayerUpgradeTierComponents(
            MovementView movementView,
            AttackView attackView,
            CharacterAnimationView characterAnimationView,
            EntityPowerView entityPowerView,
            ActiveCharacterAnchorView activeCharacterAnchorView,
            PresentationSectionMap presentationSectionMap)
        {
            MovementView = movementView;
            AttackView = attackView;
            CharacterAnimationView = characterAnimationView;
            EntityPowerView = entityPowerView;
            ActiveCharacterAnchorView = activeCharacterAnchorView;
            PresentationSectionMap = presentationSectionMap;
        }

        public MovementView MovementView { get; }
        public AttackView AttackView { get; }
        public CharacterAnimationView CharacterAnimationView { get; }
        public EntityPowerView EntityPowerView { get; }
        public ActiveCharacterAnchorView ActiveCharacterAnchorView { get; }
        public PresentationSectionMap PresentationSectionMap { get; }
    }
}
