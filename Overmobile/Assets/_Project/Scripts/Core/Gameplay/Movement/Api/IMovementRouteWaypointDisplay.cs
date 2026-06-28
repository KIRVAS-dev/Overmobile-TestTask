namespace Core.Gameplay.Movement
{
    public interface IMovementRouteWaypointDisplay
    {
        void ShowRoute(string fromEndpointKey, string toEndpointKey);
        void HideRoute();
        void DisableWaypointAtRouteIndex(int routeWaypointIndex);
    }
}
