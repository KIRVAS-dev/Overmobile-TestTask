using DG.Tweening;
using UnityEngine;

namespace ViewComponents.WaterWaves
{
    [CreateAssetMenu(fileName = "WaterWavesInstanceConfig", menuName = "Project/Configs/Water Waves Instance Config")]
    public sealed class WaterWavesInstanceConfig : ScriptableObject
    {
        public Sprite[] Sprites => _sprites;
        public float FadeInDuration => _fadeInDuration;
        public float FadeOutDuration => _fadeOutDuration;
        public Ease MoveEase => _moveEase;
        public Ease FadeInEase => _fadeInEase;
        public Ease FadeOutEase => _fadeOutEase;

        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private Ease _moveEase = Ease.InOutQuad;

        [Header("Fade In")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private Ease _fadeInEase = Ease.InOutQuad;

        [Header("Fade Out")]
        [SerializeField] private float _fadeOutDuration = 0.3f;
        [SerializeField] private Ease _fadeOutEase = Ease.InOutQuad;

        public void Validate()
        {
            if (_sprites == null
             || _sprites.Length == 0)
            {
                throw new InvalidWaterWavesSpritesException();
            }

            foreach (Sprite sprite in _sprites)
            {
                if (sprite == null)
                {
                    throw new InvalidWaterWavesSpritesException();
                }
            }

            if (_fadeInDuration < 0f)
            {
                throw new InvalidWaterWavesFadeDurationException("fade in duration", _fadeInDuration);
            }

            if (_fadeOutDuration < 0f)
            {
                throw new InvalidWaterWavesFadeDurationException("fade out duration", _fadeOutDuration);
            }
        }
    }
}
