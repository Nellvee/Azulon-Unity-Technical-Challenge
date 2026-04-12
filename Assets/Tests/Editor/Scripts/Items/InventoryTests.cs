using NUnit.Framework;
using Project.Items;
using Project.Items._Inventory;
using Project.Items.Behaviours;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Testing.Editor.Items
{
    public class InventoryTests
    {
        // --- Mocks ---
        private class MockItemData : IItemData
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public int StackSize { get; set; }

            public AssetReferenceSprite IconSprite => throw new System.NotImplementedException();

            public IReadOnlyList<IItemBehaviour> Behaviours => new List<IItemBehaviour>();
        }

        private class MockFactory : IItemFactory
        {
            public IItem CreateItem(IItemData data, int amount = 1) => new Item(data, amount);

            // Simulates async loading
            public async Task<IItem> CreateItemAsync(string id, int amount = 1)
            {
                await Task.Yield();
                return new Item(new MockItemData { Id = id, StackSize = 64 }, amount);
            }
        }

        private Inventory _inventory;
        private MockFactory _factory;
        private MockItemData _swordData;

        [SetUp]
        public void Setup()
        {
            GameObject player = new GameObject();
            _factory = new MockFactory();
            _inventory = new Inventory(player, _factory, 5); // 5 slots max
            _swordData = new MockItemData { Id = "iron_sword", StackSize = 1 };
        }

        #region Basic Adding & Removal
        [Test]
        public void AddItem_WhenEmpty_OccupiesOneSlot()
        {
            var item = new Item(_swordData, 1);

            bool success = _inventory.TryAddItem(item);

            Assert.IsTrue(success);
            Assert.AreEqual(1, _inventory.Items.Count);
        }

        [Test]
        public void AddItem_WhenFull_ReturnsFalse()
        {
            // Fill 5 slots
            for (int i = 0; i < 5; i++)
            {
                _inventory.TryAddItem(new Item(new MockItemData { Id = $"item_{i}", StackSize = 1 }, 1));
            }

            bool success = _inventory.TryAddItem(new Item(_swordData, 1));

            Assert.IsFalse(success, "Inventory should not add item when capacity is reached.");
        }
        #endregion

        #region Stacking Logic
        [Test]
        public void AddItem_ExistingPartialStack_FillsExistingStack()
        {
            var potionData = new MockItemData { Id = "potion", StackSize = 10 };
            _inventory.TryAddItem(new Item(potionData, 5)); // Slot 0 has 5/10

            _inventory.TryAddItem(new Item(potionData, 3)); // Should add to Slot 0

            Assert.AreEqual(1, _inventory.Items.Count, "Should still only occupy 1 slot.");
            Assert.AreEqual(8, _inventory.Items[0].Count);
        }

        [Test]
        public void AddItem_OverflowsToNewSlot_WhenStackIsFull()
        {
            var potionData = new MockItemData { Id = "potion", StackSize = 10 };
            _inventory.TryAddItem(new Item(potionData, 8)); // Slot 0 has 8/10

            // Add 5 more. 2 should go to Slot 0, 3 should go to Slot 1.
            _inventory.TryAddItem(new Item(potionData, 5));

            Assert.AreEqual(2, _inventory.Items.Count);
            Assert.AreEqual(10, _inventory.Items[0].Count);
            Assert.AreEqual(3, _inventory.Items[1].Count);
        }
        #endregion

        #region Complex Removal
        [Test]
        public void RemoveById_SpanningMultipleStacks_RemovesCorrectly()
        {
            var oreData = new MockItemData { Id = "iron_ore", StackSize = 10 };
            _inventory.TryAddItem(new Item(oreData, 10)); // Slot 0
            _inventory.TryAddItem(new Item(oreData, 10)); // Slot 1
            _inventory.TryAddItem(new Item(oreData, 5));  // Slot 2

            // Total 25. Remove 15.
            bool success = _inventory.TryRemoveById("iron_ore", 15);

            Assert.IsTrue(success);
            //slot 0 deleted, slot 1 - 5 remaining, slot 5 still 5.
            Assert.AreEqual(10, _inventory.Items.Sum(i => i.Count));
            Assert.AreEqual(2, _inventory.Items.Count);
        }
        [Test]
        public void RemoveById_Multiple_FromLast()
        {
            var oreData = new MockItemData { Id = "iron_ore", StackSize = 10 };
            Item item = new Item(oreData, 10);
            Item item2 = new Item(oreData, 1);
            _inventory.TryAddItem(item);
            _inventory.TryAddItem(item2);
            //slot 0 - 10, slot 1 - 1

            Debug.Log("Inventory:");
            foreach (var i in _inventory.Items)
            {
                Debug.Log($"Item: {i}");
            }
            var itemToRemove = _inventory.Items[1];
            Debug.Log($"Removing item: {itemToRemove}");
            bool success = _inventory.TryRemoveItem(itemToRemove, 1);
            Assert.IsTrue(success);
            Debug.Log("Inventory after remove:");
            foreach (var i in _inventory.Items)
            {
                Debug.Log($"Item: {i}");
            }
            Assert.AreEqual(10, _inventory.Items.Sum(i => i.Count));
            Assert.AreEqual(1, _inventory.Items.Count);
            Assert.AreEqual(item.Data, _inventory.Items[0].Data);
            Assert.AreEqual(10, _inventory.Items[0].Count);

        }

        [Test]
        public void RemoveById_Multiple_FromLast2()
        {
            var oreData = new MockItemData { Id = "iron_ore", StackSize = 1 };
            Item item = new Item(oreData, 1);
            Item item2 = new Item(oreData, 1);
            _inventory.TryAddItem(item);
            _inventory.TryAddItem(item2);
            //slot 0 - 1, slot 1 - 1

            Debug.Log("Inventory:");
            foreach (var i in _inventory.Items)
            {
                Debug.Log($"Item: {i}");
            }
            var itemToRemove = _inventory.Items[1];
            Debug.Log($"Removing item: {itemToRemove}");
            bool success = _inventory.TryRemoveItem(itemToRemove, 1);
            Assert.IsTrue(success);
            Debug.Log("Inventory after remove:");
            foreach (var i in _inventory.Items)
            {
                Debug.Log($"Item: {i}");
            }
            Assert.AreEqual(1, _inventory.Items.Sum(i => i.Count));
            Assert.AreEqual(1, _inventory.Items.Count);
            Assert.AreEqual(item.Data, _inventory.Items[0].Data);
            Assert.AreEqual(1, _inventory.Items[0].Count);

        }
        #endregion

        #region Async Factory Tests
        [Test]
        public async Task AddItemById_CallsFactoryAndAddsToInventory()
        {
            bool success = await _inventory.TryAddItemById("magic_staff", 1);

            Assert.IsTrue(success);
            Assert.AreEqual("magic_staff", _inventory.Items[0].Data.Id);
        }
        #endregion

        #region Event Verification
        [Test]
        public void AddItem_TriggersOnItemAddedEvent()
        {
            IItem eventItem = null;
            _inventory.OnItemAdded += (item) => eventItem = item;

            _inventory.TryAddItem(new Item(_swordData, 1));

            Assert.IsNotNull(eventItem);
            Assert.AreEqual(_swordData.Id, eventItem.Data.Id);
        }
        #endregion

    }
}