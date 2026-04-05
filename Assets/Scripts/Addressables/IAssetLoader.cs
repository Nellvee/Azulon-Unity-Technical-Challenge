using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project._Addressables
{
    public interface IAssetLoader
    {
        public Awaitable<T> LoadAsync<T>(AssetReferenceT<T> reference, CancellationToken token = default, Action<float> progress = null) where T : UnityEngine.Object;
        public bool TryGet<T>(AssetReferenceT<T> reference, out T value) where T : UnityEngine.Object;
        public void Unload(AssetReference reference);
        public void UnloadAll();
    }
}