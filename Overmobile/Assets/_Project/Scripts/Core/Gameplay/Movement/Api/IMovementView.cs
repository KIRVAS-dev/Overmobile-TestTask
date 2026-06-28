using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public interface IMovementView
    {
        UniTask MoveAlongPathAsync(IReadOnlyList<Vector3> pathPoints, float moveSpeed, float facingRotationDuration,
            Vector3 destinationFacingWorldPosition, Action<int> onWaypointReached);

        UniTask FaceTowardAsync(Vector3 destinationFacingWorldPosition, float facingRotationDuration);
    }
}
