using ExtendedExceptions;

namespace Core.Gameplay.Interaction
{
    public sealed class InteractableTargetNotFoundException : ExtendedException
    {
        public InteractableTargetNotFoundException(string entityKey)
            : base("interaction-1", $"Interactable target not found for key '{entityKey}'") { }
    }

    public sealed class InteractableTargetNotFoundByPowerIdException : ExtendedException
    {
        public InteractableTargetNotFoundByPowerIdException(string powerId)
            : base("interaction-2", $"Interactable target not found for power id '{powerId}'") { }
    }
}
