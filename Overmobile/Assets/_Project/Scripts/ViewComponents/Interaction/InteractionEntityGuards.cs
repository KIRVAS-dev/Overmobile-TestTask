using System.Collections.Generic;
using UnityEngine;
using EntityId = ViewComponents.Entity.EntityId;

namespace ViewComponents.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractionEntityGuards : MonoBehaviour
    {
        [SerializeField] private EntityId[] _guardEntityIds;

        public IReadOnlyList<string> GetGuardEntityIds(string hostEntityId)
        {
            if (_guardEntityIds == null
             || _guardEntityIds.Length == 0)
            {
                throw new InvalidInteractionEntityGuardsException(gameObject.name, "Guard entity ids are not assigned");
            }

            List<string> guardEntityIds = new List<string>(_guardEntityIds.Length);
            HashSet<string> uniqueGuardEntityIds = new HashSet<string>();

            for (int i = 0; i < _guardEntityIds.Length; i++)
            {
                EntityId guardEntityId = _guardEntityIds[i];

                if (guardEntityId == null)
                {
                    throw new InvalidInteractionEntityGuardsException(
                        gameObject.name,
                        $"Guard entity id at index {i} is missing"
                    );
                }

                string guardId = guardEntityId.Id;

                if (guardId == hostEntityId)
                {
                    throw new InvalidInteractionEntityGuardsException(
                        gameObject.name,
                        "Guard entity id cannot reference the host entity"
                    );
                }

                if (!uniqueGuardEntityIds.Add(guardId))
                {
                    throw new InvalidInteractionEntityGuardsException(gameObject.name, $"Duplicate guard entity id '{guardId}'");
                }

                guardEntityIds.Add(guardId);
            }

            return guardEntityIds;
        }
    }
}
