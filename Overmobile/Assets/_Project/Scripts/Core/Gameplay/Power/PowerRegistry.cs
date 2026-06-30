using System.Collections.Generic;

namespace Core.Gameplay.Power
{
    public sealed class PowerRegistry : IPowerRegistry
    {
        private readonly Dictionary<string, PowerEntityModel> _entities = new Dictionary<string, PowerEntityModel>();

        public PowerRegistry(IEntityPowerProvider entityPowerProvider)
        {
            PlayerEntityId = entityPowerProvider.PlayerEntityId;

            if (string.IsNullOrWhiteSpace(PlayerEntityId))
            {
                throw new InvalidEntityPowerProviderException(nameof(entityPowerProvider), "Player entity id is not assigned");
            }

            IReadOnlyList<EntityPowerData> entityPowers = entityPowerProvider.GetEntityPowers();

            foreach (EntityPowerData entityPowerData in entityPowers)
            {
                if (_entities.ContainsKey(entityPowerData.EntityId))
                {
                    throw new DuplicateEntityIdException(entityPowerData.EntityId);
                }

                _entities.Add(
                    entityPowerData.EntityId,
                    new PowerEntityModel(entityPowerData.EntityId, entityPowerData.InitialPower)
                );
            }

            if (!_entities.ContainsKey(PlayerEntityId))
            {
                throw new PlayerEntityNotFoundException();
            }
        }

        public string PlayerEntityId { get; }

        public IPowerEntity Get(string entityId)
        {
            return GetModel(entityId);
        }

        public bool IsResolved(string entityId)
        {
            return GetModel(entityId).IsResolved.CurrentValue;
        }

        public bool TryTransferPowerToPlayer(string sourceEntityId, bool requirePlayerPowerGreater)
        {
            PowerEntityModel source = GetModel(sourceEntityId);

            if (source.IsResolved.CurrentValue)
            {
                return false;
            }

            PowerEntityModel player = GetModel(PlayerEntityId);

            if (requirePlayerPowerGreater && player.Power.CurrentValue <= source.Power.CurrentValue)
            {
                return false;
            }

            player.AddPower(source.Power.CurrentValue);
            source.MarkResolved();

            return true;
        }

        private PowerEntityModel GetModel(string entityId)
        {
            return !_entities.TryGetValue(entityId, out PowerEntityModel entity)
                ? throw new EntityPowerNotFoundException(entityId)
                : entity;
        }
    }
}
