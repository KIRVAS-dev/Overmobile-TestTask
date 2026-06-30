using Core.Gameplay.Power;
using System;
using UnityEngine;
using ViewComponents.UI.PowerPanel;

namespace ViewComponents.Power
{
    [DisallowMultipleComponent]
    public sealed class EntityPowerView : MonoBehaviour
    {
        private PowerPanelView _powerPanelView;
        private IDisposable _binding;

        public void BindPowerPanel(IPowerRegistry powerRegistry, string entityId)
        {
            _binding?.Dispose();
            _binding = new PowerPanelBinding(powerRegistry, entityId, ResolvePowerPanelView());
        }

        private PowerPanelView ResolvePowerPanelView()
        {
            if (_powerPanelView != null)
            {
                return _powerPanelView;
            }

            PowerPanelView[] powerPanelViews = GetComponentsInChildren<PowerPanelView>(includeInactive: true);

            switch (powerPanelViews.Length)
            {
                case 0:
                    throw new MissingPowerPanelViewException(gameObject.name);

                case > 1:
                    throw new MultiplePowerPanelViewException(gameObject.name);

                default:
                    _powerPanelView = powerPanelViews[0];

                    return _powerPanelView;
            }
        }

        private void OnDestroy()
        {
            _binding?.Dispose();
        }
    }
}
