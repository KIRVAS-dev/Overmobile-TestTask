using ExtendedExceptions;
using UnityEngine;

namespace Input
{
    [CreateAssetMenu(fileName = "PlayerPointerInputConfig", menuName = "Project/Configs/Player Pointer Input Config")]
    public sealed class PlayerPointerInputConfig : ScriptableObject
    {
        public float ConfirmDelaySeconds => _confirmDelaySeconds;
        public float TouchSlopPixels => _touchSlopPixels;
        public bool MouseUsesInstantConfirm => _mouseUsesInstantConfirm;

        [SerializeField] private float _confirmDelaySeconds = 0.08f;
        [SerializeField] private float _touchSlopPixels = 12f;
        [SerializeField] private bool _mouseUsesInstantConfirm = true;

        public void Validate()
        {
            Guard.AgainstNegative(
                _confirmDelaySeconds,
                () => new InvalidPlayerPointerInputConfigValueException(nameof(_confirmDelaySeconds), _confirmDelaySeconds)
            );

            Guard.AgainstNonPositive(
                _touchSlopPixels,
                () => new InvalidPlayerPointerInputConfigValueException(nameof(_touchSlopPixels), _touchSlopPixels)
            );
        }
    }
}
