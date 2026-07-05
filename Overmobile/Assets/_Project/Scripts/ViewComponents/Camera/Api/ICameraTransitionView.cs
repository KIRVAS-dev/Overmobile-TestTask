using Cysharp.Threading.Tasks;
using System.Threading;

namespace ViewComponents.Camera
{
    public interface ICameraTransitionView
    {
        UniTask PlayTransitionAsync(CancellationToken cancellationToken);
    }
}
