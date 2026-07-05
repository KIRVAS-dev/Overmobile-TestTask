using ExtendedExceptions;

namespace ViewComponents.Camera
{
    public sealed class MissingCameraTransitionFieldException : ExtendedException
    {
        public MissingCameraTransitionFieldException(string fieldName, string objectName)
            : base("camera-transition-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidCameraTransitionValueException : ExtendedException
    {
        public InvalidCameraTransitionValueException(
            string fieldName,
            string objectName,
            float value)
            : base("camera-transition-2", $"Field '{fieldName}' on '{objectName}' has invalid value: {value}") { }
    }

    public sealed class CameraTransitionOrthographicRequiredException : ExtendedException
    {
        public CameraTransitionOrthographicRequiredException(string objectName)
            : base("camera-transition-3", $"Camera on '{objectName}' must be orthographic") { }
    }

    public sealed class MissingCameraShakeFieldException : ExtendedException
    {
        public MissingCameraShakeFieldException(string fieldName, string objectName)
            : base("camera-shake-view-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class CameraShakeOrthographicRequiredException : ExtendedException
    {
        public CameraShakeOrthographicRequiredException(string objectName)
            : base("camera-shake-view-2", $"Camera on '{objectName}' must be orthographic") { }
    }

    public sealed class InvalidCameraShakeValueException : ExtendedException
    {
        public InvalidCameraShakeValueException(string fieldName, float value)
            : base("camera-shake-1", $"Field '{fieldName}' has invalid value: {value}") { }

        public InvalidCameraShakeValueException(string fieldName, int value)
            : base("camera-shake-2", $"Field '{fieldName}' has invalid value: {value}") { }
    }
}
