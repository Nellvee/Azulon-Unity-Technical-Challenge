using System.Threading;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Project._Addressables
{
    public static class AddressablesAwaitableExtensions
    {
        public static async Awaitable<T> AsAwaitable<T>(this AsyncOperationHandle<T> handle, CancellationToken token = default)
        {
            // If it's already done, return immediately
            if (handle.IsDone) return handle.Result;

            // Create a completion source that works with Unity's Awaitable
            var completionSource = new AwaitableCompletionSource<T>();

            // Link the Addressables 'Completed' event to our Awaitable
            handle.Completed += h =>
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                    completionSource.SetResult(h.Result);
                else
                    completionSource.SetException(h.OperationException ?? new System.Exception("Addressable Load Failed"));
            };

            // Handle Cancellation
            using (token.Register(() => completionSource.SetCanceled()))
            {
                return await completionSource.Awaitable;
            }
        }
    }
}