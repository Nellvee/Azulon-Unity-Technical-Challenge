using Project.Items;
using Project.Items._Inventory;
using System.Collections.Generic;
using UnityEngine;
namespace Project.UI.Items
{
    /// <summary>
    /// Simply remove item when clicked.
    /// UPD: Was modified to also use item before removing
    /// </summary>
    [RequireComponent(typeof(PlayerInventoryGridUI))]
    public class PlayerInventoryGridUIItemRemover : MonoBehaviour
    {
        private PlayerInventoryGridUI _inventoryGrid;

        private void Awake()
        {
            TryGetComponent(out _inventoryGrid);
        }
        private void Start()
        {
            _inventoryGrid.OnSlotItemClicked += ItemDatabase_OnSlotItemClicked;
            _inventoryGrid.OnSlotBound += ItemDatabase_OnSlotBound;
        }
        private void OnDestroy()
        {
            _inventoryGrid.OnSlotBound -= ItemDatabase_OnSlotBound;
            _inventoryGrid.OnSlotItemClicked -= ItemDatabase_OnSlotItemClicked;
        }

        private void ItemDatabase_OnSlotItemClicked(InventorySlotUI slot)
        {
            //cache item
            IItem targetItem = slot.BoundItem;
            /// UPD: Was modified to also use item before removing
            if (_inventoryGrid.Inventory is Inventory inventory)
            {
                inventory.UseItem(targetItem);
            }
            _inventoryGrid.Inventory.TryRemoveItem(targetItem, 1);
        }
        private void ItemDatabase_OnSlotBound(InventorySlotUI slot, Project.Items.IItem item)
        {
            //Try to modify slot tooltip
            if (slot.TryGetComponent(out InventorySlotTooltipUI slotTooltip))
            {
                slotTooltip.OnRequestTooltipInfo -= HandleRemoverTooltip;
                slotTooltip.OnRequestTooltipInfo += HandleRemoverTooltip;
            }
        }
        private void HandleRemoverTooltip(IItem item, List<string> lines)
        {
            lines.Add($"<b>Try to click on item, to remove this Item from Player</b>");
        }
    }
}