using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [CreateAssetMenu(fileName = "PowerPanelVisibilityConfig", menuName = "Project/Configs/UI/Power Panel Visibility Config")]
    public sealed class PowerPanelVisibilityConfig : ScriptableObject
    {
        [Header("Show")]
        [SerializeField] private PowerPanelVisibilityTransition _enableTransition = new PowerPanelVisibilityTransition();

        [Header("Hide")]
        [SerializeField] private PowerPanelVisibilityTransition _disableTransition = new PowerPanelVisibilityTransition();

        public PowerPanelVisibilityTransition EnableTransition => _enableTransition;
        public PowerPanelVisibilityTransition DisableTransition => _disableTransition;
    }
}
