using UnityEngine;
namespace Project.Items.Behaviours
{
    /// <summary>
    /// Created by EditorWindow: Tools -> Items -> Item Behaviours
    /// </summary>
    public abstract class ItemBehaviourSO : ScriptableObject, IItemBehaviour
    {
        [SerializeField]
        private string _id;
        public string Id => _id;
        public virtual void OnAcquired(ItemContext context) { }
        public virtual void OnLost(ItemContext context) { }
        public virtual void OnUsed(ItemContext context) { }
    }
}