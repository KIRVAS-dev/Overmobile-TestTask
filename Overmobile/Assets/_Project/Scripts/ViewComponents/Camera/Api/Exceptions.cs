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
}
