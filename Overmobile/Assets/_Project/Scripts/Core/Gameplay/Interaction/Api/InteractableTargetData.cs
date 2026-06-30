using Core.Animation;
using Core.Gameplay.Entity;
using Core.Gameplay.Inventory;

namespace Core.Gameplay.Interaction
{
    public readonly struct InteractableTargetData
    {
        public string EntityKey { get; }
        public string PowerId { get; }
        public ICharacterAnimationView AnimationView { get; }
        public EntityType Type { get; }
        public ItemType? RequiredItem { get; }

        public InteractableTargetData(
            string entityKey,
            string powerId,
            ICharacterAnimationView animationView,
            EntityType type,
            ItemType? requiredItem)
        {
            EntityKey = entityKey;
            PowerId = powerId;
            AnimationView = animationView;
            Type = type;
            RequiredItem = requiredItem;
        }
    }
}
