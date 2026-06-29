using Core.Gameplay.Inventory;
using UnityEngine;

namespace ViewComponents.Inventory
{
    public sealed class Item
        : MonoBehaviour,
          IItem
    {
        public ItemType ItemType => _item;

        [SerializeField] private ItemType _item;
    }
}
