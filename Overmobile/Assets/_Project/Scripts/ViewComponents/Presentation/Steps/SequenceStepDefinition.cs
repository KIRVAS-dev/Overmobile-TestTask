using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class SequenceStepDefinition : PresentationStepDefinition
    {
        [SerializeReference] private PresentationStepDefinition[] _steps = Array.Empty<PresentationStepDefinition>();

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            if (_steps == null
             || _steps.Length == 0)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(SequenceStepDefinition),
                    "Steps are not assigned"
                );
            }

            for (int i = 0; i < _steps.Length; i++)
            {
                PresentationStepDefinition step = _steps[i];

                if (step == null)
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(SequenceStepDefinition),
                        $"Step at index {i} is missing"
                    );
                }

                await step.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}
