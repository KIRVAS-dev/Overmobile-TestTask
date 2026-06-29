using Core.Camera;
using Core.Gameplay.Player;
using Core.Input.Movement;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint : IStartable, IDisposable
    {
        private readonly ICameraTransitionView _cameraTransitionView;
        private readonly IPlayerSpawnView _playerSpawnView;
        private readonly MovementInputHandler _movementInputHandler;

        public CoreEntryPoint(
            ICameraTransitionView cameraTransitionView,
            IPlayerSpawnView playerSpawnView,
            MovementInputHandler movementInputHandler)
        {
            _cameraTransitionView = cameraTransitionView;
            _playerSpawnView = playerSpawnView;
            _movementInputHandler = movementInputHandler;
        }

        void IStartable.Start()
        {
            _playerSpawnView.Spawn(0);
            _movementInputHandler.StartListening();

            StartCameraTransitionAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _movementInputHandler.StopListening();
        }

        private async UniTaskVoid StartCameraTransitionAsync()
        {
            await _cameraTransitionView.PlayTransitionAsync(CancellationToken.None);
        }
    }
}
