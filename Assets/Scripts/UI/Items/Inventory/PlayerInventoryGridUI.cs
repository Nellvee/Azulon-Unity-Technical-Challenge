using Project.Items;
using Project.Items.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Project.UI.Items
{
    /// <summary>
    /// Simple inventory UI
    /// </summary>
    public class PlayerInventoryGridUI : VirtualizedGridUI<IItem>
    {
        public event Action<InventorySlotUI> OnSlotItemClicked;

        private IInventory _inventory;
        public IInventory Inventory => _inventory;
        public void Initialize(IInventory inventory)
        {
            if (_inventory != null) _inventory.OnInventoryChanged -= HandleInventoryChanged;

            _inventory = inventory;
            _inventory.OnInventoryChanged += HandleInventoryChanged;

            SetData(_inventory.Items);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_inventory != null) _inventory.OnInventoryChanged -= HandleInventoryChanged;
        }

        private void HandleInventoryChanged(IInventory inv)
        {
            SetData(_inventory.Items);
            //RebuildGrid();
        }

        protected override void BindSlot(InventorySlotUI slot, IItem data)
        {
            slot.Bind(data);
            slot.OnSlotClicked += Slot_OnSlotClicked;

            //Try to modify slot tooltip
            if (slot.TryGetComponent(out InventorySlotTooltipUI slotTooltip))
            {
                // Unsubscribe first to prevent duplicates when the slot is recycled
                slotTooltip.OnRequestTooltipInfo -= HandleTooltipInfo;
                slotTooltip.OnRequestTooltipInfo += HandleTooltipInfo;
            }
        }
        protected override void UnbindSlot(InventorySlotUI slot)
        {
            slot.OnSlotClicked -= Slot_OnSlotClicked;
            slot.Unbind();
        }
        private void Slot_OnSlotClicked(InventorySlotUI slot)
        {
            OnSlotItemClicked?.Invoke(slot);
        }
        // Named method instead of lambda
        private void HandleTooltipInfo(IItem item, List<string> lines)
        {
            lines.Add($"<b>count:</b> {item.Count}");
        }
    }
}