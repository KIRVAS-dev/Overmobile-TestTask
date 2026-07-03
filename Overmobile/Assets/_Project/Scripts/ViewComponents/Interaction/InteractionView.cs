using Core;
using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using System;
using UnityEngine;
using ViewComponents.Presentation;

namespace ViewComponents.Interaction
{
    public sealed class InteractionView : MonoBehaviour
    {
        [SerializeField] private PresentationStepSequence _presentationSequence;

        private IDisposable _binding;

        public void Bind(
            IPowerRegistry powerRegistry,
            string entityId,
            IGameplayInputBlock gameplayInputBlock,
            IInteractionTargetPresentation interactionTargetPresentation)
        {
            _binding?.Dispose();

            if (_presentationSequence == null)
            {
                throw new InvalidInteractionViewException(gameObject.name, "Presentation sequence is not assigned");
            }

            _binding = new InteractionViewBinding(
                powerRegistry,
                entityId,
                gameplayInputBlock,
                interactionTargetPresentation,
                _presentationSequence
            );
        }

        private void OnDestroy()
        {
            _binding?.Dispose();
        }
    }
}
