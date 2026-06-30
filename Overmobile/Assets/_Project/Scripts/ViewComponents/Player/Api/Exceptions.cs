using ExtendedExceptions;

namespace ViewComponents.Player
{
    public sealed class UpgradeTierIndexOutOfRangeException : ExtendedException
    {
        public UpgradeTierIndexOutOfRangeException(string objectName, int tierIndex, int tierCount)
            : base("player-view-1", $"Upgrade tier index {tierIndex} is out of range [0, {tierCount}) on '{objectName}'") { }
    }

    public sealed class MissingUpgradeTierPrefabException : ExtendedException
    {
        public MissingUpgradeTierPrefabException(string objectName, int tierIndex)
            : base("player-view-2", $"Upgrade tier prefab at index {tierIndex} is not assigned on '{objectName}'") { }
    }

    public sealed class MissingCharacterViewComponentException : ExtendedException
    {
        public MissingCharacterViewComponentException(string prefabName, string componentName)
            : base("player-view-3", $"Upgrade tier prefab '{prefabName}' is missing required component '{componentName}'") { }
    }

    public sealed class MissingActiveCharacterAnchorViewException : ExtendedException
    {
        public MissingActiveCharacterAnchorViewException(string prefabName)
            : base("player-view-4", $"Upgrade tier prefab '{prefabName}' is missing required component 'ActiveCharacterAnchorView'") { }
    }
}
