using UnityEngine;
namespace Project.Items.Behaviours
{
    /// <summary>
    /// context struct used to pass references when something happens. 
    /// <br/>Like: Used, Aquired, Lost and any other events in future
    /// </summary>
    public struct ItemContext
    {
        public IItem Item;
        public IInventory Inventory;
        public GameObject Owner;
    }
}