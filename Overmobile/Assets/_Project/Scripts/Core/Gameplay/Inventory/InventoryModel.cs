using System.Collections.Generic;

namespace Core.Gameplay.Inventory
{
    public sealed class InventoryModel
    {
        private readonly HashSet<ItemType> _items = new HashSet<ItemType>();

        public bool HasItem(ItemType itemType)
        {
            return _items.Contains(itemType);
        }

        public void Add(ItemType itemType)
        {
            _items.Add(itemType);
        }

        public void Remove(ItemType itemType)
        {
            _items.Remove(itemType);
        }
    }
}
