using UnityEngine;

namespace ViewComponents.Entity
{
    [DisallowMultipleComponent]
    public sealed class EntityId : MonoBehaviour
    {
        public string Id => string.IsNullOrWhiteSpace(_id)
            ? throw new MissingEntityIdException(gameObject.name)
            : _id;

        [SerializeField] private string _id;

        public void SaveId()
        {
            _id = gameObject.GetEntityId().ToString();
        }

        public void ClearId()
        {
            _id = string.Empty;
        }
    }
}
