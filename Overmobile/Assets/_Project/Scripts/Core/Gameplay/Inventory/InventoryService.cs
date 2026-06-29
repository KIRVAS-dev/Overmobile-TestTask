using System;

namespace Core.Gameplay.Inventory
{
    public sealed class InventoryService : IInventory
    {
        private readonly InventoryModel _inventoryModel;

        public InventoryService(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
        }

        public void Add(ItemType itemType)
        {
            ValidateItemType(itemType);

            if (_inventoryModel.HasItem(itemType))
            {
                throw new ItemAlreadyInInventoryException(itemType);
            }

            _inventoryModel.Add(itemType);
        }

        public void Remove(ItemType itemType)
        {
            ValidateItemType(itemType);

            if (!_inventoryModel.HasItem(itemType))
            {
                throw new ItemNotInInventoryException(itemType);
            }

            _inventoryModel.Remove(itemType);
        }

        public bool Get(ItemType itemType)
        {
            ValidateItemType(itemType);

            return _inventoryModel.HasItem(itemType);
        }

        private void ValidateItemType(ItemType itemType)
        {
            if (!Enum.IsDefined(typeof(ItemType), itemType))
            {
                throw new InvalidItemException(itemType);
            }
        }
    }
}
