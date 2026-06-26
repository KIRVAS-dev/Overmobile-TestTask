namespace Core.Gameplay.Movement
{
    public sealed class MovementModel
    {
        public bool IsMoving { get; private set; }

        public string CurrentEndpointKey { get; private set; }

        public void SetMoving(bool isMoving)
        {
            IsMoving = isMoving;
        }

        public void SetCurrentEndpointKey(string endpointKey)
        {
            CurrentEndpointKey = endpointKey;
        }
    }
}
