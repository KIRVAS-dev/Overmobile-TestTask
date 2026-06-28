using ExtendedExceptions;

namespace Core.Animation
{
    public sealed class CharacterAnimationSlotNotMappedException : ExtendedException
    {
        public CharacterAnimationSlotNotMappedException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-1", $"Character animation slot '{slot}' is not mapped on '{objectName}'") { }
    }
}
