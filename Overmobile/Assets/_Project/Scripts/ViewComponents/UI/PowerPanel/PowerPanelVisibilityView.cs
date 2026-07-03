using Core.Gameplay.Interaction;
using Core.Gameplay.Power;
using DG.Tweening;
using System;
using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [DisallowMultipleComponent]
    public sealed class PowerPanelVisibilityView
        : MonoBehaviour,
          IPowerPanelVisibilityView
    {
        private const float VisibleAlpha = 1f;
        private const float HiddenAlpha = 0f;

        [SerializeField] private PowerPanelView _powerPanelView;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private PowerPanelVisibilityConfig _config;

        private Tween _fadeTween;
        private IDisposable _guardBinding;

        public void Bind(IEntityGuardAccessRegistry guardAccessRegistry, string entityId)
        {
            _guardBinding?.Dispose();
            _guardBinding = new EntityGuardPowerPanelBinding(guardAccessRegistry, entityId, this);
        }

        public void Show()
        {
            ApplyTransition(_config.EnableTransition, VisibleAlpha);
        }

        public void Hide()
        {
            ApplyTransition(_config.DisableTransition, HiddenAlpha);
        }

        private void Awake()
        {
            Validate();
        }

        private void ApplyTransition(PowerPanelVisibilityTransition transition, float targetAlpha)
        {
            CancelAnimation();

            if (transition.Mode == PowerPanelVisibilityTransitionMode.Instant)
            {
                _canvasGroup.alpha = targetAlpha;

                return;
            }

            if (transition.FadeDuration < 0f)
            {
                throw new InvalidPowerPanelVisibilityFadeDurationException(gameObject.name, transition.FadeDuration);
            }

            _fadeTween = _canvasGroup.DOFade(targetAlpha, transition.FadeDuration).SetEase(transition.FadeEase);
        }

        private void OnDestroy()
        {
            _guardBinding?.Dispose();
            CancelAnimation();
        }

        private void CancelAnimation()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
            _canvasGroup.DOKill();
        }

        private void Validate()
        {
            if (_powerPanelView == null)
            {
                throw new InvalidPowerPanelVisibilityViewException(gameObject.name, "Power panel view is not assigned");
            }

            if (_canvasGroup == null)
            {
                throw new InvalidPowerPanelVisibilityViewException(gameObject.name, "Canvas group is not assigned");
            }

            if (_config == null)
            {
                throw new MissingPowerPanelVisibilityConfigException(gameObject.name);
            }
        }
    }
}
