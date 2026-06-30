using UnityEngine;

namespace ViewComponents.Power
{
    [DisallowMultipleComponent]
    public sealed class EntityPower : MonoBehaviour
    {
        public string EntityId => _entityId == null
            ? throw new MissingEntityPowerEntityIdReferenceException(gameObject.name)
            : _entityId.Id;

        public int InitialPower => _initialPower;

        [SerializeField] private ViewComponents.Entity.EntityId _entityId;
        [SerializeField] private int _initialPower;
    }
}
