using ExtendedExceptions;

namespace ViewComponents.Camera
{
    public sealed class CameraTransitionOrthographicRequiredException : ExtendedException
    {
        public CameraTransitionOrthographicRequiredException(string objectName)
            : base("camera-transition-1", $"Camera on '{objectName}' must be orthographic") { }
    }

    public sealed class CameraTransitionInvalidDelayException : ExtendedException
    {
        public CameraTransitionInvalidDelayException(string objectName, float delayBeforeTransition)
            : base(
                "camera-transition-2",
                $"Delay before transition must be zero or greater on '{objectName}', got {delayBeforeTransition}"
            ) { }
    }

    public sealed class MissingCameraTransitionConfigException : ExtendedException
    {
        public MissingCameraTransitionConfigException(string objectName)
            : base("camera-transition-3", $"Camera transition config is not assigned on '{objectName}'") { }
    }
}
