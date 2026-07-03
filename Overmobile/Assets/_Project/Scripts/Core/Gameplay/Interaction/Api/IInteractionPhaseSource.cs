using R3;

namespace Core.Gameplay.Interaction
{
    public interface IInteractionPhaseSource
    {
        ReadOnlyReactiveProperty<InteractionPhase> CurrentPhase { get; }
    }
}
