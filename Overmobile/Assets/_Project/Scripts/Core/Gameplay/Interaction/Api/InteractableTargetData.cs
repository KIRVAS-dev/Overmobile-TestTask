using Core.Animation;
using Core.Gameplay.Entity;
using Core.Gameplay.Inventory;
using System.Collections.Generic;

namespace Core.Gameplay.Interaction
{
    public readonly struct InteractableTargetData
    {
        public string EntityKey { get; }
        public string EntityId { get; }
        public ICharacterAnimationView AnimationView { get; }
        public EntityType Type { get; }
        public ItemType? RequiredItem { get; }
        public ItemType? LootItem { get; }
        public IReadOnlyList<string> GuardEntityIds { get; }

        public InteractableTargetData(
            string entityKey,
            string entityId,
            ICharacterAnimationView animationView,
            EntityType type,
            ItemType? requiredItem,
            ItemType? lootItem,
            IReadOnlyList<string> guardEntityIds)
        {
            EntityKey = entityKey;
            EntityId = entityId;
            AnimationView = animationView;
            Type = type;
            RequiredItem = requiredItem;
            LootItem = lootItem;
            GuardEntityIds = guardEntityIds;
        }
    }
}
