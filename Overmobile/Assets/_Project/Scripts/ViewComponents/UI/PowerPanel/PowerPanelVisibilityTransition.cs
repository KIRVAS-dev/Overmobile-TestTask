using DG.Tweening;
using System;
using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [Serializable]
    public sealed class PowerPanelVisibilityTransition
    {
        [SerializeField] private PowerPanelVisibilityTransitionMode _mode = PowerPanelVisibilityTransitionMode.Instant;
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private Ease _fadeEase = Ease.InOutQuad;

        public PowerPanelVisibilityTransitionMode Mode => _mode;
        public float FadeDuration => _fadeDuration;
        public Ease FadeEase => _fadeEase;
    }
}
