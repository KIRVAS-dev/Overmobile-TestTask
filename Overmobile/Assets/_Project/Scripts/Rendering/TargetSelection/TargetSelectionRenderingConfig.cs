using ExtendedExceptions;
using UnityEngine;

namespace Rendering.TargetSelection
{
    [CreateAssetMenu(
        fileName = "TargetSelectionRenderingConfig",
        menuName = "Project/Configs/Target Selection/Rendering Config")]
    public sealed class TargetSelectionRenderingConfig : ScriptableObject
    {
        public float OutlineThicknessPixels => _outlineThicknessPixels;

        [Min(0f)]
        [SerializeField] private float _outlineThicknessPixels = 2f;

        public void Validate()
        {
            Guard.AgainstNegative(
                _outlineThicknessPixels,
                () => new InvalidTargetSelectionRenderingValueException(nameof(_outlineThicknessPixels), _outlineThicknessPixels)
            );
        }
    }
}
