using System;
using Core.Input.Movement;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class CoreEntryPoint : IStartable, IDisposable
    {
        private readonly MovementInputHandler _movementInputHandler;

        public CoreEntryPoint(MovementInputHandler movementInputHandler)
        {
            _movementInputHandler = movementInputHandler;
        }

        void IStartable.Start()
        {
            _movementInputHandler.StartListening();
        }

        void IDisposable.Dispose()
        {
            _movementInputHandler.StopListening();
        }
    }
}
