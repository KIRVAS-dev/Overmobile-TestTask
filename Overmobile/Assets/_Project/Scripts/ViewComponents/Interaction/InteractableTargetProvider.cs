using Core.Gameplay.Interaction;
using Core.Gameplay.Inventory;
using Core.Gameplay.Movement;
using Core.Gameplay.Power;
using Core.Input;
using Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using ViewComponents.Inventory;
using EntityId = ViewComponents.Entity.EntityId;

namespace ViewComponents.Interaction
{
    public sealed class InteractableTargetProvider
        : MonoBehaviour,
          IInteractableTargetProvider,
          IGameplayInputTargetProvider
    {
        [SerializeField] private InteractableTarget[] _interactableTargets;

        public IReadOnlyList<InteractableTargetData> GetInteractableTargets()
        {
            ValidateInteractableTargets();

            List<InteractableTargetData> interactableTargets = new List<InteractableTargetData>(_interactableTargets.Length);

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                interactableTargets.Add(BuildInteractableTargetData(interactableTarget));
            }

            return interactableTargets;
        }

        public InteractableTargetData GetTargetByEntityKey(string entityKey)
        {
            ValidateInteractableTargets();

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                if (interactableTarget.EntityKey == entityKey)
                {
                    return BuildInteractableTargetData(interactableTarget);
                }
            }

            throw new InteractableTargetNotFoundException(entityKey);
        }

        public InteractableTargetData GetTargetByEntityId(string entityId)
        {
            ValidateInteractableTargets();

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                if (interactableTarget.EntityId == entityId)
                {
                    return BuildInteractableTargetData(interactableTarget);
                }
            }

            throw new InteractableTargetNotFoundByEntityIdException(entityId);
        }

        public void BindLootDrops(IPowerRegistry powerRegistry)
        {
            ValidateInteractableTargets();

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                DropView dropView = interactableTarget.GetComponent<DropView>();

                if (dropView == null)
                {
                    continue;
                }

                dropView.Bind(powerRegistry, interactableTarget.EntityId);
            }
        }

        public IReadOnlyList<GameplayInputTarget> GetGameplayInputTargets()
        {
            ValidateInteractableTargets();

            List<GameplayInputTarget> inputTargets = new List<GameplayInputTarget>(_interactableTargets.Length);

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                MovementInputTarget movementInputTarget = BuildMovementInputTarget(
                    interactableTarget.EntityKey,
                    interactableTarget.PointArea,
                    interactableTarget.transform.position
                );

                inputTargets.Add(new GameplayInputTarget(movementInputTarget, interactableTarget.EntityId));
            }

            return inputTargets;
        }

        private InteractableTargetData BuildInteractableTargetData(InteractableTarget interactableTarget)
        {
            InteractionRequiredItem requiredItemComponent = interactableTarget.GetComponent<InteractionRequiredItem>();

            ItemType? requiredItem = requiredItemComponent == null
                ? null
                : requiredItemComponent.RequiredItem;

            ItemType? lootItem = ResolveLootItem(interactableTarget);

            IReadOnlyList<string> guardEntityIds = ResolveGuardEntityIds(interactableTarget);

            return new InteractableTargetData(
                interactableTarget.EntityKey,
                interactableTarget.EntityId,
                interactableTarget.AnimationView,
                interactableTarget.Type,
                requiredItem,
                lootItem,
                guardEntityIds
            );
        }

        private ItemType? ResolveLootItem(InteractableTarget interactableTarget)
        {
            Drop drop = interactableTarget.GetComponent<Drop>();

            if (drop == null)
            {
                return null;
            }

            Item lootItemPrefab = drop.LootItemPrefab;

            if (lootItemPrefab == null)
            {
                throw new InvalidDropException(interactableTarget.name, "Loot item prefab is not assigned");
            }

            ItemType lootItem = lootItemPrefab.ItemType;

            return !Enum.IsDefined(typeof(ItemType), lootItem)
                ? throw new InvalidDropException(interactableTarget.name, $"Loot item prefab has invalid item type '{lootItem}'")
                : lootItem;
        }

        private IReadOnlyList<string> ResolveGuardEntityIds(InteractableTarget interactableTarget)
        {
            InteractionEntityGuards entityGuards = interactableTarget.GetComponent<InteractionEntityGuards>();

            if (entityGuards == null)
            {
                return Array.Empty<string>();
            }

            string hostEntityId = interactableTarget.GetComponent<EntityId>().Id;

            return entityGuards.GetGuardEntityIds(hostEntityId);
        }

        private MovementInputTarget BuildMovementInputTarget(string endpointKey, PointArea pointArea, Vector3 facingWorldPosition)
        {
            return new MovementInputTarget(
                endpointKey,
                pointArea.PointerDown,
                pointArea.PointerExit,
                pointArea.PointerEnter,
                pointArea.PointerUp,
                facingWorldPosition
            );
        }

        private void ValidateInteractableTargets()
        {
            if (_interactableTargets == null
             || _interactableTargets.Length == 0)
            {
                throw new InvalidInteractableTargetProviderException(gameObject.name, "Interactable targets are not assigned");
            }

            for (int i = 0; i < _interactableTargets.Length; i++)
            {
                if (_interactableTargets[i] == null)
                {
                    throw new InvalidInteractableTargetProviderException(
                        gameObject.name,
                        $"Interactable target at index {i} is missing"
                    );
                }
            }
        }
    }
}
