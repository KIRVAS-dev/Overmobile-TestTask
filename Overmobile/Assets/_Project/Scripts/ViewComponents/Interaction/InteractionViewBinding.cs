using Core;
using Core.Gameplay.Power;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using ViewComponents.Presentation;

namespace ViewComponents.Interaction
{
    public sealed class InteractionViewBinding : IDisposable
    {
        private readonly CancellationTokenSource _presentationCancellationTokenSource;
        private readonly IDisposable _resolvedSubscription;

        public InteractionViewBinding(IPowerRegistry powerRegistry, string entityId, IGameplayInputBlock gameplayInputBlock,
            PresentationStepSequence presentationSequence)
        {
            _presentationCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _presentationCancellationTokenSource.Token;

            IPowerEntity powerEntity = powerRegistry.Get(entityId);

            if (powerEntity.IsResolved.CurrentValue)
            {
                RunResolvePresentationAsync(gameplayInputBlock, presentationSequence, cancellationToken).Forget();

                return;
            }

            _resolvedSubscription = powerEntity
               .IsResolved
               .Where(isResolved => isResolved)
               .Take(1)
               .Subscribe(_ => RunResolvePresentationAsync(gameplayInputBlock, presentationSequence, cancellationToken).Forget());
        }

        public void Dispose()
        {
            _presentationCancellationTokenSource.Cancel();
            _presentationCancellationTokenSource.Dispose();
            _resolvedSubscription?.Dispose();
        }

        private async UniTaskVoid RunResolvePresentationAsync(IGameplayInputBlock gameplayInputBlock,
            PresentationStepSequence presentationSequence, CancellationToken cancellationToken)
        {
            gameplayInputBlock.Block();

            try
            {
                await presentationSequence.StartPresentationAsync(cancellationToken);
            }
            catch (OperationCanceledException) { }
            finally
            {
                gameplayInputBlock.Unblock();
            }
        }
    }
}
