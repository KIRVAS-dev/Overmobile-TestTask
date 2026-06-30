using System.Collections.Generic;

namespace Core.Gameplay.Power
{
    public interface IEntityPowerProvider
    {
        string PlayerEntityId { get; }
        IReadOnlyList<EntityPowerData> GetEntityPowers();
    }
}
