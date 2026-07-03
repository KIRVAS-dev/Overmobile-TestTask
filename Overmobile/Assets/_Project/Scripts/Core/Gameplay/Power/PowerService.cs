namespace Core.Gameplay.Power
{
    public sealed class PowerService : IPowerService
    {
        private readonly PowerRegistry _powerRegistry;

        public PowerService(PowerRegistry powerRegistry)
        {
            _powerRegistry = powerRegistry;
        }

        public bool TryTransferPowerToPlayer(string sourceEntityId, bool requirePlayerPowerGreater)
        {
            PowerEntityModel source = _powerRegistry.GetModel(sourceEntityId);

            if (source.IsResolved.CurrentValue)
            {
                return false;
            }

            PowerEntityModel player = _powerRegistry.GetModel(_powerRegistry.PlayerEntityId);

            if (requirePlayerPowerGreater && player.Power.CurrentValue <= source.Power.CurrentValue)
            {
                return false;
            }

            player.AddPower(source.Power.CurrentValue);
            source.MarkResolved();

            return true;
        }
    }
}
