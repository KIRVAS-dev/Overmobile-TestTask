using ExtendedExceptions;

namespace ViewComponents.Presentation
{
    public sealed class InvalidPresentationStepSequenceException : ExtendedException
    {
        public InvalidPresentationStepSequenceException(string objectName, string reason)
            : base("presentation-1", $"Invalid presentation step sequence on '{objectName}': {reason}") { }
    }

    public sealed class InvalidPresentationStepDefinitionException : ExtendedException
    {
        public InvalidPresentationStepDefinitionException(string stepTypeName, string reason)
            : base("presentation-2", $"Invalid presentation step '{stepTypeName}': {reason}") { }
    }

    public sealed class ActiveCharacterPresentationNotRegisteredException : ExtendedException
    {
        public ActiveCharacterPresentationNotRegisteredException()
            : base("presentation-3", "Active character presentation is not registered") { }
    }

    public sealed class ActiveCharacterAnchorNotFoundException : ExtendedException
    {
        public ActiveCharacterAnchorNotFoundException(string anchorKey)
            : base("presentation-4", $"Active character anchor '{anchorKey}' is not found") { }
    }

    public sealed class InvalidActiveCharacterAnchorViewException : ExtendedException
    {
        public InvalidActiveCharacterAnchorViewException(string objectName, string reason)
            : base("presentation-5", $"Invalid active character anchor view on '{objectName}': {reason}") { }
    }

    public sealed class InvalidPresentationSectionMapException : ExtendedException
    {
        public InvalidPresentationSectionMapException(string objectName, string reason)
            : base("presentation-6", $"Invalid presentation section map on '{objectName}': {reason}") { }
    }

    public sealed class MissingPresentationObjectSpawnerPrefabsException : ExtendedException
    {
        public MissingPresentationObjectSpawnerPrefabsException(string objectName)
            : base("presentation-7", $"Presentation object spawner prefabs are not assigned on '{objectName}'") { }
    }

    public sealed class InvalidPresentationObjectSpawnerPrefabException : ExtendedException
    {
        public InvalidPresentationObjectSpawnerPrefabException(string objectName, int prefabIndex)
            : base(
                "presentation-8",
                $"Presentation object spawner prefab at index {prefabIndex} is not assigned on '{objectName}'"
            ) { }
    }

    public sealed class InvalidPresentationObjectSpawnerCountException : ExtendedException
    {
        public InvalidPresentationObjectSpawnerCountException(string objectName, int spawnCount)
            : base(
                "presentation-9",
                $"Presentation object spawner spawn count must be greater than zero when random spawn is enabled on '{objectName}', got {spawnCount}"
            ) { }
    }
}
