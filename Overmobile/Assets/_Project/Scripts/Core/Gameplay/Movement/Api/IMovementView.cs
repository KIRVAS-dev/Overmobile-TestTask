using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public interface IMovementView
    {
        UniTask MoveAlongPathAsync(IReadOnlyList<Vector3> pathPoints, float moveSpeed, float facingRotationDuration,
            Vector3 destinationFacingWorldPosition, CancellationToken cancellationToken);

        UniTask FaceTowardAsync(Vector3 destinationFacingWorldPosition, float facingRotationDuration,
            CancellationToken cancellationToken);
    }
}
