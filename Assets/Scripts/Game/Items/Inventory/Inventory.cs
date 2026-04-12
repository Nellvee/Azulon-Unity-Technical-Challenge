using Project.Items.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Items._Inventory
{
    /// <summary>
    /// 
    /// </summary>
    /// TODO:
    /// 1. change how inventory works from just a List of Items to list of InventorySlot.
    /// Each InventorySlot will handle it's own Item reference. This will add functionality to move items inside inventory between slots.
    /// Same with slot types, unique interactions, etc.
    public class Inventory : IInventory
    {

        // ──────────────────────────────
        // Events
        // ──────────────────────────────

        public event Action<int> OnItemsCountChanged;
        public event Action<IItem> OnItemAdded;
        public event Action<IItem> OnItemRemoved;
        public event Action<IInventory> OnInventoryChanged;

        protected virtual void NotifyInventoryChanged()
        {
            OnInventoryChanged?.Invoke(this);
        }

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        private int _maxCapacity = 10;

        private List<IItem> _items = new();

        private readonly GameObject _owner;
        private readonly IItemFactory _itemFactory;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        public int MaxCapacity => _maxCapacity;
        public IReadOnlyList<IItem> Items => _items;

        // ──────────────────────────────
        // Constructors
        // ──────────────────────────────

        public Inventory(GameObject owner, IItemFactory itemFactory, int maxCapacity)
        {
            _owner = owner;
            _itemFactory = itemFactory;
            _maxCapacity = maxCapacity;
        }

        // ──────────────────────────────
        // Public methods
        // ──────────────────────────────

        public bool HasItem(string itemId, int count)
        {
            int totalAmount = _items.Where(i => i.Data.Id == itemId).Sum(i => i.Count);
            return totalAmount >= count;
        }

        public async Task<bool> TryAddItemById(string itemId, int count)
        {
            if (_itemFactory == null)
            {
                Debug.LogError("Inventory: Factory is not initialized!");
                return false;
            }

            // Wait for the factory to build the item
            IItem newItem = await _itemFactory.CreateItemAsync(itemId, count);

            if (newItem != null)
            {
                return TryAddItem(newItem);
            }
            return false;
        }

        public bool TryAddItem(IItem item)
        {
            //counter of items that needs to be added
            int remainingToAdd = item.Count;
            //find all same not fully stacked items
            //0 stack size for now is Infinite (int.MaxValue)
            IEnumerable<IItem> stackableItems = _items.Where(i => i.Equals(item) && i.Count < i.Data.StackSize);
            //list of actions we call to adding all items in all stackable, IF we have enough space after all.
            List<Action> addActionsToStackableItems = new();
            foreach (IItem stackItem in stackableItems)
            {
                int spaceLeft = stackItem.Data.StackSize - stackItem.Count;
                int toAdd = Mathf.Min(spaceLeft, remainingToAdd);

                // Capture the variable for the action
                int countToAddInAction = toAdd;
                addActionsToStackableItems.Add(() => stackItem.Count += countToAddInAction);

                remainingToAdd -= toAdd;
                if (remainingToAdd <= 0) break;
            }

            int newSlotsNeeded = 0;
            if (remainingToAdd > 0)
            {
                newSlotsNeeded = Mathf.CeilToInt((float)remainingToAdd / item.Data.StackSize);
            }

            if (_items.Count + newSlotsNeeded > _maxCapacity)
            {
                Debug.Log("Failed to Add Item. Inventory is full.");
                return false;
            }
            addActionsToStackableItems.ForEach(x => x.Invoke());

            for (int i = 0; i < newSlotsNeeded; i++)
            {
                IItem newItemInSlot = item.Clone();
                int amountForThisSlot = Mathf.Min(remainingToAdd, item.Data.StackSize);

                newItemInSlot.Count = amountForThisSlot;
                _items.Add(newItemInSlot);
                remainingToAdd -= amountForThisSlot;
                OnItemAdded?.Invoke(newItemInSlot);
                OnItemsCountChanged?.Invoke(_items.Count);
                NotifyInventoryChanged();
            }
            ExecuteBehaviour(item, behaviour => behaviour.OnAcquired(CreateContext(item)));
            return true;
        }
        /// <summary>
        /// Reduces count of <paramref name="item"/> by <paramref name="count"/>. If reduced to zero, deletes an item from inventory.
        /// </summary>
        /// <param name="item">reference of an item inside Inventory</param>
        /// <param name="count">how much to remove from this item</param>
        /// <returns></returns>
        public bool TryRemoveItem(IItem item, int count)
        {
            // Use ReferenceEquals to ensure we are talking about THIS specific object instance
            int index = _items.FindIndex(i => object.ReferenceEquals(i, item));

            if (index == -1 || item.Count < count)
            {
                Debug.LogWarning("Item instance not found in inventory or insufficient count.");
                return false;
            }

            item.Count -= count;

            if (item.Count <= 0)
            {
                // Remove specifically at the index we found
                _items.RemoveAt(index);

                OnItemRemoved?.Invoke(item);
                OnItemsCountChanged?.Invoke(_items.Count);
                NotifyInventoryChanged();
            }

            ExecuteBehaviour(item, behaviour => behaviour.OnLost(CreateContext(item)));
            return true;
        }
        /// <summary>
        /// Removes all available count of items for this item Id.
        /// </summary>
        /// <returns></returns>
        public bool TryRemoveById(string itemId, int countToRemove)
        {
            //Check if we have enough in total
            if (!HasItem(itemId, countToRemove)) return false;

            int remainingToRemove = countToRemove;
            //collection of all items by id
            IEnumerable<IItem> stacks = _items.Where(i => i.Data.Id == itemId);

            List<Action> removeActionsToStackableItems = new();


            foreach (IItem stackItem in stacks)
            {
                if (stackItem.Count <= remainingToRemove)
                {
                    // This stack is smaller than or equal to what we need to remove
                    remainingToRemove -= stackItem.Count;

                    int countToRemoveInAction = stackItem.Count;
                    removeActionsToStackableItems.Add(() =>
                    {
                        TryRemoveItem(stackItem, countToRemoveInAction);
                    });
                }
                else
                {
                    int countToRemoveInAction = remainingToRemove;
                    // This stack has more than we need
                    removeActionsToStackableItems.Add(() =>
                    {
                        stackItem.Count -= countToRemoveInAction;
                        Debug.Log($"Removed count: {stackItem}");
                    });
                    remainingToRemove = 0;
                    break;
                }

                if (remainingToRemove <= 0) break;
            }

            if(remainingToRemove <= 0)
            {
                removeActionsToStackableItems.ForEach(x => x.Invoke());
                return true;
            }
            Debug.Log($"Inventory: Failed to remove {countToRemove} Items({itemId})");
            return false;
        }

        public void UseItem(IItem item)
        {
            // Safety check: ensure the item is actually in THIS inventory
            if (!_items.Contains(item)) return;

            // Execute behaviors
            ExecuteBehaviour(item, behaviour => behaviour.OnUsed(CreateContext(item)));
        }


        // ──────────────────────────────
        // Private methods
        // ──────────────────────────────

        private ItemContext CreateContext(IItem item)
        {
            return new ItemContext
            {
                Item = item,
                Inventory = this,
                Owner = _owner,
            };
        }
        private void ExecuteBehaviour(IItem item, Action<IItemBehaviour> action)
        {
            if (item?.Data?.Behaviours == null) return;

            foreach (var behaviour in item.Data.Behaviours)
            {
                action?.Invoke(behaviour);
            }
        }
    }
}