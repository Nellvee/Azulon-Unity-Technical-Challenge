using Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
namespace Project.UI.Items
{
    /// <summary>
    /// Loads all items from Addressables database
    /// </summary>
    public class ItemDatabaseGridUI : VirtualizedGridUI<IItemData>
    {
        public event Action<InventorySlotUI> OnSlotItemClicked;
        
        private List<IItemData> _allItemTemplates = new();

        /// <summary>
        /// Loads all items from Addressables using a specific label.
        /// </summary>
        public async void LoadDatabase(string label = "ItemData")
        {
            var handle = Addressables.LoadAssetsAsync<ItemDataSO>(label, null);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                _allItemTemplates = new List<IItemData>(handle.Result);
                _allItemTemplates = _allItemTemplates.OrderBy(x => x.Id).ToList();
                SetData(_allItemTemplates);
            }
        }

        protected override void BindSlot(InventorySlotUI slot, IItemData data)
        {
            // Create a temporary 'display' item so the slot can render it
            // This is efficient because the 'Item' is just a lightweight class wrapper
            IItem displayItem = new Item(data, 1);
            slot.Bind(displayItem);

            slot.OnSlotClicked += Slot_OnSlotClicked;

            //Try to modify slot tooltip
            if(slot.TryGetComponent(out InventorySlotTooltipUI slotTooltip))
            {
                slotTooltip.OnRequestTooltipInfo += (item, lines) =>
                {
                    string databaseText = $"<color=#FFD700><b>[Database View]</b></color>\n\n";

                    databaseText += $"<b>ID:</b> {item.Data.Id}\n";
                    databaseText += $"<b>Display Name:</b> {item.Data.DisplayName}\n";
                    databaseText += $"<b>Max Stack:</b> {item.Data.StackSize}\n";
                    databaseText += $"<b>Description:</b> {item.Data.Description}\n";

                    // Extract the Addressables GUID (RuntimeKey)
                    if (item.Data.IconSprite != null && item.Data.IconSprite.RuntimeKeyIsValid())
                    {
                        databaseText += $"<b>Asset Key:</b> <color=#ADD8E6>{item.Data.IconSprite.RuntimeKey}</color>";
                    }

                    lines.Add(databaseText);
                };
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
    }
}