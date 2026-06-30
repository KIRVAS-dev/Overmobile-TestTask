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
        private readonly PowerRegistry _powerRegistry;

        public InteractionService(
            IInventory inventory,
            IMovementService movementService,
            IInteractableTargetProvider interactableTargetProvider,
            PowerRegistry powerRegistry)
        {
            _inventory = inventory;
            _movementService = movementService;
            _interactableTargetProvider = interactableTargetProvider;
            _powerRegistry = powerRegistry;

            ValidateEntityGuards();
        }

        public bool CanInteract(string entityId)
        {
            InteractableTargetData targetData = _interactableTargetProvider.GetTargetByEntityId(entityId);

            if (targetData.GuardEntityIds.Count == 0)
            {
                return true;
            }

            foreach (string guardEntityId in targetData.GuardEntityIds)
            {
                if (_powerRegistry.IsResolved(guardEntityId) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public async UniTask InteractAsync(string endpointKey, string entityId, Vector3 facingWorldPosition,
            CancellationToken cancellationToken)
        {
            if (CanInteract(entityId) == false)
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
            if (_powerRegistry.IsResolved(entityId))
            {
                return;
            }

            InteractableTargetData targetData = _interactableTargetProvider.GetTargetByEntityId(entityId);

            switch (targetData.Type)
            {
                case EntityType.Enemy:
                    ResolveEnemyInteraction(entityId, targetData.RequiredItem);
                    break;

                case EntityType.Ally:
                    _powerRegistry.TransferPowerToPlayer(entityId, requirePlayerPowerGreater: false);
                    break;

                case EntityType.Player:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(targetData.Type), targetData.Type, message: null);
            }
        }

        private void ResolveEnemyInteraction(string entityId, ItemType? requiredItem)
        {
            if (requiredItem.HasValue
             && !_inventory.Get(requiredItem.Value))
            {
                return;
            }

            _powerRegistry.TransferPowerToPlayer(entityId, requirePlayerPowerGreater: true);
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
