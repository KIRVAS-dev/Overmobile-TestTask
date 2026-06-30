#if UNITY_EDITOR
using Core.Bootstrap;
using Core.Gameplay.Entity;
using Core.Gameplay.Interaction;
using Core.Gameplay.Inventory;
using Core.Gameplay.Power;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using ViewComponents.Interaction;

namespace ProjectDebug
{
    public sealed class DebugInteractAllTrigger : MonoBehaviour
    {
        [SerializeField] private Key _hotkey = Key.I;

        private InteractableTargetProvider _interactableTargetProvider;
        private PowerRegistry _powerRegistry;
        private IInventory _inventory;

        private void Awake()
        {
            _interactableTargetProvider = FindAnyObjectByType<InteractableTargetProvider>(FindObjectsInactive.Exclude);
        }

        private void Update()
        {
            if (!DebugHotkey.WasPressedThisFrame(_hotkey))
            {
                return;
            }

            if (!TryEnsureRuntimeServices())
            {
                return;
            }

            if (_interactableTargetProvider == null)
            {
                _interactableTargetProvider = FindAnyObjectByType<InteractableTargetProvider>(FindObjectsInactive.Exclude);
            }

            if (_interactableTargetProvider == null)
            {
                return;
            }

            ResolveAllInteractables();
        }

        private bool TryEnsureRuntimeServices()
        {
            if (_powerRegistry != null
             && _inventory != null)
            {
                return true;
            }

            CoreScope coreScope = FindAnyObjectByType<CoreScope>(FindObjectsInactive.Exclude);

            if (coreScope == null)
            {
                return false;
            }

            IObjectResolver container = coreScope.Container;
            _powerRegistry = container.Resolve<PowerRegistry>();
            _inventory = container.Resolve<IInventory>();

            return true;
        }

        private void ResolveAllInteractables()
        {
            foreach (InteractableTargetData targetData in _interactableTargetProvider.GetInteractableTargets())
            {
                if (_powerRegistry.IsResolved(targetData.EntityId))
                {
                    continue;
                }

                ActivateTargetAsHero(targetData);
            }
        }

        private void ActivateTargetAsHero(InteractableTargetData targetData)
        {
            switch (targetData.Type)
            {
                case EntityType.Enemy:
                case EntityType.Ally:
                    _powerRegistry.TryTransferPowerToPlayer(targetData.EntityId, requirePlayerPowerGreater: false);
                    GrantLoot(targetData);
                    return;

                case EntityType.Player:
                    return;

                default:
                    throw new ArgumentOutOfRangeException(nameof(targetData.Type), targetData.Type, message: null);
            }
        }

        private void GrantLoot(InteractableTargetData targetData)
        {
            if (!targetData.LootItem.HasValue)
            {
                return;
            }

            ItemType lootItem = targetData.LootItem.Value;

            if (_inventory.Get(lootItem))
            {
                return;
            }

            _inventory.Add(lootItem);
        }
    }
}
#endif
