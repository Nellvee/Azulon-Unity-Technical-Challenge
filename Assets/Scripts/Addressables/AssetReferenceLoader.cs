using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project._Addressables
{
    public class AssetReferenceLoader : IAssetLoader
    {
        // Use the RuntimeKey (object) as the key instead of the Reference instance
        private readonly Dictionary<object, AsyncOperationHandle> _handles = new();
        // Keep track of how many slots are using this specific asset
        private readonly Dictionary<object, int> _refCounts = new();
        public bool TryGet<T>(AssetReferenceT<T> reference, out T value) where T : UnityEngine.Object
        {
            if (reference != null && _handles.TryGetValue(reference, out var handle) && handle.IsDone)
            {
                value = handle.Result as T;
                return true;
            }
            value = null;
            return false;
        }
        public async Awaitable<T> LoadAsync<T>(AssetReferenceT<T> reference, CancellationToken token = default, Action<float> progress = null) where T : UnityEngine.Object
        {
            if (reference == null || !reference.RuntimeKeyIsValid()) return null;

            object key = reference.RuntimeKey;

            // If already in dictionary, increment count and return existing handle
            if (_handles.TryGetValue(key, out AsyncOperationHandle existingHandle))
            {
                _refCounts[key]++;
                if (existingHandle.IsDone) return existingHandle.Result as T;
                return await existingHandle.Convert<T>().AsAwaitable(token);
            }

            // using the key to load
            // This avoids the "Already loaded" error.
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

            _handles.Add(key, handle);
            _refCounts[key] = 1;

            try
            {
                if (progress != null) _ = ReportProgress(handle, progress, token);
                return await handle.AsAwaitable(token);
            }
            catch (Exception)
            {
                Unload(reference);
                throw;
            }
        }

        public void Unload(AssetReference reference)
        {
            if (reference == null || !reference.RuntimeKeyIsValid()) return;

            object key = reference.RuntimeKey;

            if (_handles.TryGetValue(key, out var handle))
            {
                _refCounts[key]--;

                // Only truly release from memory if no one is using it anymore
                if (_refCounts[key] <= 0)
                {
                    if (handle.IsValid()) Addressables.Release(handle);
                    _handles.Remove(key);
                    _refCounts.Remove(key);
                }
            }
        }

        private async Awaitable ReportProgress(AsyncOperationHandle handle, Action<float> progress, CancellationToken token)
        {
            // While the handle is still working and not canceled
            while (!handle.IsDone && !token.IsCancellationRequested)
            {
                progress?.Invoke(handle.PercentComplete);

                // Wait for the next frame to avoid pegging the CPU
                await Awaitable.NextFrameAsync(token);
            }

            // Ensure we send 100% at the end
            if (handle.Status == AsyncOperationStatus.Succeeded)
                progress?.Invoke(1f);
        }
        public void UnloadAll()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid()) Addressables.Release(handle);
            }
            _handles.Clear();
        }
    }
}