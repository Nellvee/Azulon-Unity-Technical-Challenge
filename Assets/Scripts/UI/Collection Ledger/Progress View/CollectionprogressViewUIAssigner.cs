using Project._CollectionLedger;
using UnityEngine;
namespace Project.UI._CollectionLedger
{
    [RequireComponent(typeof(CollectionProgressViewUI))]
    public class CollectionprogressViewUIAssigner : MonoBehaviour
    {
        [SerializeField]
        private CollectionManagerInitializer _collectionManagerInitializer;

        private CollectionProgressViewUI _collectionProgressView;

        private void Awake()
        {
            TryGetComponent(out _collectionProgressView);
        }
        private void Start()
        {
            //Init on start, as collection manager itself initializes on Awake
            //skipped sanity check for now
            _collectionProgressView.Initialize(_collectionManagerInitializer.CollectionManager);
        }
    }
}