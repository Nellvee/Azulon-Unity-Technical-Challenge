using Project.Characters;
using System.Linq;
using UnityEngine;
namespace Project._CollectionLedger
{
    /// <summary>
    /// Test script to link collection manager with player. Or Should I Use just CollectionManager as Singleton<> ?
    /// </summary>
    public class CollectionManagerInitializer : MonoBehaviour
    {

        [SerializeField]
        private Player _player;
        [SerializeField]
        private CollectionRegistrySO _registrySO;

        private CollectionManager _collectionManager;

        public CollectionManager CollectionManager { get => _collectionManager; }

        private void Awake()
        {
            if (_player.IsInitialized)
            {
                OnPlayerInitialized(_player);
            }
            else
            {
                _player.OnInitialized += OnPlayerInitialized;
            }
        }

        private void OnPlayerInitialized(Player player)
        {
            _collectionManager = new CollectionManager(_registrySO, player.Inventory);
            player.OnInitialized -= OnPlayerInitialized;
        }
    }
}