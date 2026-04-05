using System.Threading.Tasks;

namespace Project.Items
{
    /// <summary>
    /// How items are created
    /// </summary>
    public interface IItemFactory
    {
        /// <summary>
        /// Creates an item from it's <paramref name="data"/>
        /// </summary>
        /// <remarks>
        /// Items will require loading.
        /// </remarks>
        /// <returns>
        /// Runtime object of an <see cref="IItem"/>
        /// </returns>
        Task<IItem> CreateItemAsync(string id, int count = 1);
        IItem CreateItem(IItemData data, int count = 1);
    }
}