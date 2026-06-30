using Core.Gameplay.Entity;

namespace Core.Gameplay.Power
{
    public readonly struct EntityPowerData
    {
        public string EntityId { get; }
        public int InitialPower { get; }
        public EntityType EntityType { get; }

        public EntityPowerData(string entityId, int initialPower, EntityType entityType)
        {
            EntityId = entityId;
            InitialPower = initialPower;
            EntityType = entityType;
        }
    }
}
