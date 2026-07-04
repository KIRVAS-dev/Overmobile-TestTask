using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using ExtendedExceptions;
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
            PowerPanelValueChangeView valueChangeView,
            IPlayerPowerDisplayState playerPowerDisplayState)
        {
            _binding?.Dispose();
            Validate();

            _binding = new InteractionDeferPowerPanelBinding(
                powerRegistry,
                entityId,
                _powerPanelView,
                interactionPhaseSource,
                valueChangeView,
                playerPowerDisplayState
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
            Guard.AgainstNull(
                _powerPanelView,
                () => new MissingPowerPanelFieldException(nameof(_powerPanelView), gameObject.name)
            );
        }
    }
}
