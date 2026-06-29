using Core;
using Core.Camera;
using Core.Gameplay.Player;
using Core.Input.Movement;
using Cysharp.Threading.Tasks;
using System;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint : IStartable, IDisposable
    {
        private readonly ICameraTransitionView _cameraTransitionView;
        private readonly IPlayerSpawnView _playerSpawnView;
        private readonly MovementInputHandler _movementInputHandler;
        private readonly CoreCancellationSource _coreCancellation;
        private readonly IGameplayInputBlock _gameplayInputBlock;

        public CoreEntryPoint(
            ICameraTransitionView cameraTransitionView,
            IPlayerSpawnView playerSpawnView,
            MovementInputHandler movementInputHandler,
            CoreCancellationSource coreCancellation,
            IGameplayInputBlock gameplayInputBlock)
        {
            _cameraTransitionView = cameraTransitionView;
            _playerSpawnView = playerSpawnView;
            _movementInputHandler = movementInputHandler;
            _coreCancellation = coreCancellation;
            _gameplayInputBlock = gameplayInputBlock;
        }

        void IStartable.Start()
        {
            _playerSpawnView.Spawn(0);
            StartIntroAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _movementInputHandler.StopListening();
        }

        private async UniTaskVoid StartIntroAsync()
        {
            _gameplayInputBlock.Block();

            try
            {
                await _cameraTransitionView.PlayTransitionAsync(_coreCancellation.Token);
            }
            finally
            {
                _gameplayInputBlock.Unblock();
            }

            _movementInputHandler.StartListening();
        }
    }
}
