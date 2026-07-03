using Core.Gameplay.Power;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ViewComponents.UI.PowerPanel
{
    [ExecuteAlways]
    public sealed class PowerPanelView
        : MonoBehaviour,
          IPowerPanelView
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

        public void SetPower(int power)
        {
            _textLabel.text = power.ToString();
        }

        private void ApplyConfig(bool isConfigRequired)
        {
            if (_config == null)
            {
                if (isConfigRequired)
                {
                    throw new MissingPowerPanelFieldException(nameof(_config), gameObject.name);
                }

                return;
            }

            _backgroundImage.color = _config.BackgroundColor;
            _textLabel.color = _config.TextColor;
        }
    }
}
