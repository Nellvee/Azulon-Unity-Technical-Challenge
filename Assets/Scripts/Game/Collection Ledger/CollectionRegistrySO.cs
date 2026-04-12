using Project.Items;
using System.Collections.Generic;
using UnityEngine;
namespace Project._CollectionLedger
{
    /// <summary>
    /// Created by EditorWindow: Tools -> Collection Ledger -> Registry
    /// </summary>
    public class CollectionRegistrySO : ScriptableObject
    {
        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [SerializeField]
        private string _id;
        [SerializeField]
        private List<ItemDataSO> _allItems;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────
        public string Id => _id;
        public IReadOnlyList<IItemData> AllItems => _allItems;
        public int TotalCount => _allItems.Count;
    }
}