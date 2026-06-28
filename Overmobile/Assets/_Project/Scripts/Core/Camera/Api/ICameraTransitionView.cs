using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Camera
{
    public interface ICameraTransitionView
    {
        UniTask PlayTransitionAsync(CancellationToken cancellationToken);
    }
}
