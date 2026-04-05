using System.Threading.Tasks;
using UnityEngine;
namespace Project.Items
{
    /// this Item factory was used with an Idea of databases. see <see cref="IItemDatabase"/>
    /// 

    //public class ItemFactory : IItemFactory
    //{
    //    private readonly IItemDatabase _database;

    //    public ItemFactory(IItemDatabase database)
    //    {
    //        _database = database;
    //    }

    //    public IItem CreateItem(IItemData data, int count = 1)
    //    {
    //        //
    //        return new Item(data, count);
    //    }

    //    public async Task<IItem> CreateItemAsync(string id, int count = 1)
    //    {
    //        await Task.Yield();

    //        IItemData data;
    //        if(!_database.TryGetItemById(id, out data))
    //        {
    //            Debug.LogError($"ItemFactory: item with ID {id} not found");
    //            return null;
    //        }
    //        return CreateItem(data, count);
    //    }
    //}
}