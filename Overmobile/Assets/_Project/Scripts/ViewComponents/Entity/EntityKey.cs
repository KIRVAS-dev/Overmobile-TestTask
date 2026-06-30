using UnityEngine;

namespace ViewComponents.Entity
{
    [DisallowMultipleComponent]
    public sealed class EntityKey : MonoBehaviour
    {
        public string Key => string.IsNullOrWhiteSpace(_key)
            ? throw new MissingEntityKeyException(gameObject.name)
            : _key;

        [SerializeField] private string _key;
    }
}
