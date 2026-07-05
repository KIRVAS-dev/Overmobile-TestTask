using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class DeathTriggerStepDefinition : PresentationStepDefinition
    {
        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            context.ActivePresentationSectionMapProvider.PlaySection(PresentationSectionKey.Death);

            return UniTask.CompletedTask;
        }
    }
}
