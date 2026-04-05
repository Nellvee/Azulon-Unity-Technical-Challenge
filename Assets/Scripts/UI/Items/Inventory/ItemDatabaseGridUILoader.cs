using UnityEngine;
namespace Project.UI.Items
{
    /// <summary>
    /// Just simple loader of <see cref="ItemDatabaseGridUI"/> at Start()
    /// </summary>
    [RequireComponent(typeof(ItemDatabaseGridUI))]
    public class ItemDatabaseGridUILoader : MonoBehaviour
    {
        [Tooltip("Label of Addressable items")]
        [SerializeField]
        private string _itemLabel;

        private ItemDatabaseGridUI _itemDatabase;
        private void Awake()
        {
            TryGetComponent(out _itemDatabase);
        }
        private void Start()
        {
            _itemDatabase.LoadDatabase(_itemLabel);
        }
    }
}