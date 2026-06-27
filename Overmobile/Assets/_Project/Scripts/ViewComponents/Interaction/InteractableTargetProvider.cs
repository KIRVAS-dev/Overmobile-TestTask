using Core.Input.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Interaction
{
    public sealed class InteractableTargetProvider
        : MonoBehaviour,
          IMovementInputTargetProvider
    {
        [SerializeField] private InteractableTarget[] _interactableTargets;

        public IReadOnlyList<MovementInputTarget> GetInputTargets()
        {
            ValidateInteractableTargets();

            List<MovementInputTarget> inputTargets = new List<MovementInputTarget>(_interactableTargets.Length);

            foreach (InteractableTarget interactableTarget in _interactableTargets)
            {
                inputTargets.Add(new MovementInputTarget(
                    interactableTarget.EndpointKey,
                    interactableTarget.PointArea.PointerUp,
                    interactableTarget.transform.position));
            }

            return inputTargets;
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
