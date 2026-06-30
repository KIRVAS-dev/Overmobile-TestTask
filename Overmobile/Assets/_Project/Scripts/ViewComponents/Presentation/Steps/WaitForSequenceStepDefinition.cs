using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class WaitForSequenceStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private PresentationStepSequence _sequence;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            if (_sequence == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(WaitForSequenceStepDefinition),
                    "Sequence is not assigned"
                );
            }

            await _sequence.StartPresentationAsync(cancellationToken);
        }
    }
}
