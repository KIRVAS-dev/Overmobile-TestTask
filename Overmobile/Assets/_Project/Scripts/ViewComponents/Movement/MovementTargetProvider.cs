using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Movement
{
    public sealed class MovementTargetProvider : MonoBehaviour
    {
        [SerializeField] private MovementTarget[] _movementTargets;

        public IReadOnlyList<MovementTarget> GetTargets()
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

            return _movementTargets;
        }
    }
}
