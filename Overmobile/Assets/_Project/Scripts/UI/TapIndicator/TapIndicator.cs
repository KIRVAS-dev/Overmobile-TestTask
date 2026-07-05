using DG.Tweening;
using ExtendedExceptions;
using Input;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.TapIndicator
{
    public sealed class TapIndicator
        : MonoBehaviour,
          ITapIndicatorTargetClickArming
    {
        private static readonly Vector3 HiddenScale = Vector3.zero;

        [SerializeField] private Image _image;
        [SerializeField] private TapIndicatorConfig _config;

        private IPlayerPointerInput _pointerInput;
        private RectTransform _rectTransform;
        private RectTransform _positionParent;
        private Vector3 _defaultScale;
        private Tween _activeScaleTween;
        private bool _isPointerHeld;
        private bool _isTargetClickReleaseArmed;

        [Inject]
        public void Construct(IPlayerPointerInput pointerInput)
        {
            _pointerInput = pointerInput;
        }

        private void Awake()
        {
            Validate();
            Initialize();
        }

        private void OnEnable()
        {
            _pointerInput.Pressed += OnPressed;
            _pointerInput.Released += OnReleased;
        }

        private void OnDisable()
        {
            _pointerInput.Pressed -= OnPressed;
            _pointerInput.Released -= OnReleased;

            Hide();
        }

        private void Update()
        {
            if (_pointerInput.IsPressed && _isPointerHeld)
            {
                FollowAt(_pointerInput.ScreenPosition);
            }
        }

        private void OnPressed()
        {
            ShowAt(_pointerInput.ScreenPosition);
        }

        private void OnReleased()
        {
            Hide();
        }

        void ITapIndicatorTargetClickArming.ArmTargetClickRelease()
        {
            _isTargetClickReleaseArmed = true;
        }

        void ITapIndicatorTargetClickArming.DisarmTargetClickRelease()
        {
            _isTargetClickReleaseArmed = false;
        }

        private void Initialize()
        {
            _rectTransform = _image.rectTransform;
            _positionParent = _rectTransform.parent as RectTransform;

            if (_positionParent == null)
            {
                throw new MissingTapIndicatorFieldException("positionParent", gameObject.name);
            }

            _defaultScale = _rectTransform.localScale;
            _image.sprite = _config.Sprite;
            _image.raycastTarget = false;
            _rectTransform.localScale = HiddenScale;
        }

        private void ShowAt(Vector2 screenPosition)
        {
            KillRegisteredTweens(_activeScaleTween, _rectTransform);

            _isPointerHeld = true;
            _rectTransform.anchoredPosition = ScreenToAnchoredPosition(screenPosition);
            _image.sprite = _config.Sprite;
            _rectTransform.localScale = HiddenScale;

            _activeScaleTween = _rectTransform.DOScale(_defaultScale, _config.ScaleInDuration).SetEase(_config.FadeInEase);
        }

        private void FollowAt(Vector2 screenPosition)
        {
            if (!_isPointerHeld)
            {
                return;
            }

            _rectTransform.anchoredPosition = ScreenToAnchoredPosition(screenPosition);
        }

        private void Hide()
        {
            if (!_isPointerHeld)
            {
                return;
            }

            _isPointerHeld = false;
            KillRegisteredTweens(_activeScaleTween, _rectTransform);

            if (_isTargetClickReleaseArmed)
            {
                _isTargetClickReleaseArmed = false;

                Vector3 peakScale = _defaultScale * _config.TargetClickScaleMultiplier;

                _activeScaleTween = DOTween
                   .Sequence()
                   .Append(
                        _rectTransform
                           .DOScale(peakScale, _config.TargetClickScaleUpDuration)
                           .SetEase(_config.TargetClickScaleUpEase)
                    )
                   .AppendInterval(_config.TargetClickScaleHoldDuration)
                   .Append(_rectTransform.DOScale(HiddenScale, _config.ScaleOutDuration).SetEase(_config.FadeOutEase));

                return;
            }

            PlayScaleOutTween();
        }

        private void PlayScaleOutTween()
        {
            _activeScaleTween = _rectTransform.DOScale(HiddenScale, _config.ScaleOutDuration).SetEase(_config.FadeOutEase);
        }

        private Vector2 ScreenToAnchoredPosition(Vector2 screenPosition)
        {
            return !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _positionParent,
                screenPosition,
                cam: null,
                out Vector2 localPoint
            )
                ? throw new InvalidTapIndicatorScreenPositionException(screenPosition)
                : localPoint;
        }

        private void OnDestroy()
        {
            KillRegisteredTweens(_activeScaleTween, _rectTransform);
        }

        private void KillRegisteredTweens(Tween tween, RectTransform targetRectTransform)
        {
            tween?.Kill();

            if (targetRectTransform != null)
            {
                targetRectTransform.DOKill();
            }
        }

        private void Validate()
        {
            _config.Validate();

            Guard.AgainstNull(_image, () => new MissingTapIndicatorFieldException(nameof(_image), gameObject.name));
            Guard.AgainstNull(_config, () => new MissingTapIndicatorFieldException(nameof(_config), gameObject.name));
        }
    }
}
