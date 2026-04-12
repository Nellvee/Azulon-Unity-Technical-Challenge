using Project.Items.Behaviours;

namespace Project.Items
{
    public interface IItemBehaviour
    {
        string Id { get; }
        void OnAcquired(ItemContext context);
        void OnUsed(ItemContext context);
        void OnLost(ItemContext context);
    }
}