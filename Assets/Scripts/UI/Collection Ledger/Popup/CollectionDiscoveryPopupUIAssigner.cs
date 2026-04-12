using Project._CollectionLedger;
using UnityEngine;

namespace Project.UI._CollectionLedger
{
    [RequireComponent(typeof(CollectionDiscoveryPopupUI))]
    public class CollectionDiscoveryPopupUIAssigner : MonoBehaviour
    {
        [SerializeField]
        private CollectionManagerInitializer _collectionManagerInitializer;

        private CollectionDiscoveryPopupUI _collectionDiscoveryPopup;

        private void Awake()
        {
            TryGetComponent(out _collectionDiscoveryPopup);
        }
        private void Start()
        {
            //Init on start, as collection manager itself initializes on Awake
            //skipped sanity check for now
            _collectionDiscoveryPopup.Initialize(_collectionManagerInitializer.CollectionManager);
        }
    }
}
