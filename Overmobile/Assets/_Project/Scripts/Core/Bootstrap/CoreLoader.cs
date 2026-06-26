using System.Threading;
using Core.Bootstrap.Api;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using LoadSceneMode = Core.Bootstrap.Api.LoadSceneMode;

namespace Core.Bootstrap
{
    public sealed class CoreLoader : ISceneLoader
    {
        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, CancellationToken cancellationToken)
        {
            UnityEngine.SceneManagement.LoadSceneMode unityLoadSceneMode = ConvertLoadSceneMode(loadSceneMode);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, unityLoadSceneMode);

            await loadOperation.ToUniTask(cancellationToken: cancellationToken);
        }

        UnityEngine.SceneManagement.LoadSceneMode ConvertLoadSceneMode(LoadSceneMode loadSceneMode)
        {
            switch (loadSceneMode)
            {
                case LoadSceneMode.Single:
                    return UnityEngine.SceneManagement.LoadSceneMode.Single;

                case LoadSceneMode.Additive:
                    return UnityEngine.SceneManagement.LoadSceneMode.Additive;

                default:
                    throw new UnhandledLoadSceneModeException(loadSceneMode);
            }
        }
    }
}
