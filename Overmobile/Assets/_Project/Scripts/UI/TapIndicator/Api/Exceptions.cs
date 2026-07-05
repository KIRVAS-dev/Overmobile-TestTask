using ExtendedExceptions;
using UnityEngine;

namespace UI.TapIndicator
{
    public sealed class MissingTapIndicatorFieldException : ExtendedException
    {
        public MissingTapIndicatorFieldException(string fieldName, string objectName)
            : base("tap-indicator-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidTapIndicatorSpriteException : ExtendedException
    {
        public InvalidTapIndicatorSpriteException()
            : base("tap-indicator-2", "Tap indicator config sprite is not assigned") { }
    }

    public sealed class InvalidTapIndicatorScaleDurationException : ExtendedException
    {
        public InvalidTapIndicatorScaleDurationException(string scaleDurationName, float scaleDuration)
            : base(
                "tap-indicator-3",
                $"Tap indicator config {scaleDurationName} must be zero or greater, got {scaleDuration}"
            ) { }
    }

    public sealed class InvalidTapIndicatorScreenPositionException : ExtendedException
    {
        public InvalidTapIndicatorScreenPositionException(Vector2 screenPosition)
            : base(
                "tap-indicator-4",
                $"Tap indicator screen position {screenPosition} could not be converted to canvas coordinates"
            ) { }
    }

    public sealed class InvalidTapIndicatorValueException : ExtendedException
    {
        public InvalidTapIndicatorValueException(string fieldName, float value)
            : base("tap-indicator-5", $"Tap indicator config {fieldName} must be one or greater, got {value}") { }
    }
}
