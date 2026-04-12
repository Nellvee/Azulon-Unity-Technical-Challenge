using Project.Items.Behaviours;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Project.Items
{
    /// <summary>
    /// Data structure of an Item
    /// </summary>
    public interface IItemData
    {
        string Id { get; }
        /// <summary>
        /// change this and other display texts to localized string in future
        /// </summary>
        string DisplayName { get; }
        string Description { get; }
        int StackSize { get; }
        AssetReferenceSprite IconSprite { get; }
        IReadOnlyList<IItemBehaviour> Behaviours { get; }

        //something else?...quality?..tags?..WorldItemAssetReference?..
    }
}