using UnityEngine;

namespace ViewComponents.Presentation.Player
{
    public sealed class ActiveCharacterPresentationProvider : IActiveCharacterPresentationProvider
    {
        private ActiveCharacterAnchorView _activeCharacterAnchorView;

        public Transform ActivePowerPanelTransform => ResolveAnchorView().ActivePowerPanelTransform;

        public Transform GetAnchor(string anchorKey)
        {
            return ResolveAnchorView().GetAnchor(anchorKey);
        }

        public void Register(ActiveCharacterAnchorView activeCharacterAnchorView)
        {
            _activeCharacterAnchorView = activeCharacterAnchorView
             ?? throw new InvalidActiveCharacterAnchorViewException(string.Empty, "Active character anchor view is not assigned");
        }

        private ActiveCharacterAnchorView ResolveAnchorView()
        {
            return _activeCharacterAnchorView ?? throw new ActiveCharacterPresentationNotRegisteredException();
        }
    }
}
