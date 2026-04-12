using Project._CollectionLedger;
using Project.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.UI._CollectionLedger
{
    public class CollectionDiscoveryPopupUI : MonoBehaviour
    {
        public struct DiscoveryQueueData
        {
            public IItemData ItemData;
            public int DiscoveryCount;
            public int CollectionTotalCount;
        }


        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────
        [Header("References")]
        [SerializeField]
        private CollectionPopupUI _popupPrefab;
        [SerializeField]
        private Transform _popupContent;
        [Header("Configurations")]
        [SerializeField]
        private float _popupFadeDuration = 0.5f;
        [SerializeField]
        private float _popupStayTimeDuration = 1f;

        private CollectionManager _collectionManager;
        private ObjectPool<CollectionPopupUI> _popupsPool;
        private readonly List<CollectionPopupUI> _activePopups = new();

        // The Queue and State tracker
        private readonly Queue<DiscoveryQueueData> _discoveryQueue = new();
        private bool _isDisplaying;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        // ──────────────────────────────
        // Constructors \ Initializers
        // ──────────────────────────────

        public void Initialize(CollectionManager collectionManager)
        {
            _collectionManager = collectionManager;

            _collectionManager.OnNewDiscovery += EnqueueDiscovery;
        }
        public void Dispose()
        {
            if (_collectionManager != null)
            {
                _collectionManager.OnNewDiscovery -= EnqueueDiscovery;
                _collectionManager = null;
            }
        }

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────

        private void Awake()
        {
            //in case prefab is on scene
            _popupPrefab.gameObject.SetActive(false);

            _popupsPool = new ObjectPool<CollectionPopupUI>(OnCreatePopup, OnGetPopup, OnReleasePopup, OnDestroyPopup);
        }

        private void OnDestroy()
        {
            Dispose();
            StopAllCoroutines();
            _isDisplaying = false;
            _discoveryQueue.Clear();

            _popupsPool.Dispose();
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────

        private void EnqueueDiscovery(IItemData itemData)
        {
            _discoveryQueue.Enqueue(new DiscoveryQueueData()
            {
                ItemData = itemData,
                DiscoveryCount = _collectionManager.DiscoveredCount,
                CollectionTotalCount = _collectionManager.TotalCount
            });

            // If we aren't already showing something, start the process
            if (!_isDisplaying)
            {
                StartCoroutine(ProcessQueueRoutine());
            }
        }
        private IEnumerator ProcessQueueRoutine()
        {
            _isDisplaying = true;

            while (_discoveryQueue.Count > 0)
            {
                DiscoveryQueueData nextItem = _discoveryQueue.Dequeue();
                yield return StartCoroutine(FadeAndScalePopupRoutine(nextItem));
            }

            _isDisplaying = false;
        }

        private IEnumerator FadeAndScalePopupRoutine(DiscoveryQueueData discoveryData)
        {
            CollectionPopupUI popup = _popupsPool.Get();

            // Set data
            string title = "Congratulations!";
            string info = discoveryData.DiscoveryCount < discoveryData.CollectionTotalCount
                ? $"You discovered {(string.IsNullOrEmpty(discoveryData.ItemData.DisplayName) ? discoveryData.ItemData.Id : discoveryData.ItemData.DisplayName)}"
                : $"Collection Complete: {_collectionManager.Registry.Id}";
            popup.SetData(title, info);

            Transform t = popup.transform;

            float duration = _popupFadeDuration;
            float stayTime = _popupStayTimeDuration;

            // --- ANIMATION: Fade In & Scale Up ---
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                popup.SetCanvasGroupAlpha(Mathf.Lerp(0, 1, progress));
                t.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, progress); // Scale from 50% to 100%
                yield return null;
            }

            yield return new WaitForSeconds(stayTime);

            // --- ANIMATION: Fade Out & Scale Down ---
            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                popup.SetCanvasGroupAlpha(Mathf.Lerp(1, 0, progress));
                t.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, progress);
                yield return null;
            }

            _popupsPool.Release(popup);
        }


        // ──────────────────────────────
        // Pool
        // ──────────────────────────────

        private CollectionPopupUI OnCreatePopup()
        {
            CollectionPopupUI popup = Instantiate(_popupPrefab, _popupContent);
            popup.gameObject.SetActive(false);
            popup.SetCanvasGroupAlpha(0);
            return popup;
        }

        private void OnGetPopup(CollectionPopupUI popup)
        {
            popup.gameObject.SetActive(true);

            _activePopups.Add(popup);
        }

        private void OnReleasePopup(CollectionPopupUI popup)
        {
            popup.gameObject.SetActive(false);

            _activePopups.Remove(popup);
        }

        private void OnDestroyPopup(CollectionPopupUI popup)
        {
            Destroy(popup.gameObject);
        }

    }
}
