using Core.Gameplay.Entity;
using UnityEngine;

namespace ViewComponents.Entity
{
    [DisallowMultipleComponent]
    public sealed class EntityRole : MonoBehaviour
    {
        public EntityType Type => _type;

        [SerializeField] private EntityType _type;
    }
}
