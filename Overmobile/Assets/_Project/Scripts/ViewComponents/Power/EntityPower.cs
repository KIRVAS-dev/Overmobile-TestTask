using UnityEngine;

namespace ViewComponents.Power
{
    [DisallowMultipleComponent]
    public sealed class EntityPower : MonoBehaviour
    {
        public string PowerId => string.IsNullOrWhiteSpace(_powerId)
            ? throw new MissingPowerIdException(gameObject.name)
            : _powerId;

        public int InitialPower => _initialPower;

        [SerializeField] private string _powerId;
        [SerializeField] private int _initialPower;

        public void SavePowerId()
        {
            _powerId = gameObject.GetEntityId().ToString();
        }

        public void ClearPowerId()
        {
            _powerId = string.Empty;
        }
    }
}
