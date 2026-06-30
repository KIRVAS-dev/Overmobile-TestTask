using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class PlaySequenceStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private PresentationStepSequence _sequence;

        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            if (_sequence == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(PlaySequenceStepDefinition),
                    "Sequence is not assigned"
                );
            }

            _sequence.StartPresentationFireAndForget(cancellationToken);

            return UniTask.CompletedTask;
        }
    }
}
