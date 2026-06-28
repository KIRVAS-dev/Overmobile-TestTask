namespace Core.Gameplay.Movement
{
    public sealed class MovementRouteDisplayService
    {
        private readonly MovementModel _movementModel;
        private readonly IMovementRouteWaypointDisplay _waypointDisplay;

        public MovementRouteDisplayService(MovementModel movementModel, IMovementRouteWaypointDisplay waypointDisplay)
        {
            _movementModel = movementModel;
            _waypointDisplay = waypointDisplay;
        }

        public void ClearPreview()
        {
            _waypointDisplay.HideRoute();
        }

        public void PreviewRouteTo(string toEndpointKey)
        {
            _waypointDisplay.ShowRoute(_movementModel.CurrentEndpointKey, toEndpointKey);
        }

        public void OnMovementStarted()
        {
            _waypointDisplay.DisableWaypointAtRouteIndex(0);
        }

        public void OnMovementWaypointReached(int routeWaypointIndex)
        {
            _waypointDisplay.DisableWaypointAtRouteIndex(routeWaypointIndex);
        }

        public void OnMovementEnded()
        {
            _waypointDisplay.HideRoute();
        }
    }
}
