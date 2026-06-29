using DG.Tweening;
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

        [SerializeField] private Sprite _sprite;

        [Header("Scale In")]
        [SerializeField] private float _scaleInDuration = 0.15f;
        [SerializeField] private Ease _fadeInEase = Ease.InQuad;

        [Header("Scale Out")]
        [SerializeField] private float _scaleOutDuration = 0.1f;
        [SerializeField] private Ease _fadeOutEase = Ease.InQuad;

        public void Validate()
        {
            if (_sprite == null)
            {
                throw new InvalidTapIndicatorSpriteException();
            }

            if (_scaleInDuration < 0f)
            {
                throw new InvalidTapIndicatorScaleDurationException("scale in duration", _scaleInDuration);
            }

            if (_scaleOutDuration < 0f)
            {
                throw new InvalidTapIndicatorScaleDurationException("scale out duration", _scaleOutDuration);
            }
        }
    }
}
