using System.Collections.Generic;

namespace Core.Input
{
    public interface IGameplayInputTargetProvider
    {
        IReadOnlyList<GameplayInputTarget> GetGameplayInputTargets();
    }
}
