using Core.Gameplay.Entity;
using System.Collections.Generic;
using UnityEngine;
using ViewComponents.Entity;
using ViewComponents.Movement;

namespace ViewComponents.Power
{
    public static class EntityPowerSceneHelper
    {
        public static EntityPower[] GetValidatedEntityPowers(EntityPower[] serializedEntityPowers, string providerName)
        {
            EntityPower[] entityPowerComponents = ResolveEntityPowerComponents(serializedEntityPowers);
            ValidateEntityPowers(entityPowerComponents, providerName);

            return entityPowerComponents;
        }

        public static EntityPower ResolveHeroEntityPower(EntityPower[] serializedEntityPowers, string providerName)
        {
            EntityPower[] entityPowerComponents = GetValidatedEntityPowers(serializedEntityPowers, providerName);

            EntityPower heroEntityPower = null;

            foreach (EntityPower entityPower in entityPowerComponents)
            {
                if (!IsHeroEntityPower(entityPower))
                {
                    continue;
                }

                if (heroEntityPower != null)
                {
                    throw new InvalidEntityPowerProviderException(providerName, "Multiple hero entity powers are assigned");
                }

                heroEntityPower = entityPower;
            }

            return heroEntityPower == null
                ? throw new InvalidEntityPowerProviderException(providerName, "Hero entity power is not assigned")
                : heroEntityPower;
        }

        public static IReadOnlyList<EntityPowerView> GetInteractableEntityPowerViews(EntityPower[] serializedEntityPowers,
            string providerName)
        {
            EntityPower[] entityPowerComponents = GetValidatedEntityPowers(serializedEntityPowers, providerName);
            List<EntityPowerView> entityPowerViews = new List<EntityPowerView>(entityPowerComponents.Length);

            foreach (EntityPower entityPower in entityPowerComponents)
            {
                if (IsHeroEntityPower(entityPower))
                {
                    continue;
                }

                entityPowerViews.Add(entityPower.GetComponent<EntityPowerView>());
            }

            return entityPowerViews;
        }

        private static bool IsHeroEntityPower(EntityPower entityPower)
        {
            EntityRole entityRole = entityPower.GetComponent<EntityRole>();

            return entityRole != null && entityRole.Type == EntityType.Player;
        }

        private static EntityPower[] ResolveEntityPowerComponents(EntityPower[] serializedEntityPowers)
        {
            return serializedEntityPowers is
            {
                Length: > 0
            }
                ? serializedEntityPowers
                : Object.FindObjectsByType<EntityPower>();
        }

        private static void ValidateEntityPowers(EntityPower[] entityPowerComponents, string providerName)
        {
            if (entityPowerComponents == null
             || entityPowerComponents.Length == 0)
            {
                throw new InvalidEntityPowerProviderException(providerName, "Entity powers are not assigned");
            }

            HashSet<string> powerIds = new HashSet<string>();

            for (int i = 0; i < entityPowerComponents.Length; i++)
            {
                EntityPower entityPower = entityPowerComponents[i];

                if (entityPower == null)
                {
                    throw new InvalidEntityPowerProviderException(providerName, $"Entity power at index {i} is missing");
                }

                string powerId = entityPower.PowerId;

                if (!powerIds.Add(powerId))
                {
                    throw new InvalidEntityPowerProviderException(providerName, $"Duplicate power id '{powerId}'");
                }

                ValidateEntityPowerPlacement(entityPower);
            }
        }

        private static void ValidateEntityPowerPlacement(EntityPower entityPower)
        {
            bool isHero = IsHeroEntityPower(entityPower);
            bool hasEntityPowerViewOnSameObject = entityPower.GetComponent<EntityPowerView>() != null;

            if (isHero)
            {
                if (entityPower.GetComponent<MovementView>() != null)
                {
                    throw new InvalidEntityPowerProviderException(
                        entityPower.gameObject.name,
                        "Hero entity power must be on player, not on character tier"
                    );
                }

                if (hasEntityPowerViewOnSameObject)
                {
                    throw new InvalidEntityPowerProviderException(
                        entityPower.gameObject.name,
                        "Hero entity power must not share a game object with entity power view"
                    );
                }

                return;
            }

            if (!hasEntityPowerViewOnSameObject)
            {
                throw new InvalidEntityPowerProviderException(
                    entityPower.gameObject.name,
                    "Interactable entity power must have entity power view on the same game object"
                );
            }
        }
    }
}
