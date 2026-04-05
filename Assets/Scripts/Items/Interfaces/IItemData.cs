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
        //something else?...quality?..tags?..
    }
}