using Project.Items;
using Project.UI.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.UI.Items
{
    [RequireComponent(typeof(InventorySlotUI))]
    public class InventorySlotTooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool _isHovering = false;
        private InventorySlotUI _inventorySlotUI;
        // Multiple scripts can subscribe to this to add lines to the tooltip
        public event Action<IItem, List<string>> OnRequestTooltipInfo;
        private void Awake()
        {
            TryGetComponent(out _inventorySlotUI);
            _inventorySlotUI.OnBoundItemChanged += InventorySlotUI_OnBoundItemChanged;
        }
        private void OnDestroy()
        {
            _inventorySlotUI.OnBoundItemChanged -= InventorySlotUI_OnBoundItemChanged;
        }

        private void InventorySlotUI_OnBoundItemChanged(IItem item)
        {
            OnRequestTooltipInfo = null;
            //If a slot is recycled while hovered, hide the tooltip
            if (item == null && _isHovering)
            {
                TooltipSystem.Hide();
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            if (_inventorySlotUI.BoundItem == null)
            {
                return;
            }

            // Create a "bucket" for all text chunks
            List<string> contentLines = new()
            {
                _inventorySlotUI.BoundItem.Data.Description
            };
            // Ask anyone listening to fill the bucket
            OnRequestTooltipInfo?.Invoke(_inventorySlotUI.BoundItem, contentLines);

            // Join everything with double newlines and show it
            string finalContent = string.Join("\n\n", contentLines);
            string title = string.IsNullOrEmpty(_inventorySlotUI.BoundItem.Data.DisplayName) ? 
                _inventorySlotUI.BoundItem.Data.Id : _inventorySlotUI.BoundItem.Data.DisplayName;
            TooltipSystem.Show(title, finalContent);            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipSystem.Hide();
            _isHovering = false;
        }
    }
}