using ExtendedExceptions;

namespace Core.Gameplay.Inventory
{
    public sealed class InvalidItemException : ExtendedException
    {
        public InvalidItemException(ItemType itemType)
            : base("inventory-1", $"Invalid item type '{itemType}'") { }
    }

    public sealed class ItemAlreadyInInventoryException : ExtendedException
    {
        public ItemAlreadyInInventoryException(ItemType itemType)
            : base("inventory-2", $"Item type '{itemType}' is already in inventory") { }
    }

    public sealed class ItemNotInInventoryException : ExtendedException
    {
        public ItemNotInInventoryException(ItemType itemType)
            : base("inventory-3", $"Item type '{itemType}' is not in inventory") { }
    }
}
