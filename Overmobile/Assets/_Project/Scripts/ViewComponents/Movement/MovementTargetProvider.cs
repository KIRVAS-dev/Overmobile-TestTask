using Core.Input.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementTargetProvider
        : MonoBehaviour,
          IMovementInputTargetProvider
    {
        [SerializeField] private MovementTarget[] _movementTargets;

        public IReadOnlyList<MovementInputTarget> GetInputTargets()
        {
            ValidateMovementTargets();

            List<MovementInputTarget> inputTargets = new List<MovementInputTarget>(_movementTargets.Length);

            foreach (MovementTarget movementTarget in _movementTargets)
            {
                inputTargets.Add(new MovementInputTarget(movementTarget.EndpointKey, movementTarget.PointArea.PointerUp));
            }

            return inputTargets;
        }

        private void ValidateMovementTargets()
        {
            if (_movementTargets == null
             || _movementTargets.Length == 0)
            {
                throw new InvalidMovementTargetViewException(gameObject.name, "Movement targets are not assigned");
            }

            for (int i = 0; i < _movementTargets.Length; i++)
            {
                if (_movementTargets[i] == null)
                {
                    throw new InvalidMovementTargetViewException(gameObject.name, $"Movement target at index {i} is missing");
                }
            }
        }
    }
}
