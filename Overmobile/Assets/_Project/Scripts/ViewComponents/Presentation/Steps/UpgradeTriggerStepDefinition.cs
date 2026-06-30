using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class UpgradeTriggerStepDefinition : PresentationStepDefinition
    {
        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            context.PlayerUpgradeService.Upgrade();

            return UniTask.CompletedTask;
        }
    }
}
