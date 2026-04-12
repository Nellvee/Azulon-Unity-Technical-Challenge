using UnityEngine;
namespace Project.Items.Behaviours
{
    public class TestBehaviour : ItemBehaviourSO
    {
        public override void OnAcquired(ItemContext context)
        {
            Debug.Log($"TestBehaviour: Item {context.Item} was aquired by {context.Owner.name}");
        }
        public override void OnUsed(ItemContext context)
        {
            Debug.Log($"TestBehaviour: Item {context.Item} was used by {context.Owner.name}");
        }
        public override void OnLost(ItemContext context)
        {
            Debug.Log($"TestBehaviour: Item {context.Item} was lost by {context.Owner.name}");
        }
    }
}