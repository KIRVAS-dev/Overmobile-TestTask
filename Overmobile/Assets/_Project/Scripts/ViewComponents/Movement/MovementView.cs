using Core.Gameplay.Movement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementView
        : MonoBehaviour,
          IMovementView
    {
        public async UniTask MoveAlongPathAsync(IReadOnlyList<Vector3> pathPoints, float moveSpeed,
            CancellationToken cancellationToken)
        {
            Vector3[] path = pathPoints.ToArray();
            float duration = CalculatePathLength(path) / moveSpeed;

            Tween tween = transform.DOPath(path, duration, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);

            await AwaitTweenAsync(tween, cancellationToken);
        }

        private async UniTask AwaitTweenAsync(Tween tween, CancellationToken cancellationToken)
        {
            CancellationTokenRegistration registration = cancellationToken.Register(() =>
                {
                    if (tween.IsActive())
                    {
                        tween.Kill();
                    }
                }
            );

            try
            {
                while (tween.IsActive())
                {
                    await UniTask.Yield();
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                registration.Dispose();
            }
        }

        private float CalculatePathLength(IReadOnlyList<Vector3> path)
        {
            float length = 0f;
            Vector3 previous = transform.position;

            for (int i = 0; i < path.Count; i++)
            {
                length += Vector3.Distance(previous, path[i]);
                previous = path[i];
            }

            return length;
        }
    }
}
