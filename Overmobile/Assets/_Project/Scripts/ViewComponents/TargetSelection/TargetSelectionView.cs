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

        private Tween _progressTween;
        private Vector3 _baseVisualLocalScale;
        private float _baseFloatingUiLocalPositionY;
        private float _currentProgress;

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
            ApplyProgress(_currentProgress);

            _progressTween = DOVirtual
               .Float(_currentProgress, to: 1f, _config.ShowDurationSeconds, ApplyProgress)
               .SetEase(_config.ShowEase);
        }

        public void Hide()
        {
            CancelAnimation();

            _progressTween = DOVirtual
               .Float(_currentProgress, to: 0f, _config.HideDurationSeconds, ApplyProgress)
               .SetEase(_config.HideEase)
               .OnComplete(() => _targetSelectionHighlighter.DisableHighlight());
        }

        public void PlayPointerDownSfx()
        {
            _pointerDownSfx.SetActive(false);
            _pointerDownSfx.SetActive(true);
        }

        private void ApplyProgress(float progress)
        {
            _currentProgress = progress;

            float factor = 1f + (_config.ScaleMultiplier - 1f) * progress;

            _visualRoot.localScale = _baseVisualLocalScale * factor;

            Vector3 floatingUiLocalPosition = _floatingUiRoot.localPosition;
            floatingUiLocalPosition.y = _baseFloatingUiLocalPositionY * factor;
            _floatingUiRoot.localPosition = floatingUiLocalPosition;

            _targetSelectionHighlighter.SetProgress(progress);
        }

        private void OnDestroy()
        {
            CancelAnimation();
        }

        private void CancelAnimation()
        {
            _progressTween?.Kill();
            _progressTween = null;
        }

        private void Validate()
        {
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

            _config.Validate();
        }
    }
}
