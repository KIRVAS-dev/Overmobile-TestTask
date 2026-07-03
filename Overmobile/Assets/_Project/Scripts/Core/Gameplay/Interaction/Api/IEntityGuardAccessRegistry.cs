using R3;

namespace Core.Gameplay.Interaction
{
    public interface IEntityGuardAccessRegistry
    {
        bool HasGuards(string entityId);

        ReadOnlyReactiveProperty<bool> GetAreGuardsBlocking(string entityId);
    }
}
