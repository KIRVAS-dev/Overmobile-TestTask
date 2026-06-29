using Core.Gameplay.Movement;
using Input;
using System;
using UnityEngine;

namespace Core.Input
{
    public sealed class PlayerPointerInputFilter
        : IPlayerPointerInput,
          IDisposable
    {
        public Vector2 ScreenPosition => _source.ScreenPosition;
        public bool IsPressed => IsInputAllowed && _source.IsPressed;

        private readonly PlayerPointerInput _source;
        private readonly IMovementService _movementService;

        public PlayerPointerInputFilter(
            PlayerPointerInput source,
            IMovementService movementService)
        {
            _source = source;
            _movementService = movementService;

            _source.Pressed += OnSourcePressed;
            _source.Released += OnSourceReleased;
        }

        public event Action Pressed;
        public event Action Released;

        private bool IsInputAllowed => !_movementService.IsMoving;

        void IDisposable.Dispose()
        {
            _source.Pressed -= OnSourcePressed;
            _source.Released -= OnSourceReleased;
        }

        private void OnSourcePressed()
        {
            if (!IsInputAllowed)
            {
                return;
            }

            Pressed?.Invoke();
        }

        private void OnSourceReleased()
        {
            if (!IsInputAllowed)
            {
                return;
            }

            Released?.Invoke();
        }
    }
}
