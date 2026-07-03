using Core.Gameplay.Entity;
using Core.Gameplay.Inventory;
using Core.Gameplay.Movement;
using Core.Gameplay.Power;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Core.Gameplay.Interaction
{
    public sealed class InteractionService : IInteractionService
    {
        private readonly IInventory _inventory;
        private readonly IMovementService _movementService;
        private readonly IInteractableTargetProvider _interactableTargetProvider;
        private readonly IEntityGuardAccessRegistry _guardAccessRegistry;
        private readonly IPowerRegistry _powerRegistry;
        private readonly IPowerService _powerService;

        public InteractionService(
            IInventory inventory,
            IMovementService movementService,
            IInteractableTargetProvider interactableTargetProvider,
            IPowerRegistry powerRegistry,
            IPowerService powerService,
            IEntityGuardAccessRegistry guardAccessRegistry)
        {
            _inventory = inventory;
            _movementService = movementService;
            _interactableTargetProvider = interactableTargetProvider;
            _powerRegistry = powerRegistry;
            _powerService = powerService;
            _guardAccessRegistry = guardAccessRegistry;

            ValidateEntityGuards();
        }

        public bool CanInteract(string entityId)
        {
            if (_powerRegistry.Get(entityId).IsResolved.CurrentValue)
            {
                return false;
            }

            return !_guardAccessRegistry.GetAreGuardsBlocking(entityId).CurrentValue;
        }

        public async UniTask InteractAsync(string endpointKey, string entityId, Vector3 facingWorldPosition,
            CancellationToken cancellationToken)
        {
            if (!CanInteract(entityId))
            {
                return;
            }

            await RunApproachPhaseAsync(endpointKey, facingWorldPosition, cancellationToken);

            RunResolvePhase(entityId);
        }

        private async UniTask RunApproachPhaseAsync(string endpointKey, Vector3 facingWorldPosition,
            CancellationToken cancellationToken)
        {
            await _movementService.MoveToAsync(endpointKey, facingWorldPosition, cancellationToken);
        }

        private void RunResolvePhase(string entityId)
        {
            ResolveInteraction(entityId);
        }

        private void ResolveInteraction(string entityId)
        {
            if (_powerRegistry.Get(entityId).IsResolved.CurrentValue)
            {
                return;
            }

            InteractableTargetData targetData = _interactableTargetProvider.GetTargetByEntityId(entityId);

            switch (targetData.Type)
            {
                case EntityType.Enemy:
                    ResolveEnemyInteraction(entityId, targetData);
                    break;

                case EntityType.Ally:
                    ResolveAllyInteraction(entityId, targetData);
                    break;

                case EntityType.Player:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(targetData.Type), targetData.Type, message: null);
            }
        }

        private void ResolveEnemyInteraction(string entityId, InteractableTargetData targetData)
        {
            if (targetData.RequiredItem.HasValue
             && !_inventory.Get(targetData.RequiredItem.Value))
            {
                return;
            }

            if (!_powerService.TryTransferPowerToPlayer(entityId, requirePlayerPowerGreater: true))
            {
                return;
            }

            GrantLoot(targetData);
        }

        private void ResolveAllyInteraction(string entityId, InteractableTargetData targetData)
        {
            if (!_powerService.TryTransferPowerToPlayer(entityId, requirePlayerPowerGreater: false))
            {
                return;
            }

            GrantLoot(targetData);
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

        private void ValidateEntityGuards()
        {
            IReadOnlyList<InteractableTargetData> interactableTargets = _interactableTargetProvider.GetInteractableTargets();

            foreach (InteractableTargetData targetData in interactableTargets)
            {
                foreach (string guardEntityId in targetData.GuardEntityIds)
                {
                    _powerRegistry.Get(guardEntityId);
                }
            }
        }
    }
}
