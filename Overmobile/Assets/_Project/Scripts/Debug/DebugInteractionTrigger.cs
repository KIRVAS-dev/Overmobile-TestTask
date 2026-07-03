#if UNITY_EDITOR
using Core.Bootstrap;
using Core.Gameplay.Entity;
using Core.Gameplay.Interaction;
using Core.Gameplay.Inventory;
using Core.Gameplay.Power;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using ViewComponents.Interaction;

namespace ProjectDebug
{
    public sealed class DebugInteractionTrigger : MonoBehaviour
    {
        [SerializeField] private Key _hotkey = Key.I;
        [SerializeField] private bool _triggerAllInteractablesOnScene;
        [SerializeField] private InteractableTarget[] _interactableTargets = Array.Empty<InteractableTarget>();

        private InteractableTargetProvider _interactableTargetProvider;
        private IPowerRegistry _powerRegistry;
        private IPowerService _powerService;
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

            ResolveInteractables();
        }

        private bool TryEnsureRuntimeServices()
        {
            if (_powerRegistry != null
             && _powerService != null
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
            _powerRegistry = container.Resolve<IPowerRegistry>();
            _powerService = container.Resolve<IPowerService>();
            _inventory = container.Resolve<IInventory>();

            return true;
        }

        private void ResolveInteractables()
        {
            foreach (InteractableTargetData targetData in GetTargetDataCollection())
            {
                if (_powerRegistry.Get(targetData.EntityId).IsResolved.CurrentValue)
                {
                    continue;
                }

                ActivateTargetAsHero(targetData);
            }
        }

        private IEnumerable<InteractableTargetData> GetTargetDataCollection()
        {
            if (_triggerAllInteractablesOnScene)
            {
                return BuildTargetDataCollection(FindObjectsByType<InteractableTarget>(FindObjectsInactive.Include));
            }

            if (_interactableTargets.Length > 0)
            {
                return BuildTargetDataCollection(_interactableTargets);
            }

            return _interactableTargetProvider.GetInteractableTargets();
        }

        private IEnumerable<InteractableTargetData> BuildTargetDataCollection(InteractableTarget[] interactableTargets)
        {
            List<InteractableTargetData> targetDataCollection = new List<InteractableTargetData>(interactableTargets.Length);

            foreach (InteractableTarget interactableTarget in interactableTargets)
            {
                if (interactableTarget == null)
                {
                    throw new InvalidOperationException($"Interactable target reference is missing on '{gameObject.name}'");
                }

                targetDataCollection.Add(_interactableTargetProvider.BuildInteractableTargetData(interactableTarget));
            }

            return targetDataCollection;
        }

        private void ActivateTargetAsHero(InteractableTargetData targetData)
        {
            switch (targetData.Type)
            {
                case EntityType.Enemy:
                case EntityType.Ally:
                    _powerService.TryTransferPowerToPlayer(targetData.EntityId, requirePlayerPowerGreater: false);
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
