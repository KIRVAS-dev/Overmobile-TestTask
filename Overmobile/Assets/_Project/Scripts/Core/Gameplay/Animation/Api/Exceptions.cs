using ExtendedExceptions;

namespace Core.Gameplay.Animation
{
    public sealed class CharacterAnimationSlotNotMappedException : ExtendedException
    {
        public CharacterAnimationSlotNotMappedException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-1", $"Character animation slot '{slot}' is not mapped on '{objectName}'") { }
    }
}
