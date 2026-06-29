using ExtendedExceptions;

namespace ViewComponents.Interaction
{
    public sealed class InvalidInteractableTargetProviderException : ExtendedException
    {
        public InvalidInteractableTargetProviderException(string providerName, string reason)
            : base("interaction-view-1", $"Invalid interactable target provider '{providerName}': {reason}") { }
    }

    public sealed class MissingInteractableEntityKeyException : ExtendedException
    {
        public MissingInteractableEntityKeyException(string objectName)
            : base("interaction-view-2", $"Interactable target '{objectName}' is missing entity key") { }
    }
}
