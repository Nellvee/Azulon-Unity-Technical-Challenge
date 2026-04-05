using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AddressableAtlasBridge : MonoBehaviour
{
    // Store handles by atlas name so we can release them later
    private static readonly Dictionary<string, AsyncOperationHandle<SpriteAtlas>> _loadedAtlases = new();

    private void OnEnable() => SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    private void OnDisable() => SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
    private void OnDestroy() 
    { 
        SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
    }

    private void OnAtlasRequested(string tag, System.Action<SpriteAtlas> action)
    {
        Debug.Log($"[AddressableAtlasBridge] OnAtlasRequested: {tag}");
        // If already loading or loaded, use the existing handle
        if (_loadedAtlases.TryGetValue(tag, out var handle))
        {
            if (handle.IsDone)
            {
                action(handle.Result);
            }
            else
            {
                handle.Completed += h => action(h.Result);
            }
            return;
        }

        // First time loading this atlas
        var loadHandle = Addressables.LoadAssetAsync<SpriteAtlas>(tag);
        _loadedAtlases[tag] = loadHandle;

        loadHandle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
                action(h.Result);
            else
                _loadedAtlases.Remove(tag); // Remove if failed so we can try again
        };
    }

    [ContextMenu("UnloadAllAtlases")]
    public void UnloadAllAtlasesLocal()
    {
        UnloadAllAtlases();
    }
    // Call this when changing scenes or major game states
    public static void UnloadAllAtlases()
    {
        foreach (var handle in _loadedAtlases.Values)
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
        _loadedAtlases.Clear();
    }
}