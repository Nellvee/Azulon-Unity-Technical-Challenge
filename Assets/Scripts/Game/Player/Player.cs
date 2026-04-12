using Project.Items;
using Project.Items._Inventory;
using System;
using UnityEngine;
namespace Project.Characters
{
    /// <summary>
    /// Simple player with inventory.
    /// </summary>
    public class Player : MonoBehaviour
    {

        // ──────────────────────────────
        // Events
        // ──────────────────────────────

        public event Action<Player> OnInitialized;

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        private IInventory _inventory;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────
        public bool IsInitialized { get; private set; } = false;
        public IInventory Inventory => _inventory;

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────

        private void Awake()
        {
            _inventory = new Inventory(gameObject, new AddressableItemFactory(), 200);
            _inventory.OnItemAdded += Inventory_OnItemAdded;
            _inventory.OnItemRemoved += Inventory_OnItemRemoved;

            IsInitialized = true;
            OnInitialized?.Invoke(this);
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


        // ──────────────────────────────
        // Editor Test
        // ──────────────────────────────
        #region Editor
#if UNITY_EDITOR
        [ContextMenu("ShowAllInventory")]
        public void ShowAllInventory()
        {
            Debug.Log($"Player: Items in inventory: {_inventory.Items.Count}");
            if (_inventory.Items.Count > 0)
            {
                Debug.Log("Inventory Log started");
                foreach (IItem item in _inventory.Items)
                {
                    Debug.Log($"Item in inventory: {item}");
                }
                Debug.Log("Inventory Log stopped");
            }
        }
#endif
        #endregion

    }
}