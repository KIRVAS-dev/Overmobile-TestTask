using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Core.Gameplay.Movement
{
    public interface IMovementService
    {
        bool IsMoving { get; }

        UniTask MoveToAsync(string toEndpointKey, Vector3 destinationFacingWorldPosition, CancellationToken cancellationToken);
    }
}
