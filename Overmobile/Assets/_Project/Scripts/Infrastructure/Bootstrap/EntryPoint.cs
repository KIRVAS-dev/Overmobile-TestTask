using System.Threading;
using Core.Bootstrap;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    public sealed class EntryPoint : IStartable
    {
        const string CORE_SCENE_NAME = "Core";

        private readonly ISceneLoader _sceneLoader;

        public EntryPoint(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Start()
        {
            LoadCoreAsync().Forget();
        }

        private async UniTaskVoid LoadCoreAsync()
        {
            await _sceneLoader.LoadSceneAsync(CORE_SCENE_NAME, LoadSceneMode.Additive, CancellationToken.None);
        }
    }
}
