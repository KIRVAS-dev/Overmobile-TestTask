using ExtendedExceptions;

namespace ViewComponents.TargetSelection
{
    public sealed class MissingTargetSelectionFieldException : ExtendedException
    {
        public MissingTargetSelectionFieldException(string fieldName, string objectName)
            : base("target-selection-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidTargetSelectionValueException : ExtendedException
    {
        public InvalidTargetSelectionValueException(string fieldName, float value)
            : base("target-selection-2", $"Target selection config field '{fieldName}' has invalid value: {value}") { }
    }

    public sealed class MissingTargetSelectionViewException : ExtendedException
    {
        public MissingTargetSelectionViewException(string objectName)
            : base("target-selection-3", $"Target selection view is not assigned on '{objectName}'") { }
    }
}
