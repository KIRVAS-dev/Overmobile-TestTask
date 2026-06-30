using Core.Gameplay.Interaction;
using Core.Gameplay.Inventory;
using Core.Gameplay.Movement;
using Core.Input;
using Input;
using System.Collections.Generic;
using UnityEngine;

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

        public InteractableTargetData GetTargetByPowerId(string powerId)
        {
            ValidateInteractableTargets();

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                if (interactableTarget.PowerId == powerId)
                {
                    return BuildInteractableTargetData(interactableTarget);
                }
            }

            throw new InteractableTargetNotFoundByPowerIdException(powerId);
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

                inputTargets.Add(new GameplayInputTarget(movementInputTarget, interactableTarget.PowerId));
            }

            return inputTargets;
        }

        private static InteractableTargetData BuildInteractableTargetData(InteractableTarget interactableTarget)
        {
            InteractionRequiredItem requiredItemComponent = interactableTarget.GetComponent<InteractionRequiredItem>();

            ItemType? requiredItem = requiredItemComponent == null
                ? null
                : requiredItemComponent.RequiredItem;

            return new InteractableTargetData(
                interactableTarget.EntityKey,
                interactableTarget.PowerId,
                interactableTarget.AnimationView,
                interactableTarget.Type,
                requiredItem
            );
        }

        private static MovementInputTarget BuildMovementInputTarget(
            string endpointKey,
            PointArea pointArea,
            Vector3 facingWorldPosition)
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
