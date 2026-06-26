using Input;

namespace Core.Input.Movement
{
    public readonly struct MovementInputTarget
    {
        public string EndpointKey { get; }

        public ITrigger PointerUp { get; }

        public MovementInputTarget(string endpointKey, ITrigger pointerUp)
        {
            EndpointKey = endpointKey;
            PointerUp = pointerUp;
        }
    }
}
