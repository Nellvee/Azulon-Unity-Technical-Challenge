using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Items
{
    public interface IInventory
    {
        event Action<int> OnItemsCountChanged;
        event Action<IItem> OnItemAdded;
        event Action<IItem> OnItemRemoved;
        event Action<IInventory> OnInventoryChanged;

        IReadOnlyList<IItem> Items { get; }
        bool TryAddItem(IItem item);
        bool TryRemoveItem(IItem item, int count);
        Task<bool> TryAddItemById(string itemId, int count);
        bool TryRemoveById(string itemId, int countToRemove);
        bool HasItem(string itemId, int count);

    }
}