using UnityEngine;
using UnityEngine.AddressableAssets;
namespace Project.Items
{
    /// <summary>
    /// ItemDataSO, please use Unity -> Tools -> Item Manager to create item data so.
    /// </summary>
    public class ItemDataSO : ScriptableObject, IItemData
    {

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [SerializeField]
        private string _id;
        [SerializeField]
        private string _displayName;
        [SerializeField, TextArea(3,15)]
        private string _description;

        [Tooltip("0 - infinite (int.MaxValue)")]
        [SerializeField]
        private int _stackSize;

        [Header("Visuals")]
        [SerializeField]
        private AssetReferenceSprite _iconSprite;
        [Tooltip("World model prefab of an Item")]
        [SerializeField]
        private AssetReferenceGameObject _worldPrefab;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        public string Id => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        /// <summary>
        /// zero or less than zero for now is Infinite (int.MaxValue)
        /// </summary>
        public int StackSize
        {
            get
            {
                if(_stackSize <= 0)
                {
                    return int.MaxValue;
                }
                return _stackSize;
            }
        }
        public AssetReferenceSprite IconSprite => _iconSprite;
        public AssetReferenceGameObject WorldPrefab => _worldPrefab;


        // ──────────────────────────────
        // Public methods
        // ──────────────────────────────

        public override string ToString()
        {
            return $"Data(ID({Id}), StackSize({StackSize}))";
        }

        // ──────────────────────────────
        // Editor
        // ──────────────────────────────
#if UNITY_EDITOR
        // Ensure the ID is never empty
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
        }
#endif
    }
}