namespace Core.Gameplay.Power
{
    public interface IPowerRegistry
    {
        string PlayerEntityId { get; }
        IPowerEntity Get(string entityId);
    }
}
