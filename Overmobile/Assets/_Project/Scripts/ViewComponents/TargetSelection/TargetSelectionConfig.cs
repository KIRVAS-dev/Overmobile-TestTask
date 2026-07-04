using DG.Tweening;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.TargetSelection
{
    [CreateAssetMenu(fileName = "TargetSelectionConfig", menuName = "Project/Configs/Target Selection Config")]
    public sealed class TargetSelectionConfig : ScriptableObject
    {
        public float ScaleMultiplier => _scaleMultiplier;
        public float ShowDurationSeconds => _showDurationSeconds;
        public float HideDurationSeconds => _hideDurationSeconds;
        public Ease ShowEase => _showEase;
        public Ease HideEase => _hideEase;
        public Color OutlineColor => _outlineColor;
        public Color FillColor => _fillColor;

        [SerializeField] private float _scaleMultiplier = 1.05f;
        [SerializeField] private float _showDurationSeconds = 0.15f;
        [SerializeField] private float _hideDurationSeconds = 0.1f;
        [SerializeField] private Ease _showEase = Ease.OutQuad;
        [SerializeField] private Ease _hideEase = Ease.InQuad;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private Color _fillColor = new Color(r: 1f, g: 1f, b: 1f, a: 0.15f);

        public void Validate()
        {
            Guard.AgainstLessThan(
                _scaleMultiplier,
                minimum: 1f,
                () => new InvalidTargetSelectionValueException(nameof(_scaleMultiplier), _scaleMultiplier)
            );

            Guard.AgainstNegative(
                _showDurationSeconds,
                () => new InvalidTargetSelectionValueException(nameof(_showDurationSeconds), _showDurationSeconds)
            );

            Guard.AgainstNegative(
                _hideDurationSeconds,
                () => new InvalidTargetSelectionValueException(nameof(_hideDurationSeconds), _hideDurationSeconds)
            );
        }
    }
}
