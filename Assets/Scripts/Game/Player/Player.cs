using Project.Items;
using Project.Items._Inventory;
using UnityEngine;
namespace Project.Characters
{
    /// <summary>
    /// Simple player with inventory.
    /// </summary>
    public class Player : MonoBehaviour
    {

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        private IInventory _inventory;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        public IInventory Inventory => _inventory;

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────

        private void Awake()
        {
            _inventory = new Inventory(new AddressableItemFactory(), 200);
            _inventory.OnItemAdded += Inventory_OnItemAdded;
            _inventory.OnItemRemoved += Inventory_OnItemRemoved;
        }

        private async void Start()
        {
            for (int i = 1; i < 6; i++)
            {
                _ = _inventory.TryAddItemById($"new_item_0{i}", i);
            }
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────
        private void Inventory_OnItemRemoved(IItem item)
        {
            Debug.Log($"Player.Inventory.OnItemRemoved: {item}");
        }

        private void Inventory_OnItemAdded(IItem item)
        {
            Debug.Log($"Player.Inventory.OnItemAdded: {item}");
        }
    }
}