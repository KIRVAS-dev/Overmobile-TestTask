using System.Collections.Generic;

namespace Core.Gameplay.Interaction
{
    public interface IInteractableTargetProvider
    {
        IReadOnlyList<InteractableTargetData> GetInteractableTargets();
        InteractableTargetData GetTargetByEntityKey(string entityKey);
        InteractableTargetData GetTargetByPowerId(string powerId);
    }
}
