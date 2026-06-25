using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewComponents.PowerPanel.Api;

namespace ViewComponents.PowerPanel
{
    [ExecuteAlways]
    public class PowerPanelView : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _textLabel;
        [SerializeField] private PowerPanelConfig _config;

        private void OnValidate()
        {
            ApplyConfig(isConfigRequired: false);
        }

        private void Awake()
        {
            ApplyConfig(isConfigRequired: Application.isPlaying);
        }

        private void ApplyConfig(bool isConfigRequired)
        {
            if (_config == null)
            {
                if (isConfigRequired)
                {
                    throw new MissingPowerPanelConfigException(gameObject.name);
                }

                return;
            }

            _backgroundImage.color = _config.BackgroundColor;
            _textLabel.color = _config.TextColor;
        }
    }
}
