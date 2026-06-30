using Core.Gameplay.Power;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Power
{
    public sealed class EntityPowerProvider
        : MonoBehaviour,
          IEntityPowerProvider
    {
        [SerializeField] private EntityPower[] _entityPowers;

        public string HeroPowerId => EntityPowerSceneHelper.ResolveHeroEntityPower(_entityPowers, gameObject.name).PowerId;

        public IReadOnlyList<EntityPowerData> GetEntityPowers()
        {
            EntityPower[] entityPowerComponents = EntityPowerSceneHelper.GetValidatedEntityPowers(_entityPowers, gameObject.name);

            List<EntityPowerData> entityPowers = new List<EntityPowerData>(entityPowerComponents.Length);

            foreach (EntityPower entityPower in entityPowerComponents)
            {
                entityPowers.Add(new EntityPowerData(entityPower.PowerId, entityPower.InitialPower));
            }

            return entityPowers;
        }

        public IReadOnlyList<EntityPowerView> GetInteractableEntityPowerViews()
        {
            return EntityPowerSceneHelper.GetInteractableEntityPowerViews(_entityPowers, gameObject.name);
        }
    }
}
