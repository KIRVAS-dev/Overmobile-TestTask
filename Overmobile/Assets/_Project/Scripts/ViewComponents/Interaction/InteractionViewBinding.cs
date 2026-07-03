using Core;
using Core.Gameplay.Interaction;
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
        private readonly string _entityId;
        private readonly IInteractionTargetPresentation _interactionTargetPresentation;
        private readonly IInteractionPhaseSource _interactionPhaseSource;
        private readonly CancellationTokenSource _presentationCancellationTokenSource;
        private readonly IDisposable _resolvedSubscription;

        public InteractionViewBinding(
            IPowerRegistry powerRegistry,
            IInteractionPhaseSource interactionPhaseSource,
            string entityId,
            IGameplayInputBlock gameplayInputBlock,
            IInteractionTargetPresentation interactionTargetPresentation,
            PresentationStepSequence presentationSequence)
        {
            _entityId = entityId;
            _interactionTargetPresentation = interactionTargetPresentation;
            _interactionPhaseSource = interactionPhaseSource;
            _presentationCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _presentationCancellationTokenSource.Token;

            IPowerEntity powerEntity = powerRegistry.Get(entityId);

            if (powerEntity.IsResolved.CurrentValue)
            {
                RunPresentationImmediateAsync(gameplayInputBlock, presentationSequence, cancellationToken).Forget();

                return;
            }

            _resolvedSubscription = powerEntity
               .IsResolved
               .Where(isResolved => isResolved)
               .Take(1)
               .Subscribe(_ => RunResolvePresentationAsync(gameplayInputBlock, presentationSequence, cancellationToken).Forget());
        }

        void IDisposable.Dispose()
        {
            _presentationCancellationTokenSource.Cancel();
            _presentationCancellationTokenSource.Dispose();
            _resolvedSubscription?.Dispose();
        }

        private async UniTaskVoid RunResolvePresentationAsync(
            IGameplayInputBlock gameplayInputBlock,
            PresentationStepSequence presentationSequence,
            CancellationToken cancellationToken)
        {
            if (_interactionPhaseSource.CurrentPhase.CurrentValue != InteractionPhase.TargetPresentation
             && _interactionPhaseSource.CurrentPhase.CurrentValue != InteractionPhase.Idle)
            {
                await UniTask.WaitUntil(
                    () => _interactionPhaseSource.CurrentPhase.CurrentValue == InteractionPhase.TargetPresentation
                     || _interactionPhaseSource.CurrentPhase.CurrentValue == InteractionPhase.Idle,
                    PlayerLoopTiming.Update,
                    cancellationToken
                );
            }

            await RunPresentationCoreAsync(gameplayInputBlock, presentationSequence, cancellationToken);
        }

        private async UniTaskVoid RunPresentationImmediateAsync(
            IGameplayInputBlock gameplayInputBlock,
            PresentationStepSequence presentationSequence,
            CancellationToken cancellationToken)
        {
            await RunPresentationCoreAsync(gameplayInputBlock, presentationSequence, cancellationToken);
        }

        private async UniTask RunPresentationCoreAsync(
            IGameplayInputBlock gameplayInputBlock,
            PresentationStepSequence presentationSequence,
            CancellationToken cancellationToken)
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
                _interactionTargetPresentation.CompleteTargetPresentation(_entityId);
            }
        }
    }
}
