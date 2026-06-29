namespace Core.Gameplay.Inventory
{
    public interface IInventory
    {
        void Add(ItemType itemType);
        void Remove(ItemType itemType);
        bool Get(ItemType itemType);
    }
}
