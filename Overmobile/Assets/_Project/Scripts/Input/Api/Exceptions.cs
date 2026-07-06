using ExtendedExceptions;

namespace Input
{
    public sealed class MissingPlayerPointerActionsAssetException : ExtendedException
    {
        public MissingPlayerPointerActionsAssetException(string objectName)
            : base("input-1", $"Player pointer actions asset is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPlayerPointerActionMapException : ExtendedException
    {
        public MissingPlayerPointerActionMapException(string mapName)
            : base("input-2", $"Input action map '{mapName}' was not found") { }
    }

    public sealed class MissingPlayerPointerActionException : ExtendedException
    {
        public MissingPlayerPointerActionException(string mapName, string actionName)
            : base("input-3", $"Input action '{mapName}/{actionName}' was not found") { }
    }

    public sealed class ActivePlayerPointerNotAssignedException : ExtendedException
    {
        public ActivePlayerPointerNotAssignedException()
            : base("input-4", "Active player pointer is not assigned") { }
    }

    public sealed class InvalidPlayerPointerInputConfigValueException : ExtendedException
    {
        public InvalidPlayerPointerInputConfigValueException(string fieldName, float value)
            : base("input-5", $"Player pointer input config {fieldName} has invalid value {value}") { }
    }

    public sealed class InvalidPointerIntentStateException : ExtendedException
    {
        public InvalidPointerIntentStateException(string objectName, int stateValue)
            : base("input-6", $"Player pointer intent state {stateValue} is invalid on '{objectName}'") { }
    }

    public sealed class MissingPlayerPointerInputConfigException : ExtendedException
    {
        public MissingPlayerPointerInputConfigException(string objectName)
            : base("input-7", $"Player pointer input config is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPointAreaFieldException : ExtendedException
    {
        public MissingPointAreaFieldException(string fieldName, string objectName)
            : base("input-8", $"Point area {fieldName} is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPointerDownIntentGateException : ExtendedException
    {
        public MissingPointerDownIntentGateException(string objectName)
            : base("input-9", $"Pointer down intent gate is not assigned on '{objectName}'") { }
    }

    public sealed class MissingPlayerPointerInputForPointAreaBindException : ExtendedException
    {
        public MissingPlayerPointerInputForPointAreaBindException(string objectName)
            : base("input-10", $"Player pointer input is not assigned for point area bind on '{objectName}'") { }
    }
}
