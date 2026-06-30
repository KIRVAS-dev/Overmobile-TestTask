using UnityEngine;
using ViewComponents.Presentation.Player;

namespace ViewComponents.Presentation
{
    public interface IActiveCharacterPresentationProvider
    {
        Transform ActivePowerPanelTransform { get; }
        Transform GetAnchor(string anchorKey);
        void Register(ActiveCharacterAnchorView activeCharacterAnchorView);
    }
}
