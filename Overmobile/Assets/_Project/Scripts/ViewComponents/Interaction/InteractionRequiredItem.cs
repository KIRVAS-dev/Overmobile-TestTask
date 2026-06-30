using Core.Gameplay.Inventory;
using UnityEngine;

namespace ViewComponents.Interaction
{
    [DisallowMultipleComponent]
    public sealed class InteractionRequiredItem : MonoBehaviour
    {
        public ItemType RequiredItem => _requiredItem;

        [SerializeField] private ItemType _requiredItem;
    }
}
