using Project._CollectionLedger;
using Project.Items;
using TMPro;
using UnityEngine;
namespace Project.UI._CollectionLedger
{
    public class CollectionProgressViewUI : MonoBehaviour
    {

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _progressText;

        private CollectionManager _collectionManager;

        // ──────────────────────────────
        // Contructors \ Initialize
        // ──────────────────────────────

        public void Initialize(CollectionManager collectionManager)
        {
            _collectionManager = collectionManager;

            _collectionManager.OnNewDiscovery += CollectionManager_OnNewDiscovery;

            SetTitleText(_collectionManager.Registry.Id);
        }

        public void Dispose()
        {
            if (_collectionManager != null)
            {
                _collectionManager.OnNewDiscovery -= CollectionManager_OnNewDiscovery;
                _collectionManager = null;
            }
        }

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────

        private void OnDestroy()
        {
            Dispose();
        }

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────

        private void CollectionManager_OnNewDiscovery(IItemData itemData)
        {
            float percent = _collectionManager.GetCompletionPercentage();
            int currentCount = _collectionManager.DiscoveredCount;
            int totalCount = _collectionManager.TotalCount;
            string progress = $"{currentCount}/{totalCount} ({percent}%)";
            SetProgressText(progress);
        }

        private void SetProgressText(string text)
        {
            _progressText.text = text;
        }
        private void SetTitleText(string title)
        {
            _titleText.text = title;
        }
    }
}