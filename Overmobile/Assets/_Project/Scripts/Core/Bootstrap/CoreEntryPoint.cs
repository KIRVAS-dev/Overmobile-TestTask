using System;
using Core.Gameplay.Player;
using Core.Input.Movement;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint : IStartable, IDisposable
    {
        private readonly IPlayerSpawnView _playerSpawnView;
        private readonly MovementInputHandler _movementInputHandler;

        public CoreEntryPoint(
            IPlayerSpawnView playerSpawnView,
            MovementInputHandler movementInputHandler)
        {
            _playerSpawnView = playerSpawnView;
            _movementInputHandler = movementInputHandler;
        }

        void IStartable.Start()
        {
            _playerSpawnView.Spawn(0);
            _movementInputHandler.StartListening();
        }

        void IDisposable.Dispose()
        {
            _movementInputHandler.StopListening();
        }
    }
}
