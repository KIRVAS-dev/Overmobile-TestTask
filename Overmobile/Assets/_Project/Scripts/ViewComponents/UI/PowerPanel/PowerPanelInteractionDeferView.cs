using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using System;
using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [DisallowMultipleComponent]
    public sealed class PowerPanelInteractionDeferView : MonoBehaviour
    {
        [SerializeField] private PowerPanelView _powerPanelView;

        private IDisposable _binding;

        public void Bind(
            IInteractionPhaseSource interactionPhaseSource,
            IPowerRegistry powerRegistry,
            string entityId,
            PowerPanelValueChangeView valueChangeView)
        {
            _binding?.Dispose();
            Validate();

            _binding = new InteractionDeferPowerPanelBinding(
                powerRegistry,
                entityId,
                _powerPanelView,
                interactionPhaseSource,
                valueChangeView
            );
        }

        private void Awake()
        {
            Validate();
        }

        private void OnDestroy()
        {
            _binding?.Dispose();
        }

        private void Validate()
        {
            if (_powerPanelView == null)
            {
                throw new InvalidPowerPanelInteractionDeferViewException(gameObject.name, "Power panel view is not assigned");
            }
        }
    }
}
