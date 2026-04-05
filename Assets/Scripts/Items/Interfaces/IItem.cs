using System;

namespace Project.Items
{
    /// <summary>
    /// Runtime instance of an Item
    /// </summary>
    public interface IItem : IEquatable<IItem>
    {
        event Action<int> OnCountChanged;
        event Action<IItemData> OnDataChanged;
        event Action<IItem> OnItemChanged;
        IItemData Data { get; }
        int Count { get; set; }
        IItem Clone();
    }
}