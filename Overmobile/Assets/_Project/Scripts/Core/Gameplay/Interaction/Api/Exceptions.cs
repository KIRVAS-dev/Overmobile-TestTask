using ExtendedExceptions;

namespace Core.Gameplay.Interaction
{
    public sealed class InteractableTargetNotFoundException : ExtendedException
    {
        public InteractableTargetNotFoundException(string entityKey)
            : base("interaction-1", $"Interactable target not found for key '{entityKey}'") { }
    }

    public sealed class InteractableTargetNotFoundByEntityIdException : ExtendedException
    {
        public InteractableTargetNotFoundByEntityIdException(string entityId)
            : base("interaction-2", $"Interactable target not found for entity id '{entityId}'") { }
    }

    public sealed class InteractionPipelineTargetPresentationEntityMismatchException : ExtendedException
    {
        public InteractionPipelineTargetPresentationEntityMismatchException(string entityId, string currentTargetEntityId)
            : base(
                "interaction-3",
                $"Target presentation entity id '{entityId}' does not match current target '{currentTargetEntityId}'"
            ) { }
    }
}
