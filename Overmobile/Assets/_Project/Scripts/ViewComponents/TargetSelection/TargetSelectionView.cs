using DG.Tweening;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.TargetSelection
{
    public sealed class TargetSelectionView : MonoBehaviour
    {
        [SerializeField] private TargetSelectionHighlighter _targetSelectionHighlighter;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Transform _floatingUiRoot;
        [SerializeField] private GameObject _pointerDownSfx;
        [SerializeField] private TargetSelectionConfig _config;

        private Tween _highlightTween;
        private Tween _scaleTween;
        private Vector3 _baseVisualLocalScale;
        private float _baseFloatingUiLocalPositionY;
        private float _highlightProgress;
        private float _scaleProgress;
        private bool _isScaleEnabled = true;
        private bool _isHighlightVisible;

        private void Awake()
        {
            Validate();

            _baseVisualLocalScale = _visualRoot.localScale;
            _baseFloatingUiLocalPositionY = _floatingUiRoot.localPosition.y;

            _targetSelectionHighlighter.Initialize(_config);
        }

        public void Show()
        {
            CancelAnimation();
            _targetSelectionHighlighter.EnableHighlight();
            _isHighlightVisible = true;

            ApplyHighlightProgress(_highlightProgress);
            ApplyScaleProgress(_scaleProgress);

            _highlightTween = DOVirtual
               .Float(_highlightProgress, to: 1f, _config.ShowDurationSeconds, ApplyHighlightProgress)
               .SetEase(_config.ShowEase);

            float targetScaleProgress = _isScaleEnabled
                ? 1f
                : 0f;

            _scaleTween = DOVirtual
               .Float(_scaleProgress, targetScaleProgress, _config.ShowDurationSeconds, ApplyScaleProgress)
               .SetEase(_config.ShowEase);
        }

        public void Hide()
        {
            CancelAnimation();
            _isHighlightVisible = false;

            _highlightTween = DOVirtual
               .Float(_highlightProgress, to: 0f, _config.HideDurationSeconds, ApplyHighlightProgress)
               .SetEase(_config.HideEase)
               .OnComplete(() => _targetSelectionHighlighter.DisableHighlight());

            _scaleTween = DOVirtual
               .Float(_scaleProgress, to: 0f, _config.HideDurationSeconds, ApplyScaleProgress)
               .SetEase(_config.HideEase);
        }

        public void SetScaleEnabled(bool isScaleEnabled)
        {
            if (_isScaleEnabled == isScaleEnabled)
            {
                return;
            }

            _isScaleEnabled = isScaleEnabled;

            if (!_isHighlightVisible)
            {
                return;
            }

            _scaleTween?.Kill();
            _scaleTween = null;

            float targetScaleProgress = isScaleEnabled
                ? 1f
                : 0f;

            float duration = isScaleEnabled
                ? _config.ShowDurationSeconds
                : _config.HideDurationSeconds;

            Ease ease = isScaleEnabled
                ? _config.ShowEase
                : _config.HideEase;

            _scaleTween = DOVirtual.Float(_scaleProgress, targetScaleProgress, duration, ApplyScaleProgress).SetEase(ease);
        }

        public void PlayPointerDownSfx()
        {
            _pointerDownSfx.SetActive(false);
            _pointerDownSfx.SetActive(true);
        }

        private void ApplyHighlightProgress(float progress)
        {
            _highlightProgress = progress;
            _targetSelectionHighlighter.SetProgress(progress);
        }

        private void ApplyScaleProgress(float progress)
        {
            _scaleProgress = progress;

            float factor = 1f + (_config.ScaleMultiplier - 1f) * progress;

            _visualRoot.localScale = _baseVisualLocalScale * factor;

            Vector3 floatingUiLocalPosition = _floatingUiRoot.localPosition;
            floatingUiLocalPosition.y = _baseFloatingUiLocalPositionY * factor;
            _floatingUiRoot.localPosition = floatingUiLocalPosition;
        }

        private void OnDestroy()
        {
            CancelAnimation();
        }

        private void CancelAnimation()
        {
            _highlightTween?.Kill();
            _highlightTween = null;

            _scaleTween?.Kill();
            _scaleTween = null;
        }

        private void Validate()
        {
            _config.Validate();

            Guard.AgainstNull(_config, () => new MissingTargetSelectionFieldException(nameof(_config), gameObject.name));
            Guard.AgainstNull(_visualRoot, () => new MissingTargetSelectionFieldException(nameof(_visualRoot), gameObject.name));

            Guard.AgainstNull(
                _floatingUiRoot,
                () => new MissingTargetSelectionFieldException(nameof(_floatingUiRoot), gameObject.name)
            );

            Guard.AgainstNull(
                _targetSelectionHighlighter,
                () => new MissingTargetSelectionFieldException(nameof(_targetSelectionHighlighter), gameObject.name)
            );

            Guard.AgainstNull(
                _pointerDownSfx,
                () => new MissingTargetSelectionFieldException(nameof(_pointerDownSfx), gameObject.name)
            );
        }
    }
}
