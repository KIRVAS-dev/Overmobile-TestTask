using Core.Gameplay.Power;
using ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ViewComponents.UI.PowerPanel
{
    public sealed class PowerPanelView
        : MonoBehaviour,
          IPowerPanelView
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _textLabel;
        [SerializeField] private PowerPanelConfig _config;

        private void Awake()
        {
            Validate();
            ApplyConfig();
        }

        public void SetPower(int power)
        {
            _textLabel.text = power.ToString();
        }

        private void ApplyConfig()
        {
            _backgroundImage.color = _config.BackgroundColor;
            _textLabel.color = _config.TextColor;
            _textLabel.outlineWidth = _config.TextOutlineWidth;
        }

        private void Validate()
        {
            _config.Validate();

            Guard.AgainstNull(
                _backgroundImage,
                () => new MissingPowerPanelFieldException(nameof(_backgroundImage), gameObject.name)
            );

            Guard.AgainstNull(_textLabel, () => new MissingPowerPanelFieldException(nameof(_textLabel), gameObject.name));
            Guard.AgainstNull(_config, () => new MissingPowerPanelFieldException(nameof(_config), gameObject.name));
        }
    }
}
