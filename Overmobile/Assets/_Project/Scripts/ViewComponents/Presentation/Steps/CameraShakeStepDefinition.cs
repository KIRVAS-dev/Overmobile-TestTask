using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class CameraShakeStepDefinition : PresentationStepDefinition
    {
        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            context.CameraShakeView.PlayShake();

            return UniTask.CompletedTask;
        }
    }
}
