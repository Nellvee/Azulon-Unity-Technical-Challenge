using UnityEngine;
namespace Project.UI.Items
{
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
            _inventoryGrid.Inventory.TryRemoveItem(slot.BoundItem, 1);
        }
        private void ItemDatabase_OnSlotBound(InventorySlotUI slot, Project.Items.IItem item)
        {
            //Try to modify slot tooltip
            if (slot.TryGetComponent(out InventorySlotTooltipUI slotTooltip))
            {
                slotTooltip.OnRequestTooltipInfo += (item, lines) =>
                {
                    lines.Add($"<b>Try to click on item, to remove this Item from Player</b>");
                };
            }
        }
    }
}