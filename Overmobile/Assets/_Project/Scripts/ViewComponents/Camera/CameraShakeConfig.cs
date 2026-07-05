using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Camera
{
    [CreateAssetMenu(fileName = "CameraShakeConfig", menuName = "Project/Configs/Camera Shake Config")]
    public sealed class CameraShakeConfig : ScriptableObject
    {
        private const float MaxRandomness = 180f;

        public Vector3 Strength => _strength;
        public float Duration => _duration;
        public int Vibrato => _vibrato;
        public float Randomness => _randomness;
        public bool FadeOut => _fadeOut;

        [SerializeField] private Vector3 _strength = new Vector3(x: 0.25f, y: 0.25f, z: 0f);

        [Min(0f)]
        [SerializeField] private float _duration = 0.3f;

        [Min(1)]
        [SerializeField] private int _vibrato = 3;

        [Range(min: 0f, max: 180f)]
        [SerializeField] private float _randomness = 90f;

        [SerializeField] private bool _fadeOut = true;

        public void Validate()
        {
            Guard.AgainstNonPositive(_duration, () => new InvalidCameraShakeValueException(nameof(_duration), _duration));

            Guard.AgainstNonPositive(_vibrato, () => new InvalidCameraShakeValueException(nameof(_vibrato), _vibrato));

            if (_randomness is < 0f or > MaxRandomness)
            {
                throw new InvalidCameraShakeValueException(nameof(_randomness), _randomness);
            }
        }
    }
}
