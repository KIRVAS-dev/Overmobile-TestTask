using System.Collections.Generic;

namespace Core.Input.Movement
{
    public interface IMovementInputTargetProvider
    {
        IReadOnlyList<MovementInputTarget> GetInputTargets();
    }
}
