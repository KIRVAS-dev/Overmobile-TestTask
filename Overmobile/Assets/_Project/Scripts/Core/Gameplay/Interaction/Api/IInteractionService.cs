using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Core.Gameplay.Interaction
{
    public interface IInteractionService
    {
        bool CanInteract(string entityId);

        UniTask InteractAsync(string endpointKey, string entityId, Vector3 facingWorldPosition,
            CancellationToken cancellationToken);
    }
}
