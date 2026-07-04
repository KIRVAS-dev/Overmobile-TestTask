using Core.Gameplay.Interaction;

namespace ViewComponents.TargetSelection
{
    public static class TargetSelectionHighlightRules
    {
        public static string ResolveHighlightEntityId(
            InteractionPhase phase,
            string interactionTargetEntityId,
            string hoveredEntityId)
        {
            if (phase != InteractionPhase.Idle
             && !string.IsNullOrEmpty(interactionTargetEntityId))
            {
                return interactionTargetEntityId;
            }

            return hoveredEntityId;
        }

        public static bool IsHighlightVisible(
            InteractionPhase phase,
            string interactionTargetEntityId,
            string entityId,
            bool isResolved,
            bool areGuardsBlocking,
            bool isBlocked)
        {
            if (areGuardsBlocking)
            {
                return false;
            }

            bool isInteractionTarget = phase != InteractionPhase.Idle
             && !string.IsNullOrEmpty(interactionTargetEntityId)
             && interactionTargetEntityId == entityId;

            if (isInteractionTarget)
            {
                return true;
            }

            return !isResolved && phase == InteractionPhase.Idle && !isBlocked;
        }
    }
}
