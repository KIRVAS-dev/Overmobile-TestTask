using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class DelayStepDefinition : PresentationStepDefinition
    {
        private const float MillisecondsPerSecond = 1000f;

        [SerializeField] private float _durationSeconds;

        public override async UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            if (_durationSeconds <= 0f)
            {
                return;
            }

            int delayMilliseconds = Mathf.RoundToInt(_durationSeconds * MillisecondsPerSecond);

            await UniTask.Delay(delayMilliseconds, cancellationToken: cancellationToken);
        }
    }
}
