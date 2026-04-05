using Project.Characters;
using UnityEngine;
namespace Project.UI.Items
{
    /// <summary>
    /// Adds functionality to <see cref="ItemDatabaseGridUI"/> to add item to player inventory on UI slot click.
    /// </summary>
    [RequireComponent(typeof(ItemDatabaseGridUI))]
    public class ItemDatabaseGridUIPlayerItemGrabber : MonoBehaviour
    {
        [SerializeField]
        private Player _player;

        private ItemDatabaseGridUI _itemDatabase;

        private void Awake()
        {
            TryGetComponent(out _itemDatabase);
        }
        private void Start()
        {
            _itemDatabase.OnSlotItemClicked += ItemDatabase_OnSlotItemClicked;
            _itemDatabase.OnSlotBound += ItemDatabase_OnSlotBound;
        }
        private void OnDestroy()
        {
            _itemDatabase.OnSlotBound -= ItemDatabase_OnSlotBound;
            _itemDatabase.OnSlotItemClicked -= ItemDatabase_OnSlotItemClicked;
        }

        private void ItemDatabase_OnSlotItemClicked(InventorySlotUI slot)
        {
            _player.Inventory.TryAddItem(slot.BoundItem);
        }
        private void ItemDatabase_OnSlotBound(InventorySlotUI slot, Project.Items.IItemData data)
        {
            //Try to modify slot tooltip
            if (slot.TryGetComponent(out InventorySlotTooltipUI slotTooltip))
            {
                slotTooltip.OnRequestTooltipInfo += (item, lines) =>
                {
                    lines.Add($"<b>Try to click on item, to add this Item to Player</b>");
                };
            }
        }
    }
}