using Core.Gameplay.Interaction;
using Core.Gameplay.Movement;
using Input;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Interaction
{
    public sealed class InteractableTargetProvider
        : MonoBehaviour,
          IInteractableTargetProvider,
          IMovementInputTargetProvider
    {
        [SerializeField] private InteractableTarget[] _interactableTargets;

        public IReadOnlyList<InteractableTargetData> GetInteractableTargets()
        {
            ValidateInteractableTargets();

            List<InteractableTargetData> interactableTargets = new List<InteractableTargetData>(_interactableTargets.Length);

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                interactableTargets.Add(new InteractableTargetData(
                    interactableTarget.EntityKey,
                    interactableTarget.AnimationView
                ));
            }

            return interactableTargets;
        }

        public IReadOnlyList<MovementInputTarget> GetInputTargets()
        {
            ValidateInteractableTargets();

            List<MovementInputTarget> inputTargets = new List<MovementInputTarget>(_interactableTargets.Length);

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                inputTargets.Add(BuildMovementInputTarget(
                    interactableTarget.EntityKey,
                    interactableTarget.PointArea,
                    interactableTarget.transform.position
                ));
            }

            return inputTargets;
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
