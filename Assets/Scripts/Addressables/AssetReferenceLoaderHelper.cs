using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project._Addressables
{
    public static class AssetReferenceLoaderHelper
    {
        private static IAssetLoader _loader = new AssetReferenceLoader();
        public static async Awaitable<T> LoadAsync<T>(AssetReferenceT<T> reference, CancellationToken token = default, Action<float> progress = null) where T : UnityEngine.Object
        {
            return await _loader.LoadAsync(reference, token, progress);

        }
        public static bool TryGet<T>(AssetReferenceT<T> reference, out T value) where T : UnityEngine.Object
        {
            return _loader.TryGet(reference, out value);
        }
        public static void Unload(AssetReference reference)
        {
            _loader.Unload(reference);
        }
        public static void UnloadAll()
        {
            _loader.UnloadAll();
        }
    }
}