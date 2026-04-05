using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI.Items
{
    /// <summary>
    /// Base class for high-performance virtualized grids.
    /// </summary>
    public abstract class VirtualizedGridUI<T> : MonoBehaviour
    {
        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [Header("UI References")]
        [SerializeField]
        protected ScrollRect _scrollRect;
        [SerializeField]
        protected RectTransform _content;
        [SerializeField]
        protected InventorySlotUI _slotPrefab;

        [Header("Grid Settings")]
        [SerializeField]
        protected int _columns = 5;
        [SerializeField]
        protected Vector2 _cellSize = new Vector2(100, 100);
        [SerializeField]
        protected Vector2 _spacing = new Vector2(10, 10);

        protected IReadOnlyList<T> _dataSource;

        private Queue<InventorySlotUI> _slotPool = new();
        private Dictionary<int, InventorySlotUI> _activeSlots = new();
        private int _previousFirstVisibleIndex = -1;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────

        // ──────────────────────────────
        // Events
        // ──────────────────────────────

        public event Action<InventorySlotUI, T> OnSlotBound;
        public event Action<InventorySlotUI> OnSlotUnbound;

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────

        protected virtual void Awake()
        {
            _slotPrefab.gameObject.SetActive(false);
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        protected virtual void OnDestroy()
        {
            _scrollRect.onValueChanged.RemoveListener(OnScroll);
            ClearActiveSlots();
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────

        public virtual void SetData(IReadOnlyList<T> data)
        {
            _dataSource = data;
            RebuildGrid();
        }

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────

        /// <summary>
        /// Logic for how the specific T data binds to the UI slot.
        /// </summary>
        protected abstract void BindSlot(InventorySlotUI slot, T data);
        protected abstract void UnbindSlot(InventorySlotUI slot);

        protected void RebuildGrid()
        {
            if (_dataSource == null) return;

            // Force all currently visible slots back into the pool
            // This ensures that when UpdateVisibleSlots runs, it RE-BINDS everything
            ClearActiveSlots();

            // Calculate Content Height
            int totalItems = _dataSource.Count;
            int rows = Mathf.CeilToInt((float)totalItems / _columns);
            float contentHeight = rows * (_cellSize.y + _spacing.y) + _spacing.y;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, contentHeight);

            // 3. Reset the index tracker and refresh
            _previousFirstVisibleIndex = -1;
            UpdateVisibleSlots();
        }

        private void OnScroll(Vector2 scrollPos) => UpdateVisibleSlots();

        private void UpdateVisibleSlots()
        {
            if (_dataSource == null || _dataSource.Count == 0)
            {
                ClearActiveSlots();
                return;
            }

            // Math for visible boundaries
            float scrollY = _content.anchoredPosition.y;
            float rowHeight = _cellSize.y + _spacing.y;
            int firstVisibleRow = Mathf.Max(0, Mathf.FloorToInt(scrollY / rowHeight));
            int firstVisibleIndex = firstVisibleRow * _columns;

            if (firstVisibleIndex == _previousFirstVisibleIndex) return;
            _previousFirstVisibleIndex = firstVisibleIndex;

            float viewportHeight = _scrollRect.viewport.rect.height;
            int visibleRows = Mathf.CeilToInt(viewportHeight / rowHeight) + 2;
            int lastVisibleIndex = Mathf.Min(firstVisibleIndex + (visibleRows * _columns), _dataSource.Count);

            // Recycle off-screen slots
            List<int> keysToRemove = new();
            foreach (var kvp in _activeSlots)
            {
                if (kvp.Key < firstVisibleIndex || kvp.Key >= lastVisibleIndex)
                {
                    UnbindSlot(kvp.Value);
                    RecycleSlot(kvp.Value);
                    OnSlotUnbound?.Invoke(kvp.Value);
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (var key in keysToRemove) _activeSlots.Remove(key);

            // Bind newly visible slots
            for (int i = firstVisibleIndex; i < lastVisibleIndex; i++)
            {
                if (!_activeSlots.ContainsKey(i))
                {
                    InventorySlotUI slot = GetSlotFromPool();
                    PositionSlot(slot, i);
                    BindSlot(slot, _dataSource[i]);
                    OnSlotBound?.Invoke(slot, _dataSource[i]);
                    _activeSlots[i] = slot;
                }
            }
        }

        private void PositionSlot(InventorySlotUI slot, int index)
        {
            int row = index / _columns;
            int col = index % _columns;
            float xPos = _spacing.x + (col * (_cellSize.x + _spacing.x));
            float yPos = -_spacing.y - (row * (_cellSize.y + _spacing.y));

            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            slot.gameObject.SetActive(true);
        }

        private InventorySlotUI GetSlotFromPool()
        {
            if (_slotPool.Count > 0) return _slotPool.Dequeue();

            InventorySlotUI newSlot = Instantiate(_slotPrefab, _content);
            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = _cellSize;
            return newSlot;
        }

        private void RecycleSlot(InventorySlotUI slot)
        {
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }

        private void ClearActiveSlots()
        {
            foreach (var slot in _activeSlots.Values)
            {
                UnbindSlot(slot);
                RecycleSlot(slot);
                OnSlotUnbound?.Invoke(slot);
            }
            _activeSlots.Clear();
        }

        
    }
}