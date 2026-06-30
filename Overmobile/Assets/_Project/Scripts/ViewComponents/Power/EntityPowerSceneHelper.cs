using Core.Gameplay.Entity;
using System.Collections.Generic;
using UnityEngine;
using ViewComponents.Entity;
using ViewComponents.Movement;

namespace ViewComponents.Power
{
    public static class EntityPowerSceneHelper
    {
        public static EntityPower ResolvePlayerEntityPower(EntityPower[] serializedEntityPowers, string providerName)
        {
            EntityPower[] entityPowerComponents = GetValidatedEntityPowers(serializedEntityPowers, providerName);

            EntityPower playerEntityPower = null;

            foreach (EntityPower entityPower in entityPowerComponents)
            {
                if (!IsPlayerEntityPower(entityPower))
                {
                    continue;
                }

                if (playerEntityPower != null)
                {
                    throw new InvalidEntityPowerProviderException(providerName, "Multiple player entity powers are assigned");
                }

                playerEntityPower = entityPower;
            }

            return playerEntityPower == null
                ? throw new InvalidEntityPowerProviderException(providerName, "Player entity power is not assigned")
                : playerEntityPower;
        }

        public static EntityPower[] GetValidatedEntityPowers(EntityPower[] serializedEntityPowers, string providerName)
        {
            EntityPower[] entityPowerComponents = ResolveEntityPowerComponents(serializedEntityPowers);
            ValidateEntityPowers(entityPowerComponents, providerName);

            return entityPowerComponents;
        }

        public static IReadOnlyList<EntityPowerView> GetInteractableEntityPowerViews(EntityPower[] serializedEntityPowers,
            string providerName)
        {
            EntityPower[] entityPowerComponents = GetValidatedEntityPowers(serializedEntityPowers, providerName);
            List<EntityPowerView> entityPowerViews = new List<EntityPowerView>(entityPowerComponents.Length);

            foreach (EntityPower entityPower in entityPowerComponents)
            {
                if (IsPlayerEntityPower(entityPower))
                {
                    continue;
                }

                entityPowerViews.Add(entityPower.GetComponent<EntityPowerView>());
            }

            return entityPowerViews;
        }

        private static bool IsPlayerEntityPower(EntityPower entityPower)
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

            HashSet<string> entityIds = new HashSet<string>();

            for (int i = 0; i < entityPowerComponents.Length; i++)
            {
                EntityPower entityPower = entityPowerComponents[i];

                if (entityPower == null)
                {
                    throw new InvalidEntityPowerProviderException(providerName, $"Entity power at index {i} is missing");
                }

                string entityId = entityPower.EntityId;

                if (!entityIds.Add(entityId))
                {
                    throw new InvalidEntityPowerProviderException(providerName, $"Duplicate entity id '{entityId}'");
                }

                ValidateEntityPowerPlacement(entityPower);
            }
        }

        private static void ValidateEntityPowerPlacement(EntityPower entityPower)
        {
            bool isPlayer = IsPlayerEntityPower(entityPower);
            bool hasEntityPowerViewOnSameObject = entityPower.GetComponent<EntityPowerView>() != null;

            if (isPlayer)
            {
                if (entityPower.GetComponent<MovementView>() != null)
                {
                    throw new InvalidEntityPowerProviderException(
                        entityPower.gameObject.name,
                        "Player entity power must be on player, not on character tier"
                    );
                }

                if (hasEntityPowerViewOnSameObject)
                {
                    throw new InvalidEntityPowerProviderException(
                        entityPower.gameObject.name,
                        "Player entity power must not share a game object with entity power view"
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
