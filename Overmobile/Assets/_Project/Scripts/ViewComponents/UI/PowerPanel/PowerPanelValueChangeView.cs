using DG.Tweening;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [DisallowMultipleComponent]
    public sealed class PowerPanelValueChangeView : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelScaleTarget;
        [SerializeField] private Transform _textScaleTarget;
        [SerializeField] private PowerPanelChangeConfig _changeConfig;

        private Sequence _sequence;

        public void PlayValueChangeAnimation()
        {
            PowerPanelScaleChangeSettings panelScaleChange = _changeConfig.PanelScaleChange;
            PowerPanelScaleChangeSettings textScaleChange = _changeConfig.TextScaleChange;

            CancelAnimation();

            _panelScaleTarget.localScale = panelScaleChange.FromScale;
            _textScaleTarget.localScale = textScaleChange.FromScale;

            _sequence = DOTween.Sequence();

            _sequence.Join(
                _panelScaleTarget
                   .DOScale(panelScaleChange.ToScale, panelScaleChange.DurationSeconds)
                   .SetEase(panelScaleChange.Ease)
            );

            _sequence.Join(
                _textScaleTarget.DOScale(textScaleChange.ToScale, textScaleChange.DurationSeconds).SetEase(textScaleChange.Ease)
            );

            if (_changeConfig.ResetToDefaultScaleOnComplete)
            {
                _sequence.OnComplete(ResetDefaultScales);
            }
        }

        private void Awake()
        {
            Validate();
        }

        private void OnDestroy()
        {
            CancelAnimation();
        }

        private void ResetDefaultScales()
        {
            _panelScaleTarget.localScale = Vector3.one;
            _textScaleTarget.localScale = Vector3.one;
        }

        private void CancelAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
            _panelScaleTarget.DOKill();
            _textScaleTarget.DOKill();
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _panelScaleTarget,
                () => new MissingPowerPanelFieldException(nameof(_panelScaleTarget), gameObject.name)
            );

            Guard.AgainstNull(
                _textScaleTarget,
                () => new MissingPowerPanelFieldException(nameof(_textScaleTarget), gameObject.name)
            );

            Guard.AgainstNull(_changeConfig, () => new MissingPowerPanelFieldException(nameof(_changeConfig), gameObject.name));

            Guard.AgainstNonPositive(
                _changeConfig.PanelScaleChange.DurationSeconds,
                () => new InvalidPowerPanelValueException(
                    "PanelScaleChange.DurationSeconds",
                    gameObject.name,
                    _changeConfig.PanelScaleChange.DurationSeconds
                )
            );

            Guard.AgainstNonPositive(
                _changeConfig.TextScaleChange.DurationSeconds,
                () => new InvalidPowerPanelValueException(
                    "TextScaleChange.DurationSeconds",
                    gameObject.name,
                    _changeConfig.TextScaleChange.DurationSeconds
                )
            );
        }
    }
}
