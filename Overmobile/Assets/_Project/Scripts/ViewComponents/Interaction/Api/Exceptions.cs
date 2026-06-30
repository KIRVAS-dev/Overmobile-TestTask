using ExtendedExceptions;

namespace ViewComponents.Interaction
{
    public sealed class InvalidInteractableTargetProviderException : ExtendedException
    {
        public InvalidInteractableTargetProviderException(string providerName, string reason)
            : base("interaction-view-1", $"Invalid interactable target provider '{providerName}': {reason}") { }
    }

    public sealed class InvalidInteractionEntityGuardsException : ExtendedException
    {
        public InvalidInteractionEntityGuardsException(string objectName, string reason)
            : base("interaction-view-2", $"Invalid interaction entity guards on '{objectName}': {reason}") { }
    }
}
