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
        private readonly IInteractionPipeline _interactionPipeline;

        public InteractionService(
            IInventory inventory,
            IMovementService movementService,
            IInteractableTargetProvider interactableTargetProvider,
            IPowerRegistry powerRegistry,
            IPowerService powerService,
            IEntityGuardAccessRegistry guardAccessRegistry,
            IInteractionPipeline interactionPipeline)
        {
            _inventory = inventory;
            _movementService = movementService;
            _interactableTargetProvider = interactableTargetProvider;
            _powerRegistry = powerRegistry;
            _powerService = powerService;
            _guardAccessRegistry = guardAccessRegistry;
            _interactionPipeline = interactionPipeline;

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

        public async UniTask InteractAsync(
            string endpointKey,
            string entityId,
            Vector3 facingWorldPosition,
            CancellationToken cancellationToken)
        {
            if (!CanInteract(entityId))
            {
                return;
            }

            if (_interactionPipeline.CurrentPhase.CurrentValue != InteractionPhase.Idle)
            {
                return;
            }

            _interactionPipeline.BeginInteraction(entityId);

            try
            {
                await RunApproachPhaseAsync(endpointKey, facingWorldPosition, cancellationToken);

                _interactionPipeline.SetPhase(InteractionPhase.Resolve);

                bool hasPresentation = _interactionPipeline.HasTargetPresentation(entityId);

                UniTask presentationWait = hasPresentation
                    ? _interactionPipeline.AwaitTargetPresentationAsync(cancellationToken)
                    : UniTask.CompletedTask;

                bool resolveResult = TryResolve(entityId);

                if (!resolveResult)
                {
                    return;
                }

                if (hasPresentation)
                {
                    _interactionPipeline.SetPhase(InteractionPhase.TargetPresentation);
                    await presentationWait;
                }
            }
            finally
            {
                _interactionPipeline.EndInteraction();
            }
        }

        private async UniTask RunApproachPhaseAsync(
            string endpointKey,
            Vector3 facingWorldPosition,
            CancellationToken cancellationToken)
        {
            await _movementService.MoveToAsync(endpointKey, facingWorldPosition, cancellationToken);
        }

        private bool TryResolve(string entityId)
        {
            if (_powerRegistry.Get(entityId).IsResolved.CurrentValue)
            {
                return false;
            }

            InteractableTargetData targetData = _interactableTargetProvider.GetTargetByEntityId(entityId);

            switch (targetData.Type)
            {
                case EntityType.Enemy:
                    return TryResolveEnemyInteraction(entityId, targetData);

                case EntityType.Ally:
                    return TryResolveAllyInteraction(entityId, targetData);

                case EntityType.Player:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException(nameof(targetData.Type), targetData.Type, message: null);
            }
        }

        private bool TryResolveEnemyInteraction(string entityId, InteractableTargetData targetData)
        {
            if (targetData.RequiredItem.HasValue
             && !_inventory.Get(targetData.RequiredItem.Value))
            {
                return false;
            }

            if (!_powerService.TryTransferPowerToPlayer(entityId, requirePlayerPowerGreater: true))
            {
                return false;
            }

            GrantLoot(targetData);

            return true;
        }

        private bool TryResolveAllyInteraction(string entityId, InteractableTargetData targetData)
        {
            bool transferResult = _powerService.TryTransferPowerToPlayer(entityId, requirePlayerPowerGreater: false);

            if (!transferResult)
            {
                return false;
            }

            GrantLoot(targetData);

            return true;
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
