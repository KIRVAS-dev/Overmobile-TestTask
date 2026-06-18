using UnityEngine;

namespace ViewComponents.PowerPanel
{
    [CreateAssetMenu(fileName = "PowerPanelConfig", menuName = "Project/Configs/UI/Power Panel Config")]
    public class PowerPanelConfig : ScriptableObject
    {
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private Color _textColor;

        public Color BackgroundColor => _backgroundColor;
        public Color TextColor => _textColor;
    }
}
