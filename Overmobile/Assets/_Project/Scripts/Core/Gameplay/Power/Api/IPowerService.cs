namespace Core.Gameplay.Power
{
    public interface IPowerService
    {
        bool TryTransferPowerToPlayer(string sourceEntityId, bool requirePlayerPowerGreater);
    }
}
