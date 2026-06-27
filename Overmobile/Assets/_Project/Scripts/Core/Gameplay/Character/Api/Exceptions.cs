using ExtendedExceptions;

namespace Core.Gameplay.Character
{
    public sealed class ActiveCharacterViewNotRegisteredException : ExtendedException
    {
        public ActiveCharacterViewNotRegisteredException()
            : base("character-1", "Active character view is not registered") { }
    }

    public sealed class InvalidActiveCharacterViewRegistrationException : ExtendedException
    {
        public InvalidActiveCharacterViewRegistrationException()
            : base("character-2", "Active character view registration is invalid") { }
    }
}
