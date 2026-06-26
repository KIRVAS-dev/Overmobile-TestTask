using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Bootstrap.Api
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, CancellationToken cancellationToken);
    }
}
