using Core;
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

        public void Bind(IPowerRegistry powerRegistry, string entityId, IGameplayInputBlock gameplayInputBlock)
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
                _presentationSequence
            );
        }

        private void OnDestroy()
        {
            _binding?.Dispose();
        }
    }
}
