using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Threading;

namespace Core.Gameplay.Interaction
{
    public sealed class InteractionPipeline : IInteractionPipeline
    {
        private readonly ReactiveProperty<InteractionPhase> _currentPhase =
            new ReactiveProperty<InteractionPhase>(InteractionPhase.Idle);

        private readonly HashSet<string> _targetPresentationEntityIds = new HashSet<string>();

        private UniTaskCompletionSource _targetPresentationCompletionSource;
        private bool _isTargetPresentationComplete;
        private string _currentTargetEntityId = string.Empty;

        public ReadOnlyReactiveProperty<InteractionPhase> CurrentPhase => _currentPhase;

        public void RegisterTargetPresentation(string entityId)
        {
            _targetPresentationEntityIds.Add(entityId);
        }

        public bool HasTargetPresentation(string entityId)
        {
            return _targetPresentationEntityIds.Contains(entityId);
        }

        public void BeginInteraction(string entityId)
        {
            _currentTargetEntityId = entityId;
            _isTargetPresentationComplete = false;
            _targetPresentationCompletionSource = null;
            _currentPhase.Value = InteractionPhase.Approach;
        }

        public void SetPhase(InteractionPhase phase)
        {
            _currentPhase.Value = phase;
        }

        public UniTask AwaitTargetPresentationAsync(CancellationToken cancellationToken)
        {
            if (_isTargetPresentationComplete)
            {
                return UniTask.CompletedTask;
            }

            if (_targetPresentationCompletionSource != null)
            {
                return _targetPresentationCompletionSource.Task.AttachExternalCancellation(cancellationToken);
            }

            _targetPresentationCompletionSource = new UniTaskCompletionSource();

            return _targetPresentationCompletionSource.Task.AttachExternalCancellation(cancellationToken);
        }

        public void CompleteTargetPresentation(string entityId)
        {
            if (_currentPhase.Value == InteractionPhase.Idle)
            {
                return;
            }

            if (_currentTargetEntityId != entityId)
            {
                throw new InteractionPipelineTargetPresentationEntityMismatchException(entityId, _currentTargetEntityId);
            }

            _isTargetPresentationComplete = true;
            _targetPresentationCompletionSource?.TrySetResult();
        }

        public void EndInteraction()
        {
            _currentPhase.Value = InteractionPhase.Idle;
            _currentTargetEntityId = string.Empty;
            _isTargetPresentationComplete = false;
            _targetPresentationCompletionSource = null;
        }
    }
}
