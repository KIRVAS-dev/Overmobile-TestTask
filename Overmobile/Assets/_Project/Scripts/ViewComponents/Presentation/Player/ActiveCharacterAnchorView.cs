using System;
using UnityEngine;
using ViewComponents.UI.PowerPanel;

namespace ViewComponents.Presentation.Player
{
    [DisallowMultipleComponent]
    public sealed class ActiveCharacterAnchorView : MonoBehaviour
    {
        [Serializable]
        private struct AnchorEntry
        {
            public string Key;
            public Transform Anchor;
        }

        [SerializeField] private PowerPanelView _powerPanelView;
        [SerializeField] private AnchorEntry[] _anchors = Array.Empty<AnchorEntry>();

        public Transform ActivePowerPanelTransform => ResolvePowerPanelTransform();

        public Transform GetAnchor(string anchorKey)
        {
            if (string.IsNullOrWhiteSpace(anchorKey))
            {
                throw new InvalidActiveCharacterAnchorViewException(gameObject.name, "Anchor key is not assigned");
            }

            foreach (AnchorEntry anchorEntry in _anchors)
            {
                if (anchorEntry.Key != anchorKey)
                {
                    continue;
                }

                if (anchorEntry.Anchor == null)
                {
                    throw new InvalidActiveCharacterAnchorViewException(
                        gameObject.name,
                        $"Anchor '{anchorKey}' transform is not assigned"
                    );
                }

                return anchorEntry.Anchor;
            }

            throw new ActiveCharacterAnchorNotFoundException(anchorKey);
        }

        private Transform ResolvePowerPanelTransform()
        {
            if (_powerPanelView != null)
            {
                return _powerPanelView.transform;
            }

            PowerPanelView[] powerPanelViews = GetComponentsInChildren<PowerPanelView>(includeInactive: true);

            switch (powerPanelViews.Length)
            {
                case 0:
                    throw new InvalidActiveCharacterAnchorViewException(gameObject.name, "Power panel view is not assigned");

                case > 1:
                    throw new InvalidActiveCharacterAnchorViewException(
                        gameObject.name,
                        "Multiple power panel views are assigned"
                    );

                default:
                    return powerPanelViews[0].transform;
            }
        }
    }
}
