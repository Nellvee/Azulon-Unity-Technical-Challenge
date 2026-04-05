using Project._Addressables;
using Project.Items;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Project.UI.Items
{
    public class InventorySlotUI : MonoBehaviour
    {

        // ──────────────────────────────
        // Serialized & Private Fields
        // ──────────────────────────────

        [SerializeField] 
        private Image _icon;
        [SerializeField] 
        private TextMeshProUGUI _countText;
        [SerializeField]
        private Button _button;

        private IItem _boundItem;
        private AssetReference _loadedSpriteReference;

        // ──────────────────────────────
        // Properties
        // ──────────────────────────────
        public RectTransform RectTransform => (RectTransform)transform;
        public IItem BoundItem => _boundItem;


        // ──────────────────────────────
        // Events
        // ──────────────────────────────

        public event Action<InventorySlotUI> OnSlotClicked;
        public event Action<IItem> OnBoundItemChanged;

        // ──────────────────────────────
        // Unity Callbacks
        // ──────────────────────────────
        private void Awake()
        {
            _button.onClick.AddListener(() => OnSlotClicked?.Invoke(this));
        }
        private void OnDestroy()
        {
            Unbind();
        }

        // ──────────────────────────────
        // Public Methods
        // ──────────────────────────────
        public void Bind(IItem item)
        {
            Unbind();
            _boundItem = item;

            if (_boundItem != null)
            {
                // Subscribe to count changes
                _boundItem.OnItemChanged += UpdateIcon;
                _boundItem.OnCountChanged += UpdateCountText;
                UpdateIcon(_boundItem);
                UpdateCountText(_boundItem.Count);
            }

            OnBoundItemChanged?.Invoke(BoundItem);
        }

        public void Unbind()
        {
            UnloadIcon();

            if (_boundItem != null)
            {
                _boundItem.OnItemChanged -= UpdateIcon;
                _boundItem.OnCountChanged -= UpdateCountText;
            }
            _boundItem = null;
            _icon.sprite = null;
            _countText.text = "";

            OnBoundItemChanged?.Invoke(BoundItem);
        }

        // ──────────────────────────────
        // Protected & Private Methods
        // ──────────────────────────────

        private void UpdateCountText(int count)
        {
            _countText.text = count > 1 ? count.ToString() : "";
        }

        private void UpdateIcon(IItem item)
        {
            UnloadIcon();
            // For now, we'll assume a helper loads the sprite.
            LoadIcon(item.Data);
        }

        private async void LoadIcon(IItemData data)
        {
            // Store the item we are TRYING to load
            IItem currentRequest = _boundItem;

            _icon.enabled = false;
            Sprite loadedIcon = await AssetReferenceLoaderHelper.LoadAsync(data.IconSprite);

            // If the slot was recycled while we were waiting, 
            // _boundItem will no longer match currentRequest. Do not apply the sprite.
            if (_boundItem != currentRequest)
            {
                // We don't need this sprite anymore; someone else is handling it now
                return;
            }

            if (loadedIcon != null)
            {
                _icon.sprite = loadedIcon;
                _icon.enabled = true;
                _loadedSpriteReference = data.IconSprite;
            }
        }
        private void UnloadIcon()
        {
            if (_loadedSpriteReference != null)
            {
                AssetReferenceLoaderHelper.Unload(_loadedSpriteReference);
                _loadedSpriteReference = null;
            }
        }
    }
}