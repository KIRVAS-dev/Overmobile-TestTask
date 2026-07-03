using ExtendedExceptions;
using UnityEngine;
using EntityId = ViewComponents.Entity.EntityId;

namespace ViewComponents.Power
{
    [DisallowMultipleComponent]
    public sealed class EntityPower : MonoBehaviour
    {
        [SerializeField] private EntityId _entityId;
        [SerializeField] private int _initialPower;

        public string EntityId => _entityId.Id;
        public int InitialPower => _initialPower;

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNull(_entityId, () => new MissingEntityPowerFieldException(nameof(_entityId), gameObject.name));
        }
    }
}
