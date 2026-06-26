using System.Threading;
using Core.Bootstrap.Api;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class EntryPoint : IStartable
    {
        const string CORE_SCENE_NAME = "Core";

        readonly ISceneLoader sceneLoader;

        public EntryPoint(ISceneLoader sceneLoader)
        {
            this.sceneLoader = sceneLoader;
        }

        public void Start()
        {
            LoadCoreAsync().Forget();
        }

        async UniTaskVoid LoadCoreAsync()
        {
            await sceneLoader.LoadSceneAsync(CORE_SCENE_NAME, LoadSceneMode.Additive, CancellationToken.None);
        }
    }
}
