using Project.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Project._CollectionLedger
{
    /// <summary>
    /// Right now it contains only 1 registry.
    /// it can only handle 1 registry with discovered Ids. In future we need many registries that can have same Ids.
    /// TODO: be able to contain many registries to handle more.
    /// 
    /// </summary>
    public class CollectionManager
    {
        //private const string SaveKey = "CollectionProgress";

        // ──────────────────────────────
        // Events
        // ──────────────────────────────

        /// <summary>
        /// This event triggers the "Visual Feedback" (Popups, sparkles, etc.)
        /// </summary>
        public event Action<IItemData> OnNewDiscovery;
        /// <summary>
        /// Triggers when all items were discovered
        /// </summary>
        public event Action OnFullyDiscovered;

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        private readonly CollectionRegistrySO _registry;
        private readonly Dictionary<string, IItemData> _registryItems;
        private HashSet<string> _discoveredIds = new();

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        public CollectionRegistrySO Registry => _registry;
        public int DiscoveredCount => _discoveredIds.Count;
        public int TotalCount => _registryItems.Count;

        // ──────────────────────────────
        // Constructors
        // ──────────────────────────────

        public CollectionManager(CollectionRegistrySO registry, IInventory playerInventory)
        {
            _registry = registry;
            //LoadProgress();

            _registryItems = registry.AllItems.ToDictionary(x => x.Id, x => x);

            // Subscribe to the inventory to "witness" new items
            playerInventory.OnItemAdded += HandleItemAdded;
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────

        public bool IsDiscovered(string id)
        {
            return _discoveredIds.Contains(id);
        }

        public float GetCompletionPercentage()
        {
            if (_registry.TotalCount == 0) return 0;
            return ((float)_discoveredIds.Count / _registryItems.Count) * 100f;
        }

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────

        private void HandleItemAdded(IItem item)
        {
            if (!_discoveredIds.Contains(item.Data.Id) && _registryItems.ContainsKey(item.Data.Id))
            {
                _discoveredIds.Add(item.Data.Id);
                //SaveProgress();
                
                OnNewDiscovery?.Invoke(item.Data);

                //trigger full discovery
                if(DiscoveredCount == TotalCount)
                {
                    OnFullyDiscovered?.Invoke();
                }
            }
        }

        //private void SaveProgress()
        //{
        //    string data = string.Join(",", _discoveredIds);
        //    PlayerPrefs.SetString(SaveKey, data);
        //    PlayerPrefs.Save();
        //}

        //private void LoadProgress()
        //{
        //    string saved = PlayerPrefs.GetString(SaveKey, "");
        //    if (!string.IsNullOrEmpty(saved))
        //    {
        //        _discoveredIds = new HashSet<string>(saved.Split(','));
        //    }
        //}
    }
}