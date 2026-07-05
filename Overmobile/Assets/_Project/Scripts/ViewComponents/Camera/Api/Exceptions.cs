using ExtendedExceptions;

namespace ViewComponents.Camera
{
    public sealed class MissingCameraConfigFieldException : ExtendedException
    {
        public MissingCameraConfigFieldException(string fieldName, string objectName)
            : base("camera-config-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidCameraConfigValueException : ExtendedException
    {
        public InvalidCameraConfigValueException(string fieldName, float value)
            : base("camera-config-2", $"Field '{fieldName}' has invalid value: {value}") { }
    }

    public sealed class InvalidCameraConfigOrthographicRangeException : ExtendedException
    {
        public InvalidCameraConfigOrthographicRangeException(float minSize, float maxSize)
            : base("camera-config-3", $"Orthographic range is invalid: min {minSize}, max {maxSize}") { }
    }

    public sealed class CameraOrthographicRequiredException : ExtendedException
    {
        public CameraOrthographicRequiredException(string objectName)
            : base("camera-config-4", $"CinemachineCamera Lens on '{objectName}' must be orthographic") { }
    }

    public sealed class MissingCameraZoomFieldException : ExtendedException
    {
        public MissingCameraZoomFieldException(string fieldName, string objectName)
            : base("camera-zoom-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class MissingCameraZoomInputActionMapException : ExtendedException
    {
        public MissingCameraZoomInputActionMapException(string mapName)
            : base("camera-zoom-2", $"Input action map '{mapName}' was not found") { }
    }

    public sealed class MissingCameraZoomInputActionException : ExtendedException
    {
        public MissingCameraZoomInputActionException(string mapName, string actionName)
            : base("camera-zoom-3", $"Input action '{actionName}' was not found in map '{mapName}'") { }
    }

    public sealed class CameraZoomViewAlreadyListeningException : ExtendedException
    {
        public CameraZoomViewAlreadyListeningException()
            : base("camera-zoom-4", "Camera zoom view is already listening") { }
    }

    public sealed class MissingCameraShakeFieldException : ExtendedException
    {
        public MissingCameraShakeFieldException(string fieldName, string objectName)
            : base("camera-shake-view-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class CameraShakeOrthographicRequiredException : ExtendedException
    {
        public CameraShakeOrthographicRequiredException(string objectName)
            : base("camera-shake-view-2", $"CinemachineCamera Lens on '{objectName}' must be orthographic") { }
    }

    public sealed class MissingCameraConfinerOrientationFieldException : ExtendedException
    {
        public MissingCameraConfinerOrientationFieldException(string fieldName, string objectName)
            : base("camera-confiner-orientation-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class MissingCameraLateUpdateDriverFieldException : ExtendedException
    {
        public MissingCameraLateUpdateDriverFieldException(string fieldName, string objectName)
            : base("camera-late-update-driver-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
