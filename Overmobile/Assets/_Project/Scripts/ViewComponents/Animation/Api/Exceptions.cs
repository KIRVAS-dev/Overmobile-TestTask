using ExtendedExceptions;

namespace ViewComponents.Animation
{
    public sealed class CharacterAnimationTriggerMapMissingException : ExtendedException
    {
        public CharacterAnimationTriggerMapMissingException(string objectName)
            : base("character-animation-view-1", $"Character animation trigger map is not assigned on '{objectName}'") { }
    }

    public sealed class DuplicateCharacterAnimationSlotException : ExtendedException
    {
        public DuplicateCharacterAnimationSlotException(string objectName, string slot)
            : base("character-animation-view-2", $"Duplicate character animation slot '{slot}' on '{objectName}'") { }
    }

    public sealed class CharacterAnimationTriggerNameMissingException : ExtendedException
    {
        public CharacterAnimationTriggerNameMissingException(string objectName, string slot)
            : base("character-animation-view-3", $"Trigger name is missing for slot '{slot}' on '{objectName}'") { }
    }
}
