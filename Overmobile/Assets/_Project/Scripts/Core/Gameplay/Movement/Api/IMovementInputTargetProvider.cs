using System.Collections.Generic;

namespace Core.Gameplay.Movement
{
    public interface IMovementInputTargetProvider
    {
        IReadOnlyList<MovementInputTarget> GetInputTargets();
    }
}
