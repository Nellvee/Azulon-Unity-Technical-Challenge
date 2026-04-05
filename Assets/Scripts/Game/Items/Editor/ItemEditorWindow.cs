using Project.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class ItemEditorWindow : EditorWindow
{
    // Configuration
    private const string BASE_ITEM_PATH = "Assets/GameData/Items";
    private const string TARGET_GROUP_NAME = "Items";

    // Internal State
    private Vector2 _scrollPos;
    private List<Type> _itemTypes;
    private int _selectedTypeIndex;
    private int _toolbarIndex = 0;
    private string[] _toolbarOptions = { "Create New", "Search & Edit", "Utilities" };

    // Create Tab Fields
    private string _newItemId = "new_item_01";
    private string _category = "General";

    // Search Tab Fields
    private string _searchQuery = "";
    private List<ItemDataSO> _foundItems = new List<ItemDataSO>();
    private ItemDataSO _selectedItem;
    private Editor _cachedEditor;

    [MenuItem("Tools/Item Manager")]
    public static void ShowWindow() => GetWindow<ItemEditorWindow>("Item Manager");

    private void OnEnable()
    {
        RefreshItemTypes();
        RefreshSearch();
    }

    private void OnGUI()
    {
        _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _toolbarOptions);
        GUILayout.Space(10);

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        switch (_toolbarIndex)
        {
            case 0: DrawCreateTab(); break;
            case 1: DrawSearchTab(); break;
            case 2: DrawUtilityTab(); break;
        }

        EditorGUILayout.EndScrollView();
    }

    #region Create Tab
    private void DrawCreateTab()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Quick Create", EditorStyles.boldLabel);

        _newItemId = EditorGUILayout.TextField("Item ID / Filename", _newItemId);
        _category = EditorGUILayout.TextField("Category (Subfolder)", _category);

        bool hasIllegalChars = _newItemId.Any(c => Path.GetInvalidFileNameChars().Contains(c));

        if (hasIllegalChars)
        {
            EditorGUILayout.HelpBox("ID contains illegal characters!", MessageType.Error);
            GUI.enabled = false;
        }
        else if (IsIdCollision(_newItemId))
        {
            EditorGUILayout.HelpBox("ID Collision: This ID is already in use!", MessageType.Error);
            GUI.enabled = false;
        }

        if (_itemTypes != null && _itemTypes.Count > 0)
        {
            string[] typeNames = _itemTypes.Select(t => t.Name).ToArray();
            _selectedTypeIndex = EditorGUILayout.Popup("Item Class Type", _selectedTypeIndex, typeNames);
        }

        if (GUILayout.Button("Create Item Asset", GUILayout.Height(30)))
        {
            QuickCreate(_newItemId, _category, _itemTypes[_selectedTypeIndex]);
        }

        GUI.enabled = true;
        EditorGUILayout.EndVertical();
    }

    private void QuickCreate(string newItemId, string category, Type selectedType)
    {
        ItemDataSO newItem = CreateItemAsset(newItemId, category, selectedType);
        RefreshSearch();

        _selectedItem = newItem;
        _cachedEditor = Editor.CreateEditor(_selectedItem);
        _toolbarIndex = 1;
    }

    private ItemDataSO CreateItemAsset(string newItemId, string category, Type selectedType)
    {
        string targetFolder = $"{BASE_ITEM_PATH}/{category}";
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
            AssetDatabase.Refresh();
        }

        string fullPath = $"{targetFolder}/{newItemId}.asset";
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        ItemDataSO newItem = CreateInstance(selectedType) as ItemDataSO;

        var idField = typeof(ItemDataSO).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);
        if (idField != null) idField.SetValue(newItem, newItemId);

        AssetDatabase.CreateAsset(newItem, fullPath);
        RegisterWithAddressables(newItem, fullPath, selectedType);

        AssetDatabase.SaveAssets();

        return newItem;
    }
    #endregion

    #region Search & Edit Tab
    private void DrawSearchTab()
    {
        EditorGUILayout.BeginHorizontal();
        _searchQuery = EditorGUILayout.TextField("Search ID", _searchQuery, EditorStyles.toolbarSearchField);
        if (GUI.changed) RefreshSearch();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        // Grouped List
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        string lastCategory = "";
        foreach (var item in _foundItems)
        {
            if (item == null) continue;

            string currentCategory = Path.GetFileName(Path.GetDirectoryName(AssetDatabase.GetAssetPath(item)));
            if (currentCategory != lastCategory)
            {
                GUILayout.Space(5);
                GUILayout.Label(currentCategory.ToUpper(), EditorStyles.centeredGreyMiniLabel);
                lastCategory = currentCategory;
            }

            GUI.backgroundColor = (_selectedItem == item) ? Color.cyan : Color.white;
            if (GUILayout.Button($"{item.Id}", EditorStyles.miniButton))
            {
                _selectedItem = item;
                _cachedEditor = Editor.CreateEditor(_selectedItem);
                EditorGUIUtility.PingObject(_selectedItem);
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        // Editor Column
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (_selectedItem != null && _cachedEditor != null)
        {
            EditorGUILayout.LabelField("Asset Reference", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_selectedItem, typeof(ItemDataSO), false);
            if (GUILayout.Button("Locate", GUILayout.Width(60)))
            {
                EditorGUIUtility.PingObject(_selectedItem);
                Selection.activeObject = _selectedItem;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            _cachedEditor.OnInspectorGUI();

            GUILayout.Space(20);
            if (GUILayout.Button("Delete Asset", GUILayout.Width(120)))
            {
                if (EditorUtility.DisplayDialog("Delete Item", $"Delete {_selectedItem.Id}?", "Yes", "No"))
                {
                    DeleteSelectedItem();
                }
            }
        }
        else
        {
            GUILayout.Label("Select an item to edit.");
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Utility Tab
    private void DrawUtilityTab()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Database Maintenance", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Force Sync All Items", GUILayout.Height(30))) SyncAllItems();
        if (GUILayout.Button("Validate Items (Health Check)", GUILayout.Height(30))) ValidateItems();
        if (GUILayout.Button("Clean Ghost Entries", GUILayout.Height(30))) CleanupGhostEntries();
        if (GUILayout.Button("Create Test Items", GUILayout.Height(30))) CreateTestItems();
        if (GUILayout.Button("Delete Test Items", GUILayout.Height(30))) DeleteTestItems();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Validation: Checks for empty IDs, missing addresses, or mismatches between ID and Address.", MessageType.Info);

        EditorGUILayout.EndVertical();
    }

    private void ValidateItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { BASE_ITEM_PATH });
        int errors = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);

            if (item == null) continue;

            // Check 1: Empty ID
            if (string.IsNullOrEmpty(item.Id))
            {
                Debug.LogError($"[Validation] <b>Empty ID:</b> Item at {path} has no ID string.", item);
                errors++;
            }

            // Check 2: Addressable Status
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings?.FindAssetEntry(guid);

            if (entry == null)
            {
                Debug.LogWarning($"[Validation] <b>Unregistered:</b> Item {item.Id} is not in Addressables.", item);
                errors++;
            }
            else if (entry.address != item.Id)
            {
                // Check 3: ID/Address Mismatch
                Debug.LogError($"[Validation] <b>Mismatch:</b> Item ID is '{item.Id}' but Address is '{entry.address}'", item);
                errors++;
            }
        }

        if (errors == 0) Debug.Log("<color=green><b>Health Check:</b> All items are valid and synchronized!</color>");
        else Debug.LogWarning($"<b>Health Check:</b> Found {errors} issues that need attention.");
    }
    #endregion

    #region Shared Logic
    private void RefreshSearch()
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { BASE_ITEM_PATH });
        _foundItems.Clear();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);

            if (item != null && (string.IsNullOrEmpty(_searchQuery) ||
                item.Id.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)))
            {
                _foundItems.Add(item);
            }
        }

        _foundItems = _foundItems
            .OrderBy(i => Path.GetDirectoryName(AssetDatabase.GetAssetPath(i)))
            .ThenBy(i => i.Id)
            .ToList();
    }

    private void SyncAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { BASE_ITEM_PATH });
        int count = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemDataSO item = AssetDatabase.LoadAssetAtPath<ItemDataSO>(path);
            if (item != null)
            {
                RegisterWithAddressables(item, path, item.GetType());
                count++;
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Synced {count} items.");
        RefreshSearch();
    }

    private void CleanupGhostEntries()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        int removedCount = 0;
        foreach (var group in settings.groups)
        {
            var entries = group.entries.ToList();
            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(entry.guid)))
                {
                    group.RemoveAssetEntry(entry);
                    removedCount++;
                }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Cleaned {removedCount} ghost entries.");
    }
    private void CreateTestItems()
    {
        for (int i = 0; i < 100; i++)
        {
            string itemId = $"testItem_{i}";
            CreateItemAsset(itemId, "Test", typeof(ItemDataSO));
        }
    }
    private void DeleteTestItems()
    {
        string targetFolder = $"{BASE_ITEM_PATH}/{"Test"}";
        AssetDatabase.DeleteAsset(targetFolder);
    }
    private void DeleteSelectedItem()
    {
        string path = AssetDatabase.GetAssetPath(_selectedItem);
        string guid = AssetDatabase.AssetPathToGUID(path);
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null) settings.RemoveAssetEntry(guid);
        AssetDatabase.DeleteAsset(path);
        _selectedItem = null;
        _cachedEditor = null;
        RefreshSearch();
    }

    private void RefreshItemTypes()
    {
        _itemTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(ItemDataSO).IsAssignableFrom(type) && !type.IsAbstract)
            .ToList();
    }

    private void RegisterWithAddressables(ItemDataSO asset, string assetPath, Type itemType)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        AddressableAssetGroup group = settings.FindGroup(TARGET_GROUP_NAME);
        if (group == null) group = settings.CreateGroup(TARGET_GROUP_NAME, false, false, true, null);

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.CreateOrMoveEntry(guid, group);

        entry.address = asset.Id;
        entry.SetLabel("ItemData", true, true);
        entry.SetLabel(itemType.Name, true, true);
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
    }

    private bool IsIdCollision(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return false;
        return settings.groups.SelectMany(g => g.entries).Any(e => e.address == id);
    }
    #endregion
}