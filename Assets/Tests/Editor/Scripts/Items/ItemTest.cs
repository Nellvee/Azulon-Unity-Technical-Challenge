using NUnit.Framework;
using Project.Items;
using Project.Items.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace Project.Testing.Editor.Items
{
    [TestFixture]
    public class ItemTest
    {
        private class MockItemData : IItemData
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public int StackSize { get; set; }

            public AssetReferenceSprite IconSprite => throw new System.NotImplementedException();

            public IReadOnlyList<IItemBehaviour> Behaviours => throw new System.NotImplementedException();
        }

        private MockItemData _testData;

        [SetUp]
        public void Setup()
        {
            _testData = new MockItemData { Id = "test_item", StackSize = 10 };
        }

        #region State & Initialization
        [Test]
        public void Constructor_SetsDataAndCountCorrectly()
        {
            var item = new Item(_testData, 5);

            Assert.AreEqual(_testData, item.Data);
            Assert.AreEqual(5, item.Count);
        }

        [Test]
        public void Constructor_NullData_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Item(null, 1));
        }
        #endregion

        #region Events
        [Test]
        public void SettingCount_TriggersOnCountChanged()
        {
            var item = new Item(_testData, 1);
            int receivedCount = 0;
            item.OnCountChanged += (count) => receivedCount = count;

            item.Count = 10;

            Assert.AreEqual(10, receivedCount);
        }

        [Test]
        public void SettingCount_TriggersOnItemChanged()
        {
            var item = new Item(_testData, 1);
            bool eventFired = false;
            item.OnItemChanged += (i) => eventFired = true;

            item.Count = 2;

            Assert.IsTrue(eventFired, "OnItemChanged should fire when Count is modified.");
        }

        [Test]
        public void SettingSameCount_DoesNotTriggerEvents()
        {
            var item = new Item(_testData, 5);
            int fireCount = 0;
            item.OnItemChanged += (i) => fireCount++;

            item.Count = 5; // Same value

            Assert.AreEqual(0, fireCount, "Events should not fire if the value hasn't changed.");
        }
        #endregion

        #region Equality & Cloning
        [Test]
        public void Equals_SameId_ReturnsTrue()
        {
            var itemA = new Item(_testData, 1);
            var itemB = new Item(new MockItemData { Id = "test_item" }, 5);

            Assert.IsTrue(itemA.Equals(itemB));
        }

        [Test]
        public void Equals_DifferentId_ReturnsFalse()
        {
            var itemA = new Item(_testData, 1);
            var itemB = new Item(new MockItemData { Id = "different_item" }, 1);

            Assert.IsFalse(itemA.Equals(itemB));
        }

        [Test]
        public void Clone_CreatesNewInstanceWithSameData()
        {
            var original = new Item(_testData, 7);
            var clone = original.Clone();

            Assert.AreNotSame(original, clone, "Clone should be a different object instance.");
            Assert.AreEqual(original.Data.Id, clone.Data.Id);
            Assert.AreEqual(original.Count, clone.Count);
        }
        #endregion
    }
}