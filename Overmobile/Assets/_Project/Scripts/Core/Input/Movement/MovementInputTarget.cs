using Input;
using UnityEngine;

namespace Core.Input.Movement
{
    public readonly struct MovementInputTarget
    {
        public string EndpointKey { get; }

        public ITrigger PointerUp { get; }

        public Vector3 FacingWorldPosition { get; }

        public MovementInputTarget(
            string endpointKey,
            ITrigger pointerUp,
            Vector3 facingWorldPosition)
        {
            EndpointKey = endpointKey;
            PointerUp = pointerUp;
            FacingWorldPosition = facingWorldPosition;
        }
    }
}
