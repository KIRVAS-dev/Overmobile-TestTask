using ViewComponents.Animation;
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
            CharacterAnimationView characterAnimationView,
            EntityPowerView entityPowerView,
            ActiveCharacterAnchorView activeCharacterAnchorView,
            PresentationSectionMap presentationSectionMap)
        {
            MovementView = movementView;
            CharacterAnimationView = characterAnimationView;
            EntityPowerView = entityPowerView;
            ActiveCharacterAnchorView = activeCharacterAnchorView;
            PresentationSectionMap = presentationSectionMap;
        }

        public MovementView MovementView { get; }
        public CharacterAnimationView CharacterAnimationView { get; }
        public EntityPowerView EntityPowerView { get; }
        public ActiveCharacterAnchorView ActiveCharacterAnchorView { get; }
        public PresentationSectionMap PresentationSectionMap { get; }
    }
}
