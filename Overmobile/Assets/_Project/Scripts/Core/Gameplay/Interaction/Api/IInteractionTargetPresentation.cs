namespace Core.Gameplay.Interaction
{
    public interface IInteractionTargetPresentation
    {
        void RegisterTargetPresentation(string entityId);
        void CompleteTargetPresentation(string entityId);
    }
}
