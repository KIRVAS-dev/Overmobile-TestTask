using DG.Tweening;
using System;
using UnityEngine;

namespace ViewComponents.UI.PowerPanel
{
    [Serializable]
    public sealed class PowerPanelScaleChangeSettings
    {
        [SerializeField] private Vector3 _fromScale = Vector3.one;
        [SerializeField] private Vector3 _toScale = new Vector3(1.15f, 1.15f, 1.15f);
        [SerializeField] private float _durationSeconds = 0.3f;
        [SerializeField] private Ease _ease = Ease.OutBack;

        public Vector3 FromScale => _fromScale;
        public Vector3 ToScale => _toScale;
        public float DurationSeconds => _durationSeconds;
        public Ease Ease => _ease;
    }

    [CreateAssetMenu(fileName = "PowerPanelChangeConfig", menuName = "Project/Configs/UI/Power Panel Change Config")]
    public sealed class PowerPanelChangeConfig : ScriptableObject
    {
        [SerializeField] private PowerPanelScaleChangeSettings _panelScaleChange = new PowerPanelScaleChangeSettings();
        [SerializeField] private PowerPanelScaleChangeSettings _textScaleChange = new PowerPanelScaleChangeSettings();
        [SerializeField] private bool _resetToDefaultScaleOnComplete = true;

        public PowerPanelScaleChangeSettings PanelScaleChange => _panelScaleChange;
        public PowerPanelScaleChangeSettings TextScaleChange => _textScaleChange;
        public bool ResetToDefaultScaleOnComplete => _resetToDefaultScaleOnComplete;
    }
}
