using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Gameplay.Interaction
{
    public interface IInteractionPipeline
        : IInteractionPhaseSource,
          IInteractionTargetPresentation
    {
        bool HasTargetPresentation(string entityId);
        void BeginInteraction(string entityId);
        void SetPhase(InteractionPhase phase);
        void EndInteraction();
        UniTask AwaitTargetPresentationAsync(CancellationToken cancellationToken);
    }
}
