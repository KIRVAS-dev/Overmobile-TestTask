using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [CreateAssetMenu(fileName = "PowerPanelConfig", menuName = "Project/Configs/UI/Power Panel Config")]
    public class PowerPanelConfig : ScriptableObject
    {
        public Color BackgroundColor => _backgroundColor;
        public Color TextColor => _textColor;
        public float TextOutlineWidth => _textOutlineWidth;

        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;

        [Range(0f, 1f)]
        [SerializeField] private float _textOutlineWidth = 0.4f;

        public void Validate()
        {
            Guard.AgainstNegative(
                _textOutlineWidth,
                () => new InvalidPowerPanelValueException(nameof(_textOutlineWidth), _textOutlineWidth)
            );
        }
    }
}
