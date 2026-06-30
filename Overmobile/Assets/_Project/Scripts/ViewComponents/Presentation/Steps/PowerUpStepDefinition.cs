using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class PowerUpStepDefinition : PresentationStepDefinition
    {
        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            context.ActivePresentationSectionMapProvider.PlaySection(PresentationSectionKey.PowerUp);

            return UniTask.CompletedTask;
        }
    }
}
