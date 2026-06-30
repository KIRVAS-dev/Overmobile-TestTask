using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ViewComponents.Presentation
{
    [Serializable]
    public abstract class PresentationStepDefinition
    {
        public abstract UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken);
    }
}
