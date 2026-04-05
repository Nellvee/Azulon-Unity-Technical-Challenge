using UnityEngine;
using Project.Characters;
namespace Project.UI.Items
{

    [RequireComponent(typeof(PlayerInventoryGridUI))]
    public class PlayerInventoryGridUIAssigner : MonoBehaviour
    {
        [SerializeField]
        private Player _player;

        private PlayerInventoryGridUI _playerInventoryUI;

        private void Awake()
        {
            TryGetComponent(out _playerInventoryUI);
            AssignPlayerInventory();
        }

        private void OnValidate()
        {
            TryGetComponent(out _playerInventoryUI);
            AssignPlayerInventory();
        }

        private void AssignPlayerInventory()
        {
            if (_player != null && _player.Inventory != null)
            {
                _playerInventoryUI.Initialize(_player.Inventory);
            }
        }
    }
}