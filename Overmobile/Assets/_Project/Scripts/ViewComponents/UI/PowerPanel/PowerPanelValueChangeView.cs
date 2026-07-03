using DG.Tweening;
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

            if (panelScaleChange.DurationSeconds <= 0f)
            {
                throw new InvalidPanelScaleChangeDurationException(gameObject.name, panelScaleChange.DurationSeconds);
            }

            if (textScaleChange.DurationSeconds <= 0f)
            {
                throw new InvalidTextScaleChangeDurationException(gameObject.name, textScaleChange.DurationSeconds);
            }

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
            if (_panelScaleTarget == null)
            {
                throw new MissingPowerPanelScaleTargetException(gameObject.name);
            }

            if (_textScaleTarget == null)
            {
                throw new MissingTextScaleTargetException(gameObject.name);
            }

            if (_changeConfig == null)
            {
                throw new MissingPowerPanelChangeConfigException(gameObject.name);
            }
        }
    }
}
