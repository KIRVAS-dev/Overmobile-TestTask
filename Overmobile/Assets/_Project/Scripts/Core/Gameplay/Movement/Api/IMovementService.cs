using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Gameplay.Movement
{
    public interface IMovementService
    {
        bool IsMoving { get; }

        string CurrentEndpointKey { get; }

        UniTask MoveToAsync(string toEndpointKey, CancellationToken cancellationToken);
    }
}
