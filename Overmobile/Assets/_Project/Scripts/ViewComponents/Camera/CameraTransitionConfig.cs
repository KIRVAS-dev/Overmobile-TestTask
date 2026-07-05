using DG.Tweening;
using UnityEngine;

namespace ViewComponents.Camera
{
    [CreateAssetMenu(fileName = "CameraTransitionConfig", menuName = "Project/Configs/Camera/Camera Transition Config")]
    public sealed class CameraTransitionConfig : ScriptableObject
    {
        public float DelayBeforeTransition => _delayBeforeTransition;
        public float TransitionSpeed => _transitionSpeed;
        public Ease Ease => _ease;
        public Vector3 StartPosition => _startPosition;
        public float StartOrthographicSize => _startOrthographicSize;
        public Vector3 TargetPosition => _targetPosition;
        public float TargetOrthographicSize => _targetOrthographicSize;

        [Min(0f)]
        [SerializeField] private float _delayBeforeTransition;

        [Min(0f)]
        [SerializeField] private float _transitionSpeed = 2f;
        [SerializeField] private Ease _ease = Ease.InOutQuad;

        [Header("Start Parameters")]
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private float _startOrthographicSize = 5f;

        [Header("Target Parameters")]
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private float _targetOrthographicSize = 8f;
    }
}
