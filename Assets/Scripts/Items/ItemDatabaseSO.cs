using System.Collections.Generic;
using UnityEngine;
namespace Project.Items
{
    /// Idea was to create database. But then I thought about memory management and removed this. Now I use Addressable groups as database
    
    //[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Project/Items/Item Database")]
    //public class ItemDatabaseSO : ScriptableObject, IItemDatabase
    //{

    //    // ──────────────────────────────
    //    // Serialized & Private Fields
    //    // ──────────────────────────────

    //    [SerializeField]
    //    private List<ItemDataSO> _items;

    //    /// <summary>
    //    /// for quick search of an item data by it's Id.
    //    /// </summary>
    //    private Dictionary<string, ItemDataSO> _itemsTable = null;

    //    // ──────────────────────────────
    //    // Constructors
    //    // ──────────────────────────────

    //    public void Initialize()
    //    {
    //        _itemsTable = new Dictionary<string, ItemDataSO>();
    //        foreach (var item in _items)
    //        {
    //            if (!_itemsTable.ContainsKey(item.Id))
    //            {
    //                _itemsTable.Add(item.Id, item);
    //            }
    //            else
    //            {
    //                Debug.LogError($"Duplicate Item ID found in database: {item.Id}");
    //            }
    //        }
    //    }


    //    // ──────────────────────────────
    //    // Public methods
    //    // ──────────────────────────────

    //    public IReadOnlyList<IItemData> GetAllItems()
    //    {
    //        return _items;
    //    }

    //    public bool TryGetItemById(string id, out IItemData data)
    //    {
    //        if (_itemsTable == null) Initialize();
    //        if(_itemsTable.TryGetValue(id, out var itemData))
    //        {
    //            data = itemData;
    //            return true;
    //        }
    //        data = null;
    //        return false;
    //    }
    //}
}