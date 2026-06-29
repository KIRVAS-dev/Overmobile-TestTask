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
}
