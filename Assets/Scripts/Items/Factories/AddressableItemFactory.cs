using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Project.Items
{
    public class AddressableItemFactory : IItemFactory
    {
        // We optionally keep a cache of handles so we don't load the same SO twice,
        // and so we can release them properly later.
        private Dictionary<string, AsyncOperationHandle<ItemDataSO>> _handleCache = new();

        public async Task<IItem> CreateItemAsync(string id, int amount = 1)
        {
            AsyncOperationHandle<ItemDataSO> handle;

            // Check if we already have this asset loaded in memory
            if (_handleCache.TryGetValue(id, out var cachedHandle))
            {
                handle = cachedHandle;
            }
            else
            {
                // Start the asynchronous load from disk
                handle = Addressables.LoadAssetAsync<ItemDataSO>(id);
                _handleCache[id] = handle;
            }

            // Wait for the operation to complete
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return CreateItem(handle.Result, amount);
            }
            else
            {
                Debug.LogError($"Failed to load Addressable Item with ID: {id}");
                _handleCache.Remove(id);
                return null;
            }
        }

        public IItem CreateItem(IItemData data, int amount = 1)
        {
            return new Item(data, amount);
        }
    }
}