using DG.Tweening;
using ExtendedExceptions;
using UnityEngine;

namespace UI.TapIndicator
{
    [CreateAssetMenu(fileName = "TapIndicatorConfig", menuName = "Project/Configs/UI/Tap Indicator Config")]
    public sealed class TapIndicatorConfig : ScriptableObject
    {
        public Sprite Sprite => _sprite;
        public float ScaleInDuration => _scaleInDuration;
        public float ScaleOutDuration => _scaleOutDuration;
        public Ease FadeInEase => _fadeInEase;
        public Ease FadeOutEase => _fadeOutEase;
        public float TargetClickScaleMultiplier => _targetClickScaleMultiplier;
        public float TargetClickScaleUpDuration => _targetClickScaleUpDuration;
        public Ease TargetClickScaleUpEase => _targetClickScaleUpEase;
        public float TargetClickScaleHoldDuration => _targetClickScaleHoldDuration;

        [SerializeField] private Sprite _sprite;

        [Header("Scale In")]
        [SerializeField] private float _scaleInDuration = 0.15f;
        [SerializeField] private Ease _fadeInEase = Ease.InQuad;

        [Header("Scale Out")]
        [SerializeField] private float _scaleOutDuration = 0.1f;
        [SerializeField] private Ease _fadeOutEase = Ease.InQuad;

        [Header("Target Click Release")]
        [SerializeField] private float _targetClickScaleMultiplier = 1.15f;
        [SerializeField] private float _targetClickScaleUpDuration = 0.08f;
        [SerializeField] private Ease _targetClickScaleUpEase = Ease.OutQuad;
        [SerializeField] private float _targetClickScaleHoldDuration = 0.05f;

        public void Validate()
        {
            Guard.AgainstNull(_sprite, () => new InvalidTapIndicatorSpriteException());

            Guard.AgainstNegative(
                _scaleInDuration,
                () => new InvalidTapIndicatorScaleDurationException("scale in duration", _scaleInDuration)
            );

            Guard.AgainstNegative(
                _scaleOutDuration,
                () => new InvalidTapIndicatorScaleDurationException("scale out duration", _scaleOutDuration)
            );

            Guard.AgainstLessThan(
                _targetClickScaleMultiplier,
                minimum: 1f,
                () => new InvalidTapIndicatorValueException(nameof(_targetClickScaleMultiplier), _targetClickScaleMultiplier)
            );

            Guard.AgainstNegative(
                _targetClickScaleUpDuration,
                () => new InvalidTapIndicatorScaleDurationException("target click scale up duration", _targetClickScaleUpDuration)
            );

            Guard.AgainstNegative(
                _targetClickScaleHoldDuration,
                () => new InvalidTapIndicatorScaleDurationException(
                    "target click scale hold duration",
                    _targetClickScaleHoldDuration
                )
            );
        }
    }
}
