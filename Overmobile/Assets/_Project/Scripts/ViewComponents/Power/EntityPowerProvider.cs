using Core.Gameplay.Entity;
using Core.Gameplay.Power;
using System.Collections.Generic;
using UnityEngine;
using ViewComponents.Entity;

namespace ViewComponents.Power
{
    public sealed class EntityPowerProvider
        : MonoBehaviour,
          IEntityPowerProvider
    {
        [SerializeField] private EntityPower[] _entityPowers;

        public string PlayerEntityId =>
            EntityPowerSceneHelper.ResolvePlayerEntityPower(_entityPowers, gameObject.name).EntityId;

        public IReadOnlyList<EntityPowerData> GetEntityPowers()
        {
            EntityPower[] entityPowerComponents = EntityPowerSceneHelper.GetValidatedEntityPowers(_entityPowers, gameObject.name);

            List<EntityPowerData> entityPowers = new List<EntityPowerData>(entityPowerComponents.Length);

            foreach (EntityPower entityPower in entityPowerComponents)
            {
                EntityRole entityRole = entityPower.GetComponent<EntityRole>();

                if (entityRole == null)
                {
                    throw new InvalidEntityPowerProviderException(
                        entityPower.gameObject.name,
                        "Entity role is not assigned"
                    );
                }

                entityPowers.Add(new EntityPowerData(entityPower.EntityId, entityPower.InitialPower, entityRole.Type));
            }

            return entityPowers;
        }

        public IReadOnlyList<EntityPowerView> GetInteractableEntityPowerViews()
        {
            return EntityPowerSceneHelper.GetInteractableEntityPowerViews(_entityPowers, gameObject.name);
        }
    }
}
