using Input;
using UnityEngine;

namespace Core.Input.Movement
{
    public readonly struct MovementInputTarget
    {
        public string EndpointKey { get; }
        public ITrigger PointerDown { get; }
        public ITrigger PointerExit { get; }
        public ITrigger PointerEnter { get; }
        public ITrigger PointerUp { get; }
        public Vector3 FacingWorldPosition { get; }

        public MovementInputTarget(
            string endpointKey,
            ITrigger pointerDown,
            ITrigger pointerExit,
            ITrigger pointerEnter,
            ITrigger pointerUp,
            Vector3 facingWorldPosition)
        {
            EndpointKey = endpointKey;
            PointerDown = pointerDown;
            PointerExit = pointerExit;
            PointerEnter = pointerEnter;
            PointerUp = pointerUp;
            FacingWorldPosition = facingWorldPosition;
        }
    }
}
