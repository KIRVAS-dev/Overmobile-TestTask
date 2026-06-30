using UnityEngine;
using ViewComponents.Inventory;

namespace ViewComponents.Interaction
{
    [DisallowMultipleComponent]
    public sealed class Drop : MonoBehaviour
    {
        public Item LootItemPrefab => _lootItemPrefab;

        [SerializeField] private Item _lootItemPrefab;
    }
}
