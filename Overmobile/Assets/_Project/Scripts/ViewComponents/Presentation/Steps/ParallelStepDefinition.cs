using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class ParallelStepDefinition : PresentationStepDefinition
    {
        [SerializeReference] private PresentationStepDefinition[] _steps = Array.Empty<PresentationStepDefinition>();

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            if (_steps == null
             || _steps.Length == 0)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(ParallelStepDefinition),
                    "Steps are not assigned"
                );
            }

            UniTask[] stepTasks = new UniTask[_steps.Length];

            for (int i = 0; i < _steps.Length; i++)
            {
                PresentationStepDefinition step = _steps[i];

                if (step == null)
                {
                    throw new InvalidPresentationStepDefinitionException(
                        nameof(ParallelStepDefinition),
                        $"Step at index {i} is missing"
                    );
                }

                stepTasks[i] = step.ExecuteAsync(context, cancellationToken);
            }

            await UniTask.WhenAll(stepTasks);
        }
    }
}
