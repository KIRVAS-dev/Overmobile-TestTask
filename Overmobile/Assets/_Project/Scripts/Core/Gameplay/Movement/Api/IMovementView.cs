using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public interface IMovementView
    {
        UniTask MoveAlongPathAsync(IReadOnlyList<Vector3> pathPoints, float moveSpeed, CancellationToken cancellationToken);
    }
}
