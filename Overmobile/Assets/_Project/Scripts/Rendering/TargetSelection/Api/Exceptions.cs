using ExtendedExceptions;

namespace Rendering.TargetSelection
{
    public sealed class MissingTargetSelectionShaderException : ExtendedException
    {
        public MissingTargetSelectionShaderException(string featureName, string shaderRole)
            : base(
                "target-selection-rendering-1",
                $"Target selection shader '{shaderRole}' is not assigned on '{featureName}'"
            ) { }
    }

    public sealed class MissingTargetSelectionMaterialException : ExtendedException
    {
        public MissingTargetSelectionMaterialException(string sourceName)
            : base("target-selection-rendering-2", $"Target selection material is not created for '{sourceName}'") { }
    }

    public sealed class MissingTargetSelectionRenderingConfigException : ExtendedException
    {
        public MissingTargetSelectionRenderingConfigException(string featureName)
            : base("target-selection-rendering-3", $"Target selection rendering config is not assigned on '{featureName}'") { }
    }

    public sealed class InvalidTargetSelectionRenderingValueException : ExtendedException
    {
        public InvalidTargetSelectionRenderingValueException(string fieldName, float value)
            : base(
                "target-selection-rendering-4",
                $"Target selection rendering field '{fieldName}' has invalid value: {value}"
            ) { }
    }

    public sealed class UnbalancedTargetSelectionHighlightActivityException : ExtendedException
    {
        public UnbalancedTargetSelectionHighlightActivityException()
            : base(
                "target-selection-rendering-5",
                "Target selection highlight activity unregistered more times than registered"
            ) { }
    }
}
